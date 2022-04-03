using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageLib
{
    public interface IBlobStorage
    {
        public string BlobUrl { get; }
        Task UploadAsync(Stream fileStream, string fileName, EnumContainerName enumContainerName);

        Task<Stream> DownloadAsync(string fileName, EnumContainerName enumContainerName);

        Task DeleteAsync(string fileName, EnumContainerName enumContainerName);

        Task SetLogAsync(string text, string fileName);

        Task<List<string>> GetLogAsync(string fileName);

        List<string> GetNames(EnumContainerName enumContainerName);
    }
}
