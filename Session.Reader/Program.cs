using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using RfidCheckout.Config;
using RfidCheckout.Messages;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Session.Reader
{
    class Program
    {
            static async Task Main(string[] args)
        {
            Console.WriteLine("Checkout Console");

            // Create a new management client
            var managementClient = new ManagementClient(AccountDetails.ConnectionString);

            // Delete the queue if it exists.
            if (await managementClient.QueueExistsAsync(AccountDetails.QueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.QueueName);
            }

            // Create a description for the queue.
            QueueDescription rfidCheckoutQueueDescription =
                new QueueDescription(AccountDetails.QueueName)
                {
                    //require sessions
                    RequiresSession = true
                };

            // Create a queue based on the queue description.
            await managementClient.CreateQueueAsync(rfidCheckoutQueueDescription);

            // Create a session client
            var sessionClient = new SessionClient
                (AccountDetails.ConnectionString, AccountDetails.QueueName);

            //executes once for each session
            while (true)
            {
                Console.WriteLine("Accepting a message session...");
                Console.ForegroundColor = ConsoleColor.White;

                // Accept a message session                    
                var messageSession = await sessionClient.AcceptMessageSessionAsync();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Accepted session: { messageSession.SessionId }");
                Console.ForegroundColor = ConsoleColor.Yellow;

                int receivedCount = 0;
                double billTotal = 0.0;

                while (true)
                {
                    // Receive a message and set timespan.. if no message is received in 
                    //1 sec then we close the session.
                    var message = await messageSession.ReceiveAsync(TimeSpan.FromSeconds(1));

                    if (message == null)
                    {
                        Console.WriteLine("Closing session...!");
                        await messageSession.CloseAsync();
                        break;
                    }
                    else
                    {
                        // Process the order message
                        var rfidJson = Encoding.UTF8.GetString(message.Body);
                        var rfidTag = JsonConvert.DeserializeObject<RfidTag>(rfidJson);
                        Console.WriteLine($"{ rfidTag.ToString() }");

                        receivedCount++;
                        billTotal += rfidTag.Price;

                        // Complete the message receive opperation
                        await messageSession.CompleteAsync(message.SystemProperties.LockToken);
                    }
                }
                // Bill the customer.
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine
                    ("Bill customer ${0} for {1} items.", billTotal, receivedCount);
            }
        }

    }
}
