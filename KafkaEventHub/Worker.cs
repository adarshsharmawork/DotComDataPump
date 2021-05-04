using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaEventHub.Model;
using Newtonsoft.Json;

namespace KafkaEventHub
{
    class Worker
    {
        public static async Task Producer(string brokerList, string connStr, string topic, string cacertlocation)
        {
            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = brokerList,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = "$ConnectionString",
                    SaslPassword = connStr,
                    SslCaLocation = "",
                    //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
                };
                using (var producer = new ProducerBuilder<long, string>(config).SetKeySerializer(Serializers.Int64).SetValueSerializer(Serializers.Utf8).Build())
                {
                    Console.WriteLine("Sending 10 messages to topic: " + topic + ", broker(s): " + brokerList);

                    //Data Creation
                    CatalogAvailability availability = new CatalogAvailability
                    {
                        UW = "Yes"
                    };

                    List<string> CategoryList = new List<string>
                    {
                        "Cat1", "Cat2", "Cat3", "Cat4"
                    };

                    CatalogContractPrices contractPrices = new CatalogContractPrices
                    {

                        ContractA = "Yes",
                        ContractC = "C",
                        ContractCategoryA = "CatA"
                    };

                    CatalogVisibility visibility = new CatalogVisibility
                    {
                        ContractA = "A level Visibility",
                        ContractC = "C Visible",
                        Default = "true"
                    };

                    CatalogFields fields = new CatalogFields {
                        Availability = availability, Brand = "New", CategoryEn = CategoryList, CategoryFr = CategoryList,
                        ContractPrices = contractPrices,NetPrice = "20.2", ProductID = "99115", SaleRank = "First",
                        TitleEn = "Good Product", TitleFr = "French Product", Visibility = visibility
                    };

                    CatalogModel catalogModel = new CatalogModel
                    {
                        Fields = fields
                    };

                    var CatalogJsonString = JsonConvert.SerializeObject(catalogModel);

                    for (int x = 0; x < 10; x++)
                    {
                        var msg = string.Format("Sample Catalog #{0} sent at {1}", CatalogJsonString, DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.ffff"));
                        var deliveryReport = await producer.ProduceAsync(topic, new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = msg });
                        Console.WriteLine(string.Format("Message {0} sent (value: '{1}')", x, msg));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Exception Occurred - {0}", e.Message));
            }
        }

        public static void Consumer(string brokerList, string connStr, string consumergroup, string topic, string cacertlocation)
        {
            List<Task> tasks = new List<Task>();
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,                //this corresponds to the Consumer config `request.timeout.ms`
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connStr,
                SslCaLocation = "",
                GroupId = consumergroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = "1.0.0",        //Event Hubs for Kafka Ecosystems supports Kafka v1.0+, a fallback to an older API will fail
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };

            using (var consumer = new ConsumerBuilder<long, string>(config).SetKeyDeserializer(Deserializers.Int64).SetValueDeserializer(Deserializers.Utf8).Build())
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

                consumer.Subscribe(topic);

                Console.WriteLine("Consuming messages from topic: " + topic + ", broker(s): " + brokerList);

                while (true)
                {
                    try
                    {
                        var msg = consumer.Consume(cts.Token);
                        Console.WriteLine($"Received: '{msg.Message.Value}'");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
        }
    }
}
