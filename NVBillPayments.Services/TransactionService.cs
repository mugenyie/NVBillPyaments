using Newtonsoft.Json;
using NVBillPayments.Core;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.PaymentProviders.DPO;
using NVBillPayments.PaymentProviders.Interswitch;
using NVBillPayments.PaymentProviders.Pegasus;
using NVBillPayments.PaymentProviders.Pesapal;
using NVBillPayments.ServiceProviders.AIRTELUG.Models;
using NVBillPayments.ServiceProviders.MTNUG;
using NVBillPayments.Services.Helpers;
using NVBillPayments.Shared;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.Helpers;
using NVBillPayments.Shared.ViewModels.Product;
using NVBillPayments.Shared.ViewModels.Transaction;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> _transactionsRepository;
        private readonly IRepository<CashBackPolicy> _cashBackPolicy;
        private readonly ConnectionFactory factory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string TransactionsQueueName = ConfigurationConstants.QUEUE_NAME;

        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly IServiceProviderService _serviceProvider;
        private readonly INotificationService _notificationService;
        private readonly ITransactionLogService _transactionLogService;
        private readonly ICachingService _cachingService;
        private readonly IMTNService _mtnService;
        private readonly IPesapalService _pesapalService;
        private readonly IPegasusService _pegasusService;
        private readonly IInterswitchService _interswitchService;

        public TransactionService(
            IRepository<Transaction> transactionsRepository,
            IRepository<CashBackPolicy> cashBackPolicy,
            IProductService productService, 
            IPaymentService paymentService, 
            IServiceProviderService serviceProvider,
            INotificationService notificationService,
            ITransactionLogService transactionLogService,
            ICachingService cachingService,
            IMTNService mtnService,
            IPesapalService pesapalService,
            IPegasusService pegasusService,
            IInterswitchService interswitchService)
        {
            factory = new ConnectionFactory { Uri = new Uri(ConfigurationConstants.RABBITMQ_URI) };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            _transactionsRepository = transactionsRepository;
            _cashBackPolicy = cashBackPolicy;
            _productService = productService;
            _paymentService = paymentService;
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _transactionLogService = transactionLogService;
            _cachingService = cachingService;
            _mtnService = mtnService;
            _pesapalService = pesapalService;
            _pegasusService = pegasusService;
            _interswitchService = interswitchService;
        }

        public void AddTransactionToQueue(object message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish(exchange: "",
                                 routingKey: TransactionsQueueName,
                                 basicProperties: null,
                                 body: body);
        }

        public async Task<string> CreateCardPaymentLinkV2(Transaction transaction, PaymentProvider paymentProvider)
        {
            if (paymentProvider == PaymentProvider.INTERSWITCH)
            {
                string link = _interswitchService.GeneratePaymentLink(transaction);

                transaction.PaymentProviderId = paymentProvider.ToString();
                transaction.ModifiedBy = $"CreateCardPaymentLink -{paymentProvider}-";
                transaction.ModifiedOnUTC = DateTime.UtcNow;
                transaction.PaymentReference = link;
                _transactionsRepository.Update(transaction);
                await _transactionsRepository.SaveChangesAsync();
                return link;
            }
            return "#";
        }

        public async Task<string> CreateCardPaymentLink(AddTransactionVM transaction, PaymentProvider paymentProvider = PaymentProvider.FLUTTERWAVE)
        {
            var _transactionRecord = await ProcessTransactionAsync(transaction);

            if (_transactionRecord.TransactionStatus != TransactionStatus.FAILED)
            {
                //var PesapalCheckoutURL = _pesapalService.GenerateCheckout(_transactionRecord);
                if(paymentProvider == PaymentProvider.FLUTTERWAVE)
                {
                    var flutterwaveCheckout = await _paymentService.InitiateFlutterwaveChargeCard(_transactionRecord);

                    _transactionRecord.PaymentProviderReponseMetaData = flutterwaveCheckout.Response;
                    _transactionRecord.PaymentProviderId = PaymentProvider.FLUTTERWAVE.ToString();
                    _transactionRecord.ModifiedBy = "CreateCardPaymentLink";
                    _transactionRecord.ModifiedOnUTC = DateTime.UtcNow;
                    _transactionRecord.PaymentReference = flutterwaveCheckout.Link;
                    _transactionsRepository.Update(_transactionRecord);
                    await _transactionsRepository.SaveChangesAsync();
                    return flutterwaveCheckout.Link;
                }
                else if(paymentProvider == PaymentProvider.INTERSWITCH)
                {
                    string link = _interswitchService.GeneratePaymentLink(_transactionRecord);

                    _transactionRecord.PaymentProviderId = paymentProvider.ToString();
                    _transactionRecord.ModifiedBy = "CreateCardPaymentLink";
                    _transactionRecord.ModifiedOnUTC = DateTime.UtcNow;
                    _transactionRecord.PaymentReference = link;
                    _transactionsRepository.Update(_transactionRecord);
                    await _transactionsRepository.SaveChangesAsync();
                    return link;
                }
                else if (paymentProvider == PaymentProvider.PEGASUS)
                {
                    string link = _pegasusService.GeneratePaymentLink(_transactionRecord);

                    _transactionRecord.PaymentProviderId = paymentProvider.ToString();
                    _transactionRecord.ModifiedBy = "CreateCardPaymentLink";
                    _transactionRecord.ModifiedOnUTC = DateTime.UtcNow;
                    _transactionRecord.PaymentReference = link;
                    _transactionsRepository.Update(_transactionRecord);
                    await _transactionsRepository.SaveChangesAsync();
                    return link;
                }
            }
            return null;
        }

        public async Task<Transaction> SaveTransactionAsync(Transaction transaction)
        {
            try
            {
                _transactionsRepository.Add(transaction);
                await _transactionsRepository.SaveChangesAsync();
                return transaction;
            }catch(Exception exp)
            {
                return null;
            }
        }

        public async Task<bool> MarkExpired(Transaction transaction)
        {
            try
            {
                transaction.IsExpired = true;
                _transactionsRepository.Update(transaction);
                await _transactionsRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception exp)
            {
            }
            return false;
        }

        public async Task<Transaction> ProcessTransactionAsync(AddTransactionVM transaction)
        {
            Transaction transactionRecord = new Transaction();
            //_transactionLogService.AddTransactionLogAsync("Processing transaction", JsonConvert.SerializeObject(transaction));

            if (transaction.ProductId.Equals("AIRTIME", StringComparison.OrdinalIgnoreCase))
            {
                if(ProductHelper.IsValidSubscriber(transaction.BeneficiaryId, "mtn"))
                {
                    transaction.ProductId = "MTNUG_AIRTIME";
                }
                else if(ProductHelper.IsValidSubscriber(transaction.BeneficiaryId, "airtel"))
                {
                    transaction.ProductId = "AIRTELUG_AIRTIME";
                }
                else
                {
                    transactionRecord = await RecordFailedTransaction(transaction, null, $"Unsupported Phonenumber Network {transaction.BeneficiaryId}", $"Unsupported Phonenumber Network {transaction.BeneficiaryId}");
                    goto COMPLETE;
                }
            }

            var product = _productService.Get(transaction.ProductId);
            try
            {
                if (product != null)
                {
                    if (!product.ServiceProvider.IsActive)
                    {
                        transactionRecord = await RecordFailedTransaction(transaction, product, $"Service Unavailable for {product.ServiceProvider.CompanyName}", $"Service Unavailable for {product.ServiceProvider.CompanyName}");
                        goto COMPLETE;
                    }

                    if (!product.IsActive)
                    {
                        transactionRecord = await RecordFailedTransaction(transaction, product, $"{product.Name} has been deactivated", $"{product.Name} has been deactivated");
                        goto COMPLETE;
                    }

                    //adjust mtn bundle
                    if ((bool)(product.Category.Name.Contains("bundles", StringComparison.OrdinalIgnoreCase) && product.ServiceProvider.CompanyName.Contains("mtn", StringComparison.OrdinalIgnoreCase)))
                    {
                        if(!ProductHelper.IsValidSubscriber(transaction.BeneficiaryId, "mtn"))
                        {
                            transactionRecord = await RecordFailedTransaction(transaction, product, $"Unsupported MTN Network Subscriber {transaction.BeneficiaryId}", $"Unsupported MTN Network Subscriber {transaction.BeneficiaryId}");
                            goto COMPLETE;
                        }

                        string customer_product_trackingId = $"_product_info_{transaction.ProductId}_{transaction.BeneficiaryId}_";
                        var productDetail = await _cachingService.Get<ProductDetailVM>(customer_product_trackingId);

                        if (productDetail != null)
                        {
                            transaction.Amount = productDetail.Amount;
                            product.Description = $"{productDetail?.Name}";
                        }
                        else
                        {
                            var productInfo = _mtnService.GetBundlePrice(transaction.BeneficiaryId, transaction.ProductId, customer_product_trackingId, true);
                            transaction.Amount = productInfo.data.amount;
                            product.Description = productInfo.data.name;
                        }
                    }

                    //adjust airtel bundle AirtelBundleMetaData
                    if ((bool)(product.Category.Name.Contains("bundles", StringComparison.OrdinalIgnoreCase) && product.ServiceProvider.CompanyName.Contains("airtel", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!ProductHelper.IsValidSubscriber(transaction.BeneficiaryId, "airtel"))
                        {
                            transactionRecord = await RecordFailedTransaction(transaction, product, $"Unsupported Airtel Network Subscriber {transaction.BeneficiaryId}", $"Unsupported MTN Network Subscriber {transaction.BeneficiaryId}");
                            goto COMPLETE;
                        }
                        transaction.Amount = product.Price;
                        var airtelBundleMeta = new AirtelBundleMetaData
                        {
                            Amount = (int)product.Price,
                            Selector = int.Parse(product.ProductId_2)
                        };
                        transaction.MetaData = JsonConvert.SerializeObject(airtelBundleMeta);
                    }

                    transactionRecord = AddTransactionRecordAsync(transaction, product).Result;

                    if (product.FreeCharge)
                    {
                        await ProcessFreeChargeProductAsync(transactionRecord);
                        goto COMPLETE;
                    }
                    else if (transaction.PaymentMethod.Equals("mobile", StringComparison.OrdinalIgnoreCase))
                    {
                        await InititateMobilePaymentCollectionAsync(transactionRecord);
                        goto COMPLETE;
                    }
                        
                }
                else
                {
                    _transactionLogService.AddTransactionLogAsync("Product Error", $"product not found -{JsonConvert.SerializeObject(transaction)}-");
                    transactionRecord = await RecordFailedTransaction(transaction, product, "product not found", $"The transaction processing was unsuccessful.");
                }
            }catch(Exception exp)
            {
                transactionRecord = await RecordFailedTransaction(transaction, product, exp.Message, $"The transaction processing was unsuccessful.");
            }
            COMPLETE:
            return transactionRecord;
        }

        public async Task ProcessFreeChargeProductAsync(Transaction transactionRecord)
        {
            transactionRecord.PaymentStatus = PaymentStatus.SUCCESSFUL;
            transactionRecord.PaymentStatusMsg = PaymentStatus.SUCCESSFUL.ToString();
            transactionRecord.AmountCharged = 0;
            transactionRecord.PaymentProviderId = "FREE_CHARGE";
            transactionRecord.PaymentProviderReponseMetaData = "FREE_CHARGE";
            transactionRecord.ModifiedBy = "Payment Processor";
            transactionRecord.ModifiedOnUTC = DateTime.UtcNow;

            _transactionsRepository.Update(transactionRecord);
            await _transactionsRepository.SaveChangesAsync();

            AddTransactionToQueue(transactionRecord);
            //_serviceProvider.ProcessOrderAsync(transactionRecord);
        }

        public async Task InititateMobilePaymentCollectionAsync(Transaction transaction)
        {
            decimal minimumCollectionAmount = 500;
            if(transaction.AmountToCharge < minimumCollectionAmount)
            {
                transaction.TechnicalStatusMessage = $"{transaction.CurrencyCode}{transaction.AmountToCharge} is below minimum collection of {transaction.CurrencyCode}{minimumCollectionAmount}";
                transaction.TransactionStatusMessage = $"{transaction.CurrencyCode}{transaction.AmountToCharge} is below minimum collection of {transaction.CurrencyCode}{minimumCollectionAmount}";
                transaction.TransactionStatus = TransactionStatus.FAILED;
                transaction.ModifiedBy = "Payment collection";
                transaction.ModifiedOnUTC = DateTime.UtcNow;
                _transactionsRepository.Update(transaction);
                await _transactionsRepository.SaveChangesAsync();
            }
            else
            {
                var paymentResponse = _paymentService.InitiateBeyonicMobileCollection(transaction.SponsorMSISDN, transaction.AmountToCharge, transaction.TransactionId.ToString());
                transaction.PaymentProviderReponseMetaData = JsonConvert.SerializeObject(paymentResponse);
                transaction.PaymentProviderId = PaymentProvider.BEYONIC.ToString();
                transaction.ModifiedBy = "InititateMobilePaymentCollection";
                transaction.ModifiedOnUTC = DateTime.UtcNow;
                _transactionsRepository.Update(transaction);
                await _transactionsRepository.SaveChangesAsync();
            }
        }

        public async Task ProcessSuccesfulPaymentCallback(string transactionId, decimal amountCharged, string paymentProviderId, string paymentMetaData)
        {
            var transaction = _transactionsRepository.GetById(new Guid(transactionId));
            if(transaction != null)
            {
                if(transaction.PaymentStatus != PaymentStatus.SUCCESSFUL)
                {
                    transaction.PaymentStatus = PaymentStatus.SUCCESSFUL;
                    transaction.PaymentStatusMsg = PaymentStatus.SUCCESSFUL.ToString();
                    transaction.AmountCharged = amountCharged == 0 ? transaction.AmountToCharge : amountCharged;
                    transaction.PaymentProviderId = paymentProviderId;
                    transaction.PaymentProviderReponseMetaData = paymentMetaData;
                    transaction.ModifiedBy = "Payment Processor";
                    transaction.ModifiedOnUTC = DateTime.UtcNow;

                    _transactionsRepository.Update(transaction);
                    await _transactionsRepository.SaveChangesAsync();
                }

                if(transaction.OrderStatus != OrderStatus.SUCCESSFUL)
                    await _serviceProvider.ProcessOrderAsync(transaction);
            }
        }

        public async Task ProcessFailedPayment(string transactionId, string paymentProviderId, string paymentMetaData, string statusMessage, string technicalStatusMessage = "")
        {
            var transaction = _transactionsRepository.GetById(new Guid(transactionId));

            if(transaction != null)
                if(transaction.TransactionStatus != TransactionStatus.FAILED)
                {
                    transaction.PaymentStatus = PaymentStatus.FAILED;
                    transaction.PaymentStatusMsg = PaymentStatus.FAILED.ToString();
                    transaction.OrderStatus = OrderStatus.FAILED;
                    transaction.OrderStatusMsg = OrderStatus.FAILED.ToString();
                    transaction.TransactionStatus = TransactionStatus.FAILED;
                    transaction.TransactionStatusMessage = statusMessage;
                    transaction.TechnicalStatusMessage = technicalStatusMessage;
                    transaction.AmountCharged = 0;
                    transaction.PaymentProviderId = paymentProviderId;
                    transaction.PaymentProviderReponseMetaData = paymentMetaData;
                    transaction.ModifiedBy = "Payment Processor";
                    transaction.ModifiedOnUTC = DateTime.UtcNow;
                    _transactionsRepository.Update(transaction);
                    await _transactionsRepository.SaveChangesAsync();
                    _notificationService.SendInAppAsync($"Failed transaction - {transaction.ProductDescription}", transaction.AccountEmail, $"Failure Reason: {transaction.TransactionStatusMessage}");
                }
        }

        public async Task ProcessOrderCallbackAsync(string transactionId, OrderStatus orderStatus, string serviceProviderMetaData)
        {
            var transaction = _transactionsRepository.GetById(new Guid(transactionId));

            if(transaction != null)
            {
                string customerMessage = $"{transaction.ProductDescription} for {transaction.BeneficiaryMSISDN}, {transaction.CurrencyCode} {Math.Round(transaction.AmountCharged, 0)}";

                var qrEmailTemplate = await _notificationService.GenerateTransactionEmailTemplateAsync(transaction);

                transaction.OrderStatus = orderStatus;
                transaction.OrderStatusMsg = orderStatus.ToString();
                transaction.ServiceProviderResponseMetaData = serviceProviderMetaData;
                transaction.TransactionStatus = transaction.OrderStatus == OrderStatus.SUCCESSFUL ? TransactionStatus.SUCCESSFUL : TransactionStatus.FAILED;
                transaction.ModifiedBy = "Order Processor";
                transaction.ModifiedOnUTC = DateTime.UtcNow;
                transaction.QRCodeUrl = qrEmailTemplate.Item1;
                _transactionsRepository.Update(transaction);
                await _transactionsRepository.SaveChangesAsync();
                _notificationService.SendInAppAsync($"Successful Transaction - {transaction.ProductDescription}", transaction.AccountEmail, customerMessage);
                _notificationService.SendEmailAsync(transaction.ProductDescription, transaction.AccountEmail, qrEmailTemplate.Item2, transaction.AccountName);
            }
        }

        public async Task<Transaction> AddTransactionRecordAsync(AddTransactionVM addTransactionVM, Product product)
        {
            var newTransaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                CreatedOnUTC = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.PENDING,
                PaymentStatusMsg = PaymentStatus.PENDING.ToString(),
                OrderStatus = OrderStatus.PENDING,
                OrderStatusMsg = OrderStatus.PENDING.ToString(),
                TransactionStatus = TransactionStatus.PENDING,
                ProductId = product.ProductId,
                ProductDescription = product.Description,
                ProductValue = addTransactionVM.Amount,
                AmountToCharge = addTransactionVM.Amount,
                ProductValidity = product?.Validity,
                AccountMSISDN = addTransactionVM.UserAccount.PhoneNumber,
                AccountName = addTransactionVM.UserAccount.Name,
                AccountEmail = addTransactionVM.UserAccount.Email,
                BeneficiaryMSISDN = addTransactionVM.BeneficiaryId,
                SponsorMSISDN = addTransactionVM.SponsorId,
                CreatedBy = "MOBILE APP",
                ServiceProviderId = product.ServiceProvider.ShortName,
                SystemCategory = product.SystemCategory.ToString(),
                MetaData = addTransactionVM.MetaData
            };

            newTransaction = AdjustCashbackValues(product, addTransactionVM, newTransaction);

            _transactionsRepository.Add(newTransaction);
            await _transactionsRepository.SaveChangesAsync();
            return newTransaction;
        }

        private Transaction AdjustCashbackValues(Product product, AddTransactionVM transactionVM, Transaction transaction)
        {
            string cashback_policy_key = $"product_{product.SystemCategory}_paymentmethod_{transactionVM.PaymentMethod}";

            var cashbackPolicy = _cashBackPolicy.Query()
                .Where(x => x.SystemCategory == product.SystemCategory && x.PaymentMethod.Equals(transactionVM.PaymentMethod) && x.IsActive)
                .FirstOrDefault();

            if(cashbackPolicy != null)
            {
                transaction.ProductValue = transactionVM.Amount + (transactionVM.Amount * (decimal)cashbackPolicy.Percentage/100);
                transaction.Cashback = cashbackPolicy.Percentage;
                return transaction;
            }
            else
            {
                return transaction;
            }
        }

        public async Task<Transaction> RecordFailedTransaction(AddTransactionVM transaction, Product product, string technicalError, string userFriendlyError)
        {
            string productDesc = product?.Description ?? transaction.ProductId;
            var newTransaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                CreatedOnUTC = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.PENDING,
                OrderStatus = OrderStatus.PENDING,
                TransactionStatus = TransactionStatus.FAILED,
                ProductId = product?.ProductId ?? transaction.ProductId,
                ProductDescription = productDesc,
                ProductValue = transaction.Amount,
                AccountMSISDN = transaction.UserAccount.PhoneNumber,
                AccountName = transaction.UserAccount.Name,
                AccountEmail = transaction.UserAccount.Email,
                BeneficiaryMSISDN = transaction.BeneficiaryId,
                SponsorMSISDN = transaction.SponsorId,
                CreatedBy = "MOBILE APP",
                ServiceProviderId = product?.ServiceProvider?.ShortName ?? "NONE",
                TechnicalStatusMessage = technicalError,
                TransactionStatusMessage = userFriendlyError,
                SystemCategory = product?.SystemCategory.ToString()
            };
            _transactionsRepository.Add(newTransaction);
            await _transactionsRepository.SaveChangesAsync();
            _notificationService.SendInAppAsync($"Failed transaction - {productDesc}", transaction.UserAccount.Email, $"Failure Reason: {userFriendlyError}");
            return newTransaction;
        }

        public Transaction GetById(string transactionId)
        {
            return _transactionsRepository.GetById(new Guid(transactionId));
        }

        public List<SimpleTransactionsVM> GetOrders(string email, string userId, string status, string category, int limit = 0, int offset = 10)
        {
            var transactionsVm = new List<SimpleTransactionsVM>();

            IQueryable<Transaction> transactions = null;

            if(!string.IsNullOrEmpty(email))
            {
                transactions = _transactionsRepository.Query().Where(x => email.Equals(x.AccountEmail));
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                transactions = _transactionsRepository.Query().Where(x => userId.Equals(x.ExternalUserId));
            }

            if (transactions?.Count() > 0 && !string.IsNullOrEmpty(status))
            {
                if(status.Equals("successful",StringComparison.OrdinalIgnoreCase))
                    transactions = transactions.Where(x => x.TransactionStatus == TransactionStatus.SUCCESSFUL);
                else if(status.Equals("failed", StringComparison.OrdinalIgnoreCase))
                    transactions = transactions.Where(x => x.TransactionStatus == TransactionStatus.FAILED);
                else if (status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                    transactions = transactions.Where(x => x.TransactionStatus == TransactionStatus.PENDING);

                if (!string.IsNullOrEmpty(category))
                    transactions = transactions.Where(x => x.SystemCategory.Equals(category.ToUpper()));
            }

            transactions = transactions?.OrderByDescending(x => x.CreatedOnUTC).Skip(offset).Take(limit);
            transactionsVm = TransactionHelper.ToSimpleListView(transactions?.ToList());

            return transactionsVm;
        }

        public async Task<object> GetThirdpartyTransactionStatusAsync(string transactionId)
        {
            Guid Id = new Guid(transactionId);
            var transaction = _transactionsRepository.GetById(Id);
            if (transaction.OrderStatus == OrderStatus.SUCCESSFUL)
                return transaction;
            else
            {
                switch (transaction.ServiceProviderId)
                {
                    case "MTNUG_TESTS":
                        {
                            if (transaction.ProductId.Contains("airtime",StringComparison.OrdinalIgnoreCase))
                            {
                                var response = await _serviceProvider.RetryMTNAirtimeRechargeAsync(transaction, false);
                                return response;
                            }
                            else
                            {
                                var response = _serviceProvider.GetMTNBundlesTransactionStatus(transaction, false);
                                return response;
                            }
                        }
                    case "MTNUG":
                        {
                            if (transaction.ProductId.Contains("airtime", StringComparison.OrdinalIgnoreCase))
                            {
                                var response = await _serviceProvider.RetryMTNAirtimeRechargeAsync(transaction, true);
                                return response;
                            }
                            else
                            {
                                var response = _serviceProvider.GetMTNBundlesTransactionStatus(transaction, true);
                                return response;
                            }
                        }
                    default:
                        {
                            return transaction;
                        }
                }
            }
        }

        public List<SimpleTransactionsVM> GetRecommendedOrders(string email, string category, bool singlePerCategory, int limit, int offset)
        {
            var transactionsVm = new List<SimpleTransactionsVM>();

            if (!string.IsNullOrEmpty(email))
            {
                if (singlePerCategory)
                {
                    var recomended =_transactionsRepository.Query()
                    .Where(x => email.Equals(x.AccountEmail) & x.TransactionStatus == TransactionStatus.SUCCESSFUL)
                    .OrderByDescending(x => x.CreatedOnUTC)
                    .Take(10)
                    .ToList();
                    var recommendedTransactions = new List<Transaction>
                    {
                        recomended.Where(x => x.SystemCategory.Equals("AIRTIME")).FirstOrDefault(),
                        recomended.Where(x => x.SystemCategory.Equals("BUNDLES")).FirstOrDefault()
                    };
                    transactionsVm = TransactionHelper.ToSimpleListView(recommendedTransactions);
                    return transactionsVm;
                }

                var transactions = _transactionsRepository.Query()
                    .Where(x => email.Equals(x.AccountEmail) & x.TransactionStatus == TransactionStatus.SUCCESSFUL)
                    .OrderByDescending(x => x.CreatedOnUTC).Skip(0).Take(20)
                    .ToList();

                if (!string.IsNullOrEmpty(category))
                {
                    transactions = transactions.Where(x => x.SystemCategory.Equals(category.ToUpper())).ToList();
                }

                var uniqueT = transactions.GroupBy(t => new { t.ProductId, t.BeneficiaryMSISDN, t.ProductValue }).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.CreatedOnUTC).Skip(offset).Take(limit).ToList();

                transactionsVm = TransactionHelper.ToSimpleListView(uniqueT);
            }

            return transactionsVm;
        }
    }
}
