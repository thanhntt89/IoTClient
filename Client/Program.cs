using IotSystem.Core;
using IotSystem.Core.Connection;
using IotSystem.Core.ThreadManagement;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing;
using IotSystem.MessageProcessing.DcuMessage;
using IotSystem.MessageProcessing.MessageStructure;
using IotSystem.MessageProcessing.MeterMessage;
using IotSystem.Utils;
using System;
using System.Threading.Tasks;

namespace IotSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var msg = MeterMessageTest.CreateRuntimeMessage();
            DecodeMessageDataThread decodeMessageData = new DecodeMessageDataThread(new MessageType()
            {
                TypeRunTime = "RunTime",
                TypeAlarm="Alarm"
            });
            decodeMessageData.ProcessingMessage(msg);
            // MessageBase msgTime = DcuPublishMessage.CreatePublishMessageTime("Setup/{0}/dcuid", "124");

            try
            {
                ClientSetting setting = FileUtil.Deserialize<ClientSetting>(FileUtil.GetTextFromFile(Constant.SettingPath));
                ShowMessage("Load Client Setting Success!!!");

                IClient client = new ClientBuilder()
               .AddBroker(setting.DCU_CONFIG.Broker)
               .AddPort(setting.DCU_CONFIG.Port)
               .AddClientId(setting.DCU_CONFIG.ClientId)
               .AddWillTopic(setting.DCU_CONFIG.SubscriberTopic)
               .AddWillQosLevel(setting.DCU_CONFIG.QoS)
               .AddIsClearSection(setting.DCU_CONFIG.IsClearSection)
               .AddUserName(setting.DCU_CONFIG.UserName)
               .AddPassword(setting.DCU_CONFIG.Password)
               .AddTypeData(setting.MESSAGE_TYPE.TypeRunTime)
               .AddTypeTime(setting.MESSAGE_TYPE.TypeTime)
               .AddTypeAlarm(setting.MESSAGE_TYPE.TypeAlarm)
               .AddTypeSetup(setting.MESSAGE_TYPE.TypeSetup)
               .AddTimeCheckConnect(setting.DCU_CONFIG.TimeCheckConnect)
               .AddIDatabaseConnectionThread(new DatabaseConnectionThread(new DatabaseConfig
               {
                   Server = setting.DATABASE_CONFIG.Server,
                   DatabaseName = setting.DATABASE_CONFIG.DatabaseName,
                   Password = setting.DATABASE_CONFIG.Password,
                   UserName = setting.DATABASE_CONFIG.UserName,
                   CommandTimeOut = setting.DATABASE_CONFIG.CommandTimeOut,
                   ConnectionTimeOut = setting.DATABASE_CONFIG.ConnectionTimeOut,
                   Port = setting.DATABASE_CONFIG.Port
               }))
               .AddIDecodeDataThread(new DecodeMessageDataThread(new MessageType()
               {
                   TypeRunTime = setting.MESSAGE_TYPE.TypeRunTime,
                   TypeAlarm = setting.MESSAGE_TYPE.TypeAlarm
               }))
               .AddIDatabaseProcessingThread(new DatabaseProcessingThread())
               .AddIPublishMessageThread(new PublishMessageThread(new PublishMessageTopic()
               {
                   MessageResponseTimeTopic = setting.DCU_CONFIG.PublishMessageTimeTopic,
                   MessageSetupDcuTopic = setting.DCU_CONFIG.PublishMessageSetupDcuTopic,
                   MessageTypeTime = setting.MESSAGE_TYPE.TypeTime,
                   MessageTypeSetup = setting.MESSAGE_TYPE.TypeSetup
               }))
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
                            var task = Task.Run(() => client.Start());
                            task.Wait();
                            break;
                        case "STOP":
                            client.Stop(true);
                            break;
                        case "EXIT":
                            client.Exit();
                            Environment.Exit(0);
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
