using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HikersBlog.Storage
{
    public class AzureBlobStorageHandler
    {
        private readonly string _connectionString;

        public AzureBlobStorageHandler(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("HikersBlogStoraage");
        }

        //public async Task Initialize()
        //{
        //    // Create a BlobServiceClient object which will be used to create a container client
        //    BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

        //    //Create a unique name for the container
        //    string containerName = "images";

        //    // Create the container and return a container client object
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        //    var blobClient = containerClient.GetBlobClient("20210623_201457.jpg");

        //}

        public async Task UploadBlob(string filename, Stream stream)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            string containerName = "images";
            //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //var result = await containerClient.UploadBlobAsync(filename, stream);

            //var blobClient = containerClient.GetBlobClient(filename);

            //var blobHttpHeader = new BlobHttpHeaders { ContentType = "image/jpeg" };

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/jpeg"
                }
            };

            var blockBlobClient = new Azure.Storage.Blobs.Specialized.BlockBlobClient(_connectionString, containerName, filename);
            var result = await blockBlobClient.UploadAsync(stream, options);

            //var uploadedBlob = await blobClient.UploadAsync(new BinaryData(byte[]), new BlobUploadOptions { HttpHeaders = blobHttpHeader });
        }

        public async void DeleteDeadBlobs(IEnumerable<string> usedFilenames)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            string containerName = "images";

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var deadBlobsList = new List<BlobItem>();

            var blobPages = containerClient.GetBlobsAsync().AsPages(default, 1000);
            await foreach (var page in blobPages)
            {
                foreach (var blob in page.Values)
                {
                    var blobFilename = blob.Name.Contains("/") ? blob.Name.Split('/').Last() : blob.Name;
                    if (!usedFilenames.Contains(blobFilename))
                    {
                        deadBlobsList.Add(blob);
                    }
                }
            }

            foreach (var blob in deadBlobsList)
            {
                await containerClient.DeleteBlobAsync(blob.Name);
            }
        }
    }
}
