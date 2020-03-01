using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class TopicSender
    {
        private TopicClient m_TopicClient;

        public TopicSender(string connectionString, string topicPath)
        {
            m_TopicClient = new TopicClient(connectionString, topicPath);

        }

        public async Task SendOrderMessage(Order order)
        {

            Console.WriteLine($"{ order.ToString() }");

            // Serialize the order to JSON
            var orderJson = JsonConvert.SerializeObject(order);

            // Create a message containing the serialized order Json
            var message = new Message(Encoding.UTF8.GetBytes(orderJson));

            // Promote properties...       
            message.UserProperties.Add("region", order.Region);           
            message.UserProperties.Add("items", order.Items);
            message.UserProperties.Add("value", order.Value);            
            message.UserProperties.Add("loyalty", order.HasLoyltyCard);

            // Set the correlation Id
            message.CorrelationId = order.Region;

            // Send the message
            await m_TopicClient.SendAsync(message);
        }

        public async Task Close()
        {
            await m_TopicClient.CloseAsync();
        }




    }
}
