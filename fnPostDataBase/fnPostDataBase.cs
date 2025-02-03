using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dataBase.Function
{
    public class fnPostDataBase
    {
        private readonly ILogger<fnPostDataBase> _logger;

        public fnPostDataBase(ILogger<fnPostDataBase> logger)
        {
            _logger = logger;
        }

        [Function("movies")]
        [CosmosDBOutput("%DataBasename%", "movies", Connection = "cosmoDBConnection", CreateIfNotExists = true, PartitionKey ="id")]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            MovieRequest movie = null;

            var content = await new StreamReader(req.Body).ReadToEndAsync(); 

            try
            {
                movie = JsonConvert.DeserializeObject<MovieRequest>(content);
                
            }
            catch (System.Exception)
            {
                
                return new BadRequestObjectResult("Erro ao deserializar objeto");
            }

            return JsonConvert.SerializeObject(movie);
        }
    }
}
