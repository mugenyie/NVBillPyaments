using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.Shared;
using NVBillPayments.Shared.ViewModels.Transaction;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NVBillPayments.WorkerService
{
    public class TransactionsWorker : BackgroundService
    {
        private readonly ILogger<TransactionsWorker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly string RabbitMQURI = ConfigurationConstants.RABBITMQ_URI;
        private readonly string QueueName = ConfigurationConstants.QUEUE_NAME;

        private IServiceScopeFactory _services { get; }

        public TransactionsWorker(ILogger<TransactionsWorker> logger, IServiceScopeFactory services)
        {
            _logger = logger;
            _services = services;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(ConfigurationConstants.RABBITMQ_URI),
                DispatchConsumersAsync = true
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //listen to transactions and process
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                _logger.LogInformation($"Processing msg: '{message}'.");
                try
                {
                    var transaction = JsonConvert.DeserializeObject<Transaction>(message);

                    using (var scope = _services.CreateScope())
                    {
                        var _serviceProvider = scope.ServiceProvider.GetRequiredService<IServiceProviderService>();
                        if (transaction.PaymentStatus == PaymentStatus.SUCCESSFUL)
                            await _serviceProvider.ProcessOrderAsync(transaction);
                    }
                    
                    _channel.BasicAck(e.DeliveryTag, false);
                }
                catch (JsonException)
                {
                    _logger.LogError($"JSON Parse Error: '{message}'.");
                    _channel.BasicNack(e.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                }
                catch (Exception exp)
                {
                    _logger.LogError(default, exp, exp.Message);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection is closed.");
        }
    }
}
