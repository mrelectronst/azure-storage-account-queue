using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLib.Infrastructure
{
    public interface IQueueStorage
    {
        public Task SendMessageAsync(string message);

        public Task<QueueMessage> RetrieveNextMessageAsync();

        public Task DeleteMessage(string messageId, string popReceipt);
    }
}
