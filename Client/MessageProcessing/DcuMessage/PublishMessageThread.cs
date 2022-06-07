using IotSystem.Core;
using IotSystem.Core.ThreadManagement;
using IotSystem.Queues;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing.DcuMessage
{
    public class PublishMessageTopic
    {
        public string MessageResponseTimeTopic { get; set; }
        public string MessageSetupDcuTopic { get; set; }
        public string MessageTypeTime { get; set; }
        public string MessageTypeSetup { get; set; }
    }

    public class PublishMessageThread : IPublishMessageThread
    {
        public event DelegatePublishMessage EventPublishMessage;
        public event DelegateShowMessage EventShowMessage;

        private PublishMessageTopic MessageTopic { get; set; }

        public PublishMessageThread(PublishMessageTopic messageTopic)
        {
            MessageTopic = messageTopic;
        }

        public void ThreadDecode(CancellationToken cancellation)
        {
            EventShowMessage?.Invoke("PublishMessageThread: Started!!!");
            MessageBase message = new MessageBase();
            while (!cancellation.IsCancellationRequested)
            {
                if (SingletonMessageTimeQueue<MessageBase>.Instance.Count > 0)
                {
                    if (SingletonMessageTimeQueue<MessageBase>.Instance.TryDequeue(out message) && message != null)
                    {
                        if (message.Topic.Contains(MessageTopic.MessageTypeTime))
                            PublishMessageTime(message);
                        else if (message.Topic.Contains(MessageTopic.MessageTypeSetup))
                            PublishMessageSetupDcu(message);
                    }
                    Thread.Sleep(10);
                    continue;
                }
                // Queue is empty wait 1s
                Thread.Sleep(1000);
            }

            EventShowMessage?.Invoke("PublishMessageThread: Stopped!!!");
        }

        private void PublishMessageSetupDcu(MessageBase message)
        {
            string dcuId = message.Topic.Split('/')[4];
            EventPublishMessage?.Invoke(string.Format(MessageTopic.MessageSetupDcuTopic, dcuId), Constant.CURRENT_TIME);
        }

        private void PublishMessageTime(MessageBase message)
        {
            string dcuId = message.Topic.Split('/')[4];
            EventPublishMessage?.Invoke(string.Format(MessageTopic.MessageResponseTimeTopic, dcuId), Constant.CURRENT_TIME);
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }

        public void PublishMessage(DelegatePublishMessage publishMessage)
        {
            EventPublishMessage += publishMessage;
        }

    }
}
