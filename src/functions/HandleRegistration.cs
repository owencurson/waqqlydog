using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System;

public static class HandleRegistration
{
    private static readonly string endpoint = Environment.GetEnvironmentVariable("COSMOS_DB_ENDPOINT") ?? string.Empty;
    private static readonly string key = Environment.GetEnvironmentVariable("COSMOS_DB_KEY") ?? string.Empty;
    private static readonly CosmosClient client = new CosmosClient(endpoint, key);

    [Function("HandleRegistration")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HandleRegistration");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<RequestData>(requestBody);
        
        if (data == null)
        {
            var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request body.");
            return badResponse;
        }

        var type = data.Type;

        var response = req.CreateResponse();

        if (string.IsNullOrEmpty(type))
        {
            response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            await response.WriteStringAsync("Please provide a type (pet or walker) in the request body.");
            return response;
        }

        try
        {
            object responseMessage;
            if (type == "pet")
            {
                responseMessage = await HandlePetRegistration(data);
            }
            else if (type == "walker")
            {
                responseMessage = await HandleWalkerRegistration(data);
            }
            else
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid type provided. Must be 'pet' or 'walker'.");
                return response;
            }

            response.StatusCode = System.Net.HttpStatusCode.OK;
            await response.WriteAsJsonAsync(responseMessage);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing request.");
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            await response.WriteStringAsync($"Error: {ex.Message}");
            return response;
        }
    }

    private static async Task<object> HandlePetRegistration(RequestData body)
    {
        var database = client.GetDatabase("waqqlydog");
        var container = database.GetContainer("pets");

        var newItem = new
        {
            id = Guid.NewGuid().ToString(),
            petName = body.PetName,
            petType = body.PetType
        };

        var response = await container.CreateItemAsync(newItem);
        return response.Resource;
    }

    private static async Task<object> HandleWalkerRegistration(RequestData body)
    {
        var database = client.GetDatabase("waqqlydog");
        var container = database.GetContainer("walkers");

        var newItem = new
        {
            id = Guid.NewGuid().ToString(),
            walkerName = body.WalkerName,
            walkerPhone = body.WalkerPhone
        };

        var response = await container.CreateItemAsync(newItem);
        return response.Resource;
    }

    public class RequestData
    {
        public string? Type { get; set; }
        public string? PetName { get; set; }
        public string? PetType { get; set; }
        public string? WalkerName { get; set; }
        public string? WalkerPhone { get; set; }
    }
}
