using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Z.IIoT.HubConsumer
{
    public class Consumer
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        private IModel Model { get; set; }

        /// <summary>
        /// Gets or sets the connection to rabbit
        /// </summary>
        /// <value>The connection to rabbit</value>
        public IConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>The name of the queue.</value>
        public string QueueName { get; set; }

        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string RoutingKeyName { get; set; }
        public string ExchangeName { get; set; }
        
        /// <summary>
        /// Read a message from the queue.
        /// </summary>
        /// <param name="onDequeue">The action to take when recieving a message</param>
        /// <param name="onError">If an error occurs, provide an action to take.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="routingKeyName">Name of the routing key.</param>
        public void ReadFromQueue(Action<string, Consumer, ulong> onDequeue, Action<Exception, Consumer, ulong> onError, string exchangeName, string queueName, string routingKeyName)
        {
            BindToQueue(exchangeName, queueName, routingKeyName);

            var consumer = new EventingBasicConsumer(Model);
            
            // Receive the message from the queue and act on that message
            consumer.Received += (o, e) =>
            {
                var queuedMessage = Encoding.ASCII.GetString(e.Body);
                onDequeue.Invoke(queuedMessage, this, e.DeliveryTag);
            };

            // If the consumer shutdowns reconnect to rabbit and begin reading from the queue again.
            consumer.Shutdown += (o, e) =>
            {
                ConnectToRabbitMq();
                ReadFromQueue(onDequeue, onError, exchangeName, queueName, routingKeyName);
            };
            
            Model.BasicConsume(queueName, false, consumer);
        }

        /// <summary>
        /// Bind to a queue.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="routingKeyName">Name of the routing key.</param>
        private void BindToQueue(string exchangeName, string queueName, string routingKeyName)
        {
            const bool durable = false, autoDelete = false, exclusive = false;

            ExchangeName = exchangeName;
            QueueName = queueName;
            RoutingKeyName = routingKeyName;

            Model.BasicQos(0, 1, false);

            // replicate the queue to all hosts. Queue arguments are optional
            //IDictionary queueArgs = new Dictionary<string, object>
            //    {
            //        {"x-ha-policy", "all"}
            //    };
            QueueName = Model.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            if (!string.IsNullOrWhiteSpace(ExchangeName))
            {
                Model.QueueBind(queueName, exchangeName, routingKeyName, null);
            }
        }

        /// <summary>
        /// Connect to rabbit mq.
        /// </summary>
        /// <returns><c>true</c> if a connection to RabbitMQ has been made, <c>false</c> otherwise</returns>
        public bool ConnectToRabbitMq()
        {
            int attempts = 0;
            // make 3 attempts to connect to RabbitMQ just in case an interruption occurs during the connection
            while (attempts < 3)
            {
                attempts++;

                try
                {
                    var connectionFactory = new ConnectionFactory
                    {
                        HostName = HostName,
                        UserName = UserName,
                        Password = Password,
                        RequestedHeartbeat = 60
                    };
                    Connection = connectionFactory.CreateConnection();

                    // Create the model 
                    CreateModel();

                    return true;
                }
                catch (System.IO.EndOfStreamException ex)
                {
                    // Handle Connection Exception Here
                    return false;
                }
                catch (BrokerUnreachableException ex)
                {
                    // Handle Connection Exception Here
                    return false;
                }

                // wait before trying again
                Thread.Sleep(1000);
            }

            if (Connection != null)
                Connection.Dispose();

            return false;
        }

        /// <summary>
        /// Create a model.
        /// </summary>
        private void CreateModel()
        {
            Model = Connection.CreateModel();

            // When AutoClose is true, the last channel to close will also cause the connection to close. 
            // If it is set to true before any channel is created, the connection will close then and there.
            Connection.AutoClose = true;

            // Configure the Quality of service for the model. Below is how what each setting means.
            // BasicQos(0="Dont send me a new message untill I’ve finshed",  1= "Send me one message at a time", false ="Apply to this Model only")
            Model.BasicQos(0, 1, false);

            const bool durable = false, exchangeAutoDelete = true, queueAutoDelete = false, exclusive = false;

            // Create a new, durable exchange, and have it auto delete itself as long as an exchange name has been provided.
            if (!string.IsNullOrWhiteSpace(ExchangeName)) {
                Model.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable, exchangeAutoDelete, null);
                Model.QueueBind(QueueName, ExchangeName, RoutingKeyName, null);
            }
            Model.QueueDeclare(queue: QueueName, durable: durable, exclusive: exclusive, autoDelete: queueAutoDelete, arguments: new Dictionary<string, object>());
        }

    }
}
