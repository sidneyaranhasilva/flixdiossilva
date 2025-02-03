using filter.Function;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace movies.Function
{
    public class movies
    {
        private readonly ILogger<movies> _logger;
         private readonly CosmosClient _cosmosClient;

        public movies(ILogger<movies> logger,  CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("movies")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get" )] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var container = _cosmosClient.GetContainer("DioFlixDB", "movies");
            var id = req.Query["id"];
            var query = $"SELECT * FROM c ";
            var queryDefinition = new QueryDefinition(query);
            var result = container.GetItemQueryIterator<MovieResult>(queryDefinition);
            var results = new List<MovieResult>();
            while(result.HasMoreResults)
            {
                foreach (var item in await result.ReadNextAsync())
                {
                    results.Add(item);
                }
            }

            var respmen = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await respmen.WriteAsJsonAsync(results);



            return respmen;
        }
    }
}
