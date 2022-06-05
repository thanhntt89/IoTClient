using IotSystem.Core;
using IotSystem.Core.Connection;
using IotSystem.Core.ThreadManagement;
using IotSystem.MessageProcessing;
using IotSystem.Utils;
using System;

namespace IotSystem
{
    class Program
    {       
        static void Main(string[] args)
        {
            try
            {
                ClientSetting setting = FileUtil.Deserialize<ClientSetting>(FileUtil.GetTextFromFile(Constant.SettingPath));
                ShowMessage("Load Client Setting Success!!!");

                IClient client = new ClientBuilder()
               .AddDatabaseServer(setting.DATABASE_CONFIG.Server)
               .AddDatabaseName(setting.DATABASE_CONFIG.DatabaseName)
               .AddDbUserName(setting.DATABASE_CONFIG.UserName)
               .AddDbPassword(setting.DATABASE_CONFIG.Password)
               .AddDbPort(setting.DATABASE_CONFIG.Port)
               .AddDbConnectionTimeOut(setting.DATABASE_CONFIG.ConnectionTimeOut)
               .AddDbCommandTimeOut(setting.DATABASE_CONFIG.CommandTimeOut)
               .AddBroker(setting.DCU_CONFIG.Broker)
               .AddPort(setting.DCU_CONFIG.Port)
               .AddClientId(setting.DCU_CONFIG.ClientId)
               .AddWillTopic(setting.DCU_CONFIG.SubscriberTopic)
               .AddWillPublisherTopic(setting.DCU_CONFIG.PublisherTopic)
               .AddWillQosLevel(setting.DCU_CONFIG.QoS)
               .AddUserName(setting.DCU_CONFIG.UserName)
               .AddPassword(setting.DCU_CONFIG.Password)
               .AddTypeData(setting.DCU_CONFIG.TypeData)
               .AddTypeTime(setting.DCU_CONFIG.TypeTime)
               .AddTimeCheckConnect(setting.DCU_CONFIG.TimeCheckConnect)
               .AddIDatabaseConnectionThread(SingletonDatabaseConnection.Instance)
               .AddIDecodeDataThread(new DecodeMessageDataThread())
               .AddIInsertDataThread(SingletonInsertDataThread.Instance)
               .AddIPublishMessageThread(SingletonPublishThread.Instance)
               .Build();

                client.ShowMessage(ShowMessage);
                               

                while (true)
                {
                    Console.WriteLine("\n-----------MENUS-----------");
                    Console.WriteLine("Select options following");
                    Console.WriteLine("START to Start client CONNECT to broker");
                    Console.WriteLine("STOP to Stop client DISCONNECT to broker");
                    Console.WriteLine("EXIT to Quit client: Stop all thread and exit!!");
                    Console.WriteLine("\n---------------------------");
                    Console.WriteLine("Press option:");
                    string press = Console.ReadLine().ToUpper();

                    switch (press)
                    {
                        case "START":
                            client.Start();
                            break;
                        case "STOP":
                            client.Stop(true);
                            break;
                        case "EXIT":
                            client.Exit();
                            //Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Wrong selection!!!");
                            break;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while init Client!!!/nError: {ex.Message}");
            }
        }

        private static void ShowMessage(string message)
        {            
            Console.WriteLine(message);            
        }
    }
}
