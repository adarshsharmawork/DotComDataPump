using System;

namespace KafkaEventHub
{
    class Program
    {
        private const string V = ".\\cacert.pem";

        public static void Main(string[] args)
        {
            string brokerList = "gntsearchmessaging.servicebus.windows.net:9093"; //adarsheventhub.servicebus.windows.net:9093
            string connectionString = "Endpoint=sb://gntsearchmessaging.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9grYzEo2TusIpguLcD/yJv7vHCfS8uIzmMGZB7CMTmA=";
                                      //Endpoint=sb://adarsheventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Nmp7UQTi+vkY0WYJcfE5PfFlYronWPy6v0QmYHXQ+Ro=
            string topic = "test";
            string caCertLocation = V;
            string consumerGroup = "TestGroup";

            Console.WriteLine("Initializing Producer");
            Worker.Producer(brokerList, connectionString, topic, caCertLocation).Wait();
            Console.WriteLine();
            Console.WriteLine("Initializing Consumer");
            Worker.Consumer(brokerList, connectionString, consumerGroup, topic, caCertLocation);
            Console.ReadKey();
        }
    }
}