using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System;

public static class HandleRegistration
{
    private static readonly string endpoint = Environment.GetEnvironmentVariable("COSMOS_DB_ENDPOINT");
    private static readonly string key = Environment.GetEnvironmentVariable("COSMOS_DB_KEY");
    private static readonly CosmosClient client = new CosmosClient(endpoint, key);

    [Function("HandleRegistration")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HandleRegistration");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string type = data?.type;

        var response = req.CreateResponse();

        if (string.IsNullOrEmpty(type))
        {
            response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            await response.WriteStringAsync("Please provide a type (pet or walker) in the request body.");
            return response;
        }

        try
        {
            dynamic responseMessage;
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

    private static async Task<dynamic> HandlePetRegistration(dynamic body)
    {
        var database = client.GetDatabase("waqqlydog");
        var container = database.GetContainer("pets");

        var newItem = new
        {
            id = Guid.NewGuid().ToString(),
            petName = (string)body.petName,
            petType = (string)body.petType
        };

        var response = await container.CreateItemAsync(newItem);
        return response.Resource;
    }

    private static async Task<dynamic> HandleWalkerRegistration(dynamic body)
    {
        var database = client.GetDatabase("waqqlydog");
        var container = database.GetContainer("walkers");

        var newItem = new
        {
            id = Guid.NewGuid().ToString(),
            walkerName = (string)body.walkerName,
            walkerPhone = (string)body.walkerPhone
        };

        var response = await container.CreateItemAsync(newItem);
        return response.Resource;
    }
}
