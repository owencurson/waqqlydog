using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class GetRegisteredPets
{
    private static readonly string connectionString = Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? string.Empty;
    private static readonly CosmosClient client = new CosmosClient(connectionString);

    [Function("GetRegisteredPets")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetRegisteredPets");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var database = client.GetDatabase("waqqlydog");
        var container = database.GetContainer("pets");

        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<Pet>(query);

        List<Pet> pets = new List<Pet>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            pets.AddRange(response.Resource);
        }

        var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await responseMessage.WriteAsJsonAsync(pets);
        return responseMessage;
    }

    public class Pet
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string? petName { get; set; }
        public string? petType { get; set; }
    }
}
