using IotClient.Utils;
using System;

namespace IotClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ClientSetting setting = Util.Deserialize<ClientSetting>(Util.GetTextFromFile(Contants.SettingPath));
                ShowMessage("Load Client Setting Success!!!");

                IClient client = new ClientBuilder()
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
               .Build();

                client.ShowMessage(ShowMessage);
                do
                {
                    ShowMenu(client);
                } while (true);
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

        private static void ShowMenu(IClient client)
        {
            Console.WriteLine("======CLIENT MENU========");
            Console.WriteLine("Select options following");
            Console.WriteLine("START to Start client");
            Console.WriteLine("STOP to Stop client");
            Console.WriteLine("EXIT to Quit client");
            Console.WriteLine("=========================");

            string press = Console.ReadLine().ToUpper();

            switch (press)
            {
                case "START":
                    client.Start();
                    ShowMessage("Started Client Success!!!");
                    break;
                case "STOP":
                    client.Stop();
                    ShowMessage("Stop Client Success!!!");
                    break;
                case "EXIT":
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }
    }
}
