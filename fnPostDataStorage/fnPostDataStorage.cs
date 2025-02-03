using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class fnPostDataStorage
    {
        private readonly ILogger<fnPostDataStorage> _logger;

        public fnPostDataStorage(ILogger<fnPostDataStorage> logger)
        {
            _logger = logger;
        }

        [Function("DataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processando a imagem no storage");
            
            
                if(!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
                {
                    return new BadRequestObjectResult("O cabeçalho  file tipe é obrigatorio");
                }

                var fileType = fileTypeHeader.ToString();
                var form = await req.ReadFormAsync();
                var file = form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("O arquivo nao foi enviado");

                }

                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string containerName = fileType;
                BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);
                BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
                
                await blobContainerClient.CreateIfNotExistsAsync();
                await blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
                
                string blobName = file.FileName;
                var blob = blobContainerClient.GetBlobClient(blobName);

                using(var stream = file.OpenReadStream())
                {
                    await blob.UploadAsync(stream, true);
                }
                _logger.LogInformation("Enviado com acuesso");

                return new OkObjectResult(new
                {
                    Massage = $"Arquivo {file.FileName} armado com sucesso!",
                    BlobUri = blob.Uri
                });

           
            
            
        }
    }
}
