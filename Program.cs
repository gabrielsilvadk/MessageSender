using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace MessagePublisher
{
    public class Program
    {
        private const string ServiceBusConnectionString = "";

        private const string QueueName = "messagequeue";

        private const int NumberOfMessages = 3;

        static ServiceBusClient client = default;

        static ServiceBusSender sender = default;

        public static async Task Main(string[] args)
        {
            client = new ServiceBusClient(ServiceBusConnectionString);
            sender = client.CreateSender(QueueName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= NumberOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine("Messages sent");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
