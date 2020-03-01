using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions
{
    class TopicsAndSubscriptionsConsole
    {
        // Enter a Service Bus connection string.
        private static string ServiceBusConnectionString = "";
        private static string OrdersTopicPath = "Orders";


        static async Task Main(string[] args)
        {
            Console.WriteLine("Topics and Subscriptions Console");

            PrompAndWait("Press enter to create topic and subscriptions...");
            await CreateTopicsAndSubscriptions();

            PrompAndWait("Press enter to send order messages...");
            await SendOrderMessages();

            PrompAndWait("Press enter to receive order messages...");
            await ReceiveOrdersFromAllSubscriptions();

            PrompAndWait("Topics and Subscriptions Console Complete");
        }



        static async Task CreateTopicsAndSubscriptions()
        {
            //use ManagementClient
            var manager = new Manager(ServiceBusConnectionString);
           
            await manager.CreateTopic(OrdersTopicPath);

            //Subscription without a filter, rule 1=1 gets created , can receive all messages.
            await manager.CreateSubscription(OrdersTopicPath, "AllOrders");

            //SubscriptionWithSqlFilter, create SubscriptionDescription and ruledescription 
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "UsaOrders", "region = 'USA'");
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "EuOrders", "region = 'EU'");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "LargeOrders", "items > 30");
            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "HighValueOrders", "value > 500");

            await manager.CreateSubscriptionWithSqlFilter(OrdersTopicPath, "LoyaltyCardOrders", "loyalty = true AND region = 'USA'");

            //with correlationID
            await manager.CreateSubscriptionWithCorrelationFilter(OrdersTopicPath, "UkOrders", "UK");

        }

        static async Task SendOrderMessages()
        {
            var orders = CreateTestOrders();

            //use TopicClient(connectstring and name of topic)
            var sender = new TopicSender(ServiceBusConnectionString, OrdersTopicPath);

            foreach (var order in orders)
            {
                await sender.SendOrderMessage(order);
            }

            // Always Close...
            await sender.Close();
        }

        static async Task ReceiveOrdersFromAllSubscriptions()
        {
            var manager = new Manager(ServiceBusConnectionString);

            // Get the subscriptions from our topic...
            var subscriptionDescriptions = await manager.GetSubscriptionsForTopic(OrdersTopicPath);

            // Loop through the subscriptions and process the order messages.
            foreach (var subscriptionDescription in subscriptionDescriptions)
            {
                var receiver = new SubscriptionReceiver(ServiceBusConnectionString, OrdersTopicPath, subscriptionDescription.SubscriptionName);
                receiver.RegisterMessageHandler();
                PrompAndWait($"Receiving orders from { subscriptionDescription.SubscriptionName }, press enter when complete..");
                await receiver.Close();
            }
        }




        static List<Order> CreateTestOrders()
        {
            var orders = new List<Order>();

            orders.Add(new Order()
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyltyCard = true
            });
            orders.Add(new Order()
            {
                Name = "Large Order",
                Value = 49.99,
                Region = "USA",
                Items = 50,
                HasLoyltyCard = false
            });
            orders.Add(new Order()
            {
                Name = "High Value",
                Value = 749.45,
                Region = "USA",
                Items = 45,
                HasLoyltyCard = false
            });
            orders.Add(new Order()
            {
                Name = "Loyal Europe",
                Value = 49.45,
                Region = "EU",
                Items = 3,
                HasLoyltyCard = true
            });
            orders.Add(new Order()
            {
                Name = "UK Order",
                Value = 49.45,
                Region = "UK",
                Items = 3,
                HasLoyltyCard = false
            });

            // Feel free to add more orders if you like.


            return orders;
        }

        static void PrompAndWait(string text)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ForegroundColor = temp;
            Console.ReadLine();
        }


    }
}
