using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ExternalServices.UploadServices
{
    public class BlobStorageUploadService : IUploadService
    {
        private readonly IConfiguration _configuration;

        public BlobStorageUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool CheckIfFileTypeIsPdf(string ContentType,string FileName)
        {
            var extension = Path.GetExtension(FileName).ToLowerInvariant();
            if (extension != ".pdf")
            {
                return false;
            }

            if (ContentType != "application/pdf")
            {
                return false;
            }

            return true;
        }

        public async Task<string> UploadSpecificTypeAsync(Stream stream,string fileName,string contentType)
        {
            Guid id = Guid.NewGuid();

            BlobClient blobClient = GetBlobClient(fileName+id.ToString());

            var blobHttpHeader = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeader
            });


            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            Guid id = Guid.NewGuid();

            BlobClient blobClient = GetBlobClient(fileName+id.ToString());

            await blobClient.UploadAsync(stream);

            return blobClient.Uri.ToString();
        }

        private BlobClient GetBlobClient(string fileName)
        {
            string connectionString = _configuration.GetSection("blobconnectionstring").Value!;
            string containerName = "taskalayze";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            return containerClient.GetBlobClient(fileName);
        }


    }
}
