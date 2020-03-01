using Microsoft.Azure.ServiceBus;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ReqRes.Client
{
    class Program
    {
        // Create request and response queue clients
        static QueueClient RequestQueueClient =
            new QueueClient("ConnectionString", "QueueName");

        static QueueClient ResponseQueueClient =
            new QueueClient("ConnectionString", "QueueName");


        static async Task Main(string[] args)
        {
            Console.WriteLine("Client Console");

            while (true)
            {
                Console.WriteLine("Enter text:");
                string text = Console.ReadLine();

                // Create a session identifier for the response message
                
                string responseSessionId = Guid.NewGuid().ToString();

                // Create a message using text as the body.
                var requestMessage = new Message(Encoding.UTF8.GetBytes(text));

                // Set the appropriate message properties.
                //Send the first message to the first queue with the 
                // responseID set
                requestMessage.ReplyToSessionId = responseSessionId;

                var stopwatch = Stopwatch.StartNew();

                // Send the message on the request queue.
                await RequestQueueClient.SendAsync(requestMessage);

                // Create a session client
                // wait for the server to responsd with the same SessionId
                // by creating a session client
                var sessionClient =
                    new SessionClient("ConnectionString", "QueueName");

                // Accept a message session
                var messageSession = await sessionClient.AcceptMessageSessionAsync(responseSessionId);

                // Receive the response message.
                var responseMessage = await messageSession.ReceiveAsync();
                stopwatch.Stop();

                // Close the session, we got the message.
                await sessionClient.CloseAsync();

                // Deserialise the message body to echoText.
                string echoText = Encoding.UTF8.GetString(responseMessage.Body);

                Console.WriteLine(echoText);
                Console.WriteLine("Time: {0} ms.", stopwatch.ElapsedMilliseconds);
                Console.WriteLine();


            }
        }
    }
}
