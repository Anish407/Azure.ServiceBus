﻿using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Session.COmmon;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReqRes.Server
{
    class Program
    {
        static QueueClient RequestQueueClient =
            new QueueClient("ConnectionString", "QueueName");

        static QueueClient ResponseQueueClient =
            new QueueClient("ConnectionString", "QueueName");

        static async Task Main(string[] args)
        {
            Console.WriteLine("Server Console");


            // Create a new management client
            var managementClient = new ManagementClient(AccountDetails.ConnectionString);

            Console.Write("Creatng queues...");

            // Delete any existing queues
            if (await managementClient.QueueExistsAsync(AccountDetails.RequestQueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.RequestQueueName);
            }

            if (await managementClient.QueueExistsAsync(AccountDetails.ResponseQueueName))
            {
                await managementClient.DeleteQueueAsync(AccountDetails.ResponseQueueName);
            }

            // Create Request Queue
            //We receive messages sent by the sender with the ReplyToSessionId 
            await managementClient.CreateQueueAsync(AccountDetails.RequestQueueName);

            // Create Response With Sessions 
            QueueDescription responseQueueDescription =
                new QueueDescription(AccountDetails.ResponseQueueName)
                {
                    //We create a session and set the session id received from the 
                    // ReplyToSessionId 
                    RequiresSession = true
                };

            await managementClient.CreateQueueAsync(responseQueueDescription);

            Console.WriteLine("Done!");

            RequestQueueClient.RegisterMessageHandler
                (ProcessRequestMessage, new MessageHandlerOptions(ProcessMessageException));
            Console.WriteLine("Processing, hit Enter to exit.");
            Console.ReadLine();

            // Close the queue clients...
            await RequestQueueClient.CloseAsync();
            await ResponseQueueClient.CloseAsync();
        }



        private static async Task ProcessRequestMessage(Message requestMessage, CancellationToken arg2)
        {
            // Deserialize the message body into text.
            string text = Encoding.UTF8.GetString(requestMessage.Body);
            Console.WriteLine("Received: " + text);

            Thread.Sleep(DateTime.Now.Millisecond * 20);

            string echoText = "Echo: " + text;

            // Create a response message using echoText as the message body.
            var responseMessage = new Message(Encoding.UTF8.GetBytes(echoText));

            // Set the session id
            responseMessage.SessionId = requestMessage.ReplyToSessionId;

            // Send the response message. The Client is wating with a session open for this 
            // session id
            await ResponseQueueClient.SendAsync(responseMessage);
            Console.WriteLine("Sent: " + echoText);
        }

        private static  Task ProcessMessageException(ExceptionReceivedEventArgs arg)
        {
            throw arg.Exception;
        }
    }
}
