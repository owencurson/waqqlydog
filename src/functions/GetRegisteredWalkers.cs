using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class GetRegisteredWalkers
{
    private static readonly string connectionString = Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? string.Empty;
    private static readonly CosmosClient client = new CosmosClient(connectionString);

    [Function("GetRegisteredWalkers")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetRegisteredWalkers");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var database = client.GetDatabase("waqqlydog");
        var container = database.GetContainer("walkers");

        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<Walker>(query);

        List<Walker> walkers = new List<Walker>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            walkers.AddRange(response.Resource);
        }

        var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await responseMessage.WriteAsJsonAsync(walkers);
        return responseMessage;
    }

    public class Walker
    {
        public string id { get; set; }
        public string walkerName { get; set; }
        public string walkerPhone { get; set; }
    }
}
