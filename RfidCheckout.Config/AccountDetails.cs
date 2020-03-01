
namespace Session.COmmon
{
    public class AccountDetails
    {
        //ToDo: Enter a valid Serivce Bus connection string
        public static string ConnectionString = "";
        public static string QueueName = "rfidcheckout";

        public static string RequestQueueName = "requestqueue";
        public static string ResponseQueueName = "responsequeue";
    }
}


