using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public  class MessageBus : IMessageBus
    {
        private string ConnectionString = "Endpoint=sb://mangowebbykhateeb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=IckuzaI/mi+nIpMH4tFQW6TTD2Lng1doF+ASbNKGfqc=";
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(ConnectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage)){
                CorrelationId = Guid.NewGuid().ToString(),

            };
            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }

     }
    }

