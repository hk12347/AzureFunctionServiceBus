using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctionServiceBus
{
    /// <summary>
    /// Read messages from Azure Service Bus (trigger)
    /// </summary>
    public class ReadMessages
    {
        [FunctionName("ReadMessages")]
        public void Run([ServiceBusTrigger("productqueue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage[] message, ILogger log)
        {
            for (int x = 0; x < message.Count(); x++)
            {
                log.LogInformation($"ServiceBus queue trigger function processed message. Id: {message[x].SequenceNumber}");

                var json = Encoding.UTF8.GetString(message[x].Body);
                ProductQueueMessage? product = JsonSerializer.Deserialize<ProductQueueMessage>(json);

                log.LogWarning($"Name: {product.Name}");
                log.LogWarning($"Description: {product.Description}");
                log.LogWarning($"Price: {product.Price}");
            }
        }
    }
}
