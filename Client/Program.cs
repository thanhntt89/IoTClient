using IotClient.Utils;
using System;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace IotClient
{
    class Program
    {
        static object objLock = new object();

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
               .Build();

                client.ShowMessage(ShowMessage);

                bool menu = true;

                while (menu)
                {
                    menu = ShowMenu(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while init Client!!!/nError: {ex.Message}");
            }
        }

        private static void ShowMessage(string message)
        {
            lock (objLock)
            {
                Console.WriteLine(message);
            }
        }

        private static bool ShowMenu(IClient client)
        {
            Console.WriteLine("\n-----------MENUS-----------");
            Console.WriteLine("Select options following");
            Console.WriteLine("START to Start client");
            Console.WriteLine("STOP to Stop client");
            Console.WriteLine("EXIT to Quit client");
            Console.WriteLine("\n---------------------------");

            string press = Console.ReadLine().ToUpper();

            switch (press)
            {
                case "START":
                    client.Start();
                    return true;
                case "STOP":
                    client.Stop();
                    return true;
                case "EXIT":
                    client.Stop();
                    Environment.Exit(0);
                    return false;
                default:
                    return true;
            }
        }
    }
}
