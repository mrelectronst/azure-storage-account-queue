﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using NoSqlDomain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageLib
{
    public class BlobStorageRepository : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageRepository()
        {
            _blobServiceClient = new BlobServiceClient(ConnectionStrings.AzStorageConnectionString);
        }
        public string BlobUrl => "https://exampleazurestorageacc.blob.core.windows.net";

        public async Task DeleteAsync(string fileName, EnumContainerName enumContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(enumContainerName.ToString());

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string fileName, EnumContainerName enumContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(enumContainerName.ToString());

            var blobClient = containerClient.GetBlobClient(fileName);

            var info = await blobClient.DownloadAsync();

            return info.Value.Content;             
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            List<string> logs = new List<string>();
            var containerClient = _blobServiceClient.GetBlobContainerClient(EnumContainerName.azurecontainerlogs.ToString());

            await containerClient.CreateIfNotExistsAsync();

            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);

            await appendBlobClient.CreateIfNotExistsAsync();

            var info = await appendBlobClient.DownloadAsync();

            using (StreamReader sr = new StreamReader(info.Value.Content))
            {
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    logs.Add(line);
                }
            }

            return logs;
        }

        public List<string> GetNames(EnumContainerName enumContainerName)
        {
            List<string> blobNames = new List<string>();

            var containerClient = _blobServiceClient.GetBlobContainerClient(enumContainerName.ToString());

            var blobs = containerClient.GetBlobs();

            blobs.ToList().ForEach(x =>
            {
                blobNames.Add(x.GetType().ToString());
                blobNames.Add(x.Name);
            });

            return blobNames;
        }

        public async Task SetLogAsync(string text, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(EnumContainerName.azurecontainerlogs.ToString());

            var appendBlobClient = containerClient.GetAppendBlobClient(fileName);

            await appendBlobClient.CreateIfNotExistsAsync();

            using(MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write($"{DateTime.Now}:{text}\n");
                    sw.Flush();
                    ms.Position = 0;

                    await appendBlobClient.AppendBlockAsync(ms);
                }
            }
        }

        public async Task UploadAsync(Stream fileStream, string fileName, EnumContainerName enumContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(enumContainerName.ToString());

            await containerClient.CreateIfNotExistsAsync();

            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream);
        }
    }
}
