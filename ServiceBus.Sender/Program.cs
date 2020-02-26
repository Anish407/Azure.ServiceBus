using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ServiceBus.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Sender
{
    class Program
    {
        const string connectionString = "Endpoint=sb://sampleanish407.servicebus.windows.net/;SharedAccessKeyName=samplepolicy;SharedAccessKey=DTY8Mu2QPfRzofBld7qw1l+/gjSj0prRLKT5Lib9Dp0=;";
        const string queueName = "samplebrokerqueue";
        static async Task Main(string[] args)
        {
            await SendMessageToQueue(connectionString, queueName);
            Console.ReadLine();
        }


        static async Task SendMessageToQueue(string connectionString, string queueName)
        {
            // create a new client, this is used to send and receive message to the queue async
            IQueueClient queueClient = new QueueClient(connectionString, queueName);

            for (int i = 0; i < 10; i++)
            {
                //Serialize to json String 
                Person person = new Person { Name=$"Anish {i}", StudentId=i };
                string json = JsonConvert.SerializeObject(person);

                //Convert data to byte array and Send message to queue
                await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(json)));
            }
            

            //Close the connection
            await queueClient.CloseAsync();

        }
    }
    
   
}
