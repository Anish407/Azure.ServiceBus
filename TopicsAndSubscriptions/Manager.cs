using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class Manager
    {

        private ManagementClient m_ManagementClient;

        public Manager(string connectionString)
        {
            m_ManagementClient = new ManagementClient(connectionString);
        }


        public async Task<TopicDescription> CreateTopic(string topicPath)
        {
            Console.WriteLine($"Creating Topic { topicPath }");



            if (await m_ManagementClient.TopicExistsAsync(topicPath))
            {
                await m_ManagementClient.DeleteTopicAsync(topicPath);
            }

            return await m_ManagementClient.CreateTopicAsync(topicPath);
        }






        public async Task<SubscriptionDescription> CreateSubscription(string topicPath, string subscriptionName)
        {
            Console.WriteLine($"Creating Subscription { topicPath }/{ subscriptionName }");

            return await m_ManagementClient.CreateSubscriptionAsync(topicPath, subscriptionName);
        }

        public async Task<SubscriptionDescription> CreateSubscriptionWithSqlFilter(string topicPath, string subscriptionName, string sqlExpression)
        {
            Console.WriteLine($"Creating Subscription with SQL Filter{ topicPath }/{ subscriptionName } ({ sqlExpression })");

            var subscriptionDescription = new SubscriptionDescription(topicPath, subscriptionName);

            var ruleDescription = new RuleDescription("Default", new SqlFilter(sqlExpression));

            return await m_ManagementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
        }

        public async Task<SubscriptionDescription> CreateSubscriptionWithCorrelationFilter(string topicPath, string subscriptionName, string correlationId)
        {
            Console.WriteLine($"Creating Subscription with Correlation Filter{ topicPath }/{ subscriptionName } ({ correlationId })");

            

            var subscriptionDescription = new SubscriptionDescription(topicPath, subscriptionName);

            var ruleDescription = new RuleDescription("Default", new CorrelationFilter(correlationId));

            return await m_ManagementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
        }



        public async Task<IList<SubscriptionDescription>> GetSubscriptionsForTopic(string topicPath)
        {


            return await m_ManagementClient.GetSubscriptionsAsync(topicPath);
        }

    }
}
