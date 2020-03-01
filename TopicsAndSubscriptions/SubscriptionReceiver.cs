using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class SubscriptionReceiver
    {
        private SubscriptionClient m_SubscriptionClient;

        public SubscriptionReceiver(string connectionString, string topicPath, string subscriptionName)
        {
            m_SubscriptionClient = new SubscriptionClient(connectionString, topicPath, subscriptionName);
        }



        public void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            m_SubscriptionClient.RegisterMessageHandler(ProcessOrderMessageMessageAsync, messageHandlerOptions);
        }



        private async Task ProcessOrderMessageMessageAsync(Message message, CancellationToken token)
        {
            // Process the order message
            var orderJson = Encoding.UTF8.GetString(message.Body);
            var order = JsonConvert.DeserializeObject<Order>(orderJson);

            Console.WriteLine($"{ order.ToString() }");

            // Complete the message
            await m_SubscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }

        public async Task Close()
        {
            await m_SubscriptionClient.CloseAsync();
        }
    }
}
