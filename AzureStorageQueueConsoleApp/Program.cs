using AzureStorageLib.Services;
using NoSqlDomain;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageQueueConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionStrings.AzStorageConnectionString = ""; //Fill it with your connection string

            QueueStorageRepository queueStorageRepository = new QueueStorageRepository("examplequeue");

            //SendQueueMessage(queueStorageRepository, "queue1");

            //Console.WriteLine(RetrieveMessage(queueStorageRepository));

            //DeleteMessage(queueStorageRepository);

        }

        static void SendQueueMessage(QueueStorageRepository _queueStorageRepository, string messageText)
        {
            string base64Message = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageText));

            _queueStorageRepository.SendMessageAsync(base64Message).Wait();

            Console.WriteLine("Saved Message");
        }

        static string RetrieveMessage(QueueStorageRepository _queueStorageRepository)
        {
            var message = _queueStorageRepository.RetrieveNextMessageAsync().Result;

            string text = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));

            return "Message: " + text;
        }

        static async Task DeleteMessage(QueueStorageRepository _queueStorageRepository)
        {
            var message = _queueStorageRepository.RetrieveNextMessageAsync().Result;

            await _queueStorageRepository.DeleteMessage(message.MessageId, message.PopReceipt);

            Console.WriteLine("Delete Message");
        }
    }
}
