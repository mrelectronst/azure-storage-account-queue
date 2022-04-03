using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureStorageLib.Infrastructure;
using NoSqlDomain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageLib.Services
{
    public class QueueStorageRepository: IQueueStorage
    {
        private readonly QueueClient _queueClient;

        public QueueStorageRepository(string queueName)
        {
            _queueClient = new QueueClient(ConnectionStrings.AzStorageConnectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task DeleteMessage(string messageId, string popReceipt)
        {
            await _queueClient.DeleteMessageAsync(messageId, popReceipt);
        }

        public async  Task<QueueMessage> RetrieveNextMessageAsync()
        {
            QueueProperties queueProperties = await _queueClient.GetPropertiesAsync();

            if (queueProperties.ApproximateMessagesCount > 0)
            {
                QueueMessage[] queueMessages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(1));

                if (queueMessages.Any())
                {
                    return queueMessages[0];
                }
            }

            return null;
        }

        public async Task SendMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }
    }
}
