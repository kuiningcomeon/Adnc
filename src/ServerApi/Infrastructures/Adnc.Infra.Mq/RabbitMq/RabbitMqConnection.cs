﻿using System;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Polly;
using Microsoft.Extensions.Logging;

namespace Adnc.Infra.Mq.RabbitMq
{
    public sealed class RabbitMqConnection
    {
        private static volatile RabbitMqConnection _uniqueInstance;
        private static readonly object _lockObject = new object();
        private ILogger<dynamic> _logger;
        public IConnection Connection { get; private set; }

        public static RabbitMqConnection GetInstance(IOptionsSnapshot<RabbitMqConfig> options, ILogger<dynamic> logger)
        {
            if (_uniqueInstance == null || _uniqueInstance.Connection == null || _uniqueInstance.Connection.IsOpen == false)
            {
                lock (_lockObject)
                {
                    if (_uniqueInstance == null || _uniqueInstance.Connection == null || _uniqueInstance.Connection.IsOpen == false)
                    {
                        _uniqueInstance = new RabbitMqConnection(options.Value, logger);
                    }
                }
            }
            return _uniqueInstance;
        }

        private RabbitMqConnection(RabbitMqConfig mqOption, ILogger<dynamic> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = mqOption.HostName,
                VirtualHost = mqOption.VirtualHost,
                UserName = mqOption.UserName,
                Password = mqOption.Password,
                Port = mqOption.Port
                //Rabbitmq集群需要加这两个参数
                //AutomaticRecoveryEnabled = true,
                //TopologyRecoveryEnabled=true
            };

            Policy.Handle<SocketException>()
                  .Or<BrokerUnreachableException>()
                  .WaitAndRetry(6, retryAttempt => TimeSpan.FromSeconds(1), (ex, time, retryCount, content) =>
                  {
                      if (6 == retryCount)
                          throw ex;
                      _logger.LogError(ex, string.Format("{0}:{1}", retryCount, ex.Message));
                  })
                  .Execute(() =>
                  {
                      Connection = factory.CreateConnection();
                  });
        }
    }
}
