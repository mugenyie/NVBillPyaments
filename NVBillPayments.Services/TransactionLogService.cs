using Newtonsoft.Json;
using NVBillPayments.Core;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.Shared;
using NVBillPayments.Shared.ViewModels.Logs;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class TransactionLogService : ITransactionLogService
    {
        //private readonly IRepository<TransactionLog> _transactionLogRepository;
        //private readonly ConnectionFactory factory;
        //private readonly IConnection connection;
        //private readonly IModel channel;
        //private readonly string TransactionLogName = ConfigurationConstants.LOGS_QUEUE;

        //public TransactionLogService(IRepository<TransactionLog> transactionLogRepository)
        //{
        //    _transactionLogRepository = transactionLogRepository;

        //    factory = new ConnectionFactory
        //    {
        //        Uri = new Uri(ConfigurationConstants.RABBITMQ_URI)
        //    };
        //    connection = factory.CreateConnection();
        //    channel = connection.CreateModel();
        //}

        //public async Task AddTransactionLogAsync(string Title, string transactionData)
        //{
        //    await Task.Run(() =>
        //    {
        //        LogVM message = new LogVM
        //        {
        //            Title = Title,
        //            Data = transactionData
        //        };

        //        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        //        channel.BasicPublish(exchange: "",
        //                             routingKey: TransactionLogName,
        //                             basicProperties: null,
        //                             body: body);
        //    });
        //}

        public async Task AddTransactionLogAsync(string Title, string Data)
        {
            await Task.Run(() =>
            {
                //var transactionLogData = new TransactionLog
                //{
                //    LogId = Guid.NewGuid(),
                //    Title = Title,
                //    Metadata = Data,
                //    CreatedOnUTC = DateTime.UtcNow
                //};
                //_transactionLogRepository.Add(transactionLogData);
                //_transactionLogRepository.SaveChanges();
            });
        }
    }
}
