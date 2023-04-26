using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;

namespace AzureFunctionServiceBus
{
    public static class SendMessage
    {
        /// <summary>
        /// Azure Function -> Send Message (Product) to Azure Service Bus
        /// </summary>
        [FunctionName("SendMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [ServiceBus("productqueue", Connection = "ServiceBusConnection", EntityType = ServiceBusEntityType.Queue)] IAsyncCollector<ProductQueueMessage> serviceBusCollector,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Read values from url (GET)
            string name = req.Query["name"];
            string description = req.Query["description"];
            string price = req.Query["price"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //Read values from body (POST)
            name = name ?? data?.name;
            description = description ?? data?.description;
            price = price ?? data?.price;

            await serviceBusCollector.AddAsync(new ProductQueueMessage
            {
                Name = name,
                Description = description,
                Price = price
            });

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}