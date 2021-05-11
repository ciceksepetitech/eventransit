using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTransit.Core.Abstractions.Common;
using EventTransit.Core.Abstractions.Data;
using EventTransit.Core.Entities;
using EventTransit.Messaging.RabbitMq.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventTransit.Messaging.RabbitMq
{
    public class EventConsumer : IEventConsumer
    {
        private readonly IModel _channel;
        private readonly List<string> _queues;
        
        public EventConsumer(IRabbitMqConnectionFactory connection, IMongoRepository<Events> eventsRepository)
        {
            _channel = connection.ConsumerConnection.CreateModel();
            
            var events = eventsRepository.GetEvents().Result;
            
            // TODO Fix fetch queue name
            _queues = events.Select(x => x.Services.Select(y => y.Name).First()).ToList();
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, mq) =>
            {
                try
                {
                    var messageBody = mq.Body;
                    var message = Encoding.UTF8.GetString(messageBody.ToArray());
                    
                    // TODO HTTP Call
                    Console.WriteLine($"From Queue: {message}");
                
                    _channel.BasicAck(mq.DeliveryTag, false);
                    
                    // TODO Log success info to RabbitMq
                }
                catch (Exception e)
                {
                    _channel.BasicNack(mq.DeliveryTag, false, false);
                    
                    // TODO Log Exceptions to RabbitMq
                }
            };
            
            foreach (var queue in _queues)
            {
                // TODO Refactor here
                _channel.QueueBind(queue, "order_created", routingKey: "order_created");

                _channel.BasicConsume(queue,
                    autoAck: false,
                    consumer: consumer);
            }
        }
    }
}