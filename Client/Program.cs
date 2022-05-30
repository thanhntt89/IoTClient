using IotClient.Utils;
using System;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace IotClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ClientSetting setting = FileUtil.Deserialize<ClientSetting>(FileUtil.GetTextFromFile(Contants.SettingPath));
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
             SingletonShowMessage.Instance.ShowMessage(message);
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

    [Synchronization]
    public class SingletonShowMessage
    {
        private static SingletonShowMessage _instance;
        private static object synObject = new object();


        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        private SingletonShowMessage()
        {

        }

        static SingletonShowMessage()
        {

        }

        public static SingletonShowMessage Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock (synObject)
                    {
                        _instance = new SingletonShowMessage();
                    }
                }
                return _instance;
            }
        }
    }
}
