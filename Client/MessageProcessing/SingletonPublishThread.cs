using IotSystem.Core;
using IotSystem.Core.ThreadManagement;
using IotSystem.Queues;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing
{
    public class SingletonPublishThread: IPublishMessageThread
    {
        public event DelegatePublishMessage EventPublishMessage;
        public event DelegateShowMessage EventShowMessage;

        private static IPublishMessageThread intance;
        private static readonly object objLock = new object();             

        public void ThreadDecode(CancellationToken cancellation)
        {
            EventShowMessage?.Invoke("ThreadDecodeMessageTime: Started!!!");
            MessageData message = new MessageData();
            while (!cancellation.IsCancellationRequested)
            {
                if (SingletonMessageTimeQueue<MessageData>.Instance.Count > 0)
                {
                    if (SingletonMessageTimeQueue<MessageData>.Instance.TryDequeue(out message) && message != null)
                    {
                        MessageProcessing(message);
                    }
                    Thread.Sleep(10);
                    continue;
                }
                // Queue is empty wait 1s
                Thread.Sleep(1000);
            }

            EventShowMessage?.Invoke("ThreadDecodeMessageTime: Stopped!!!");
        }

        private void MessageProcessing(MessageData message)
        {
            string dcuId = message.Topic.Split('/')[1];
            EventPublishMessage?.Invoke(dcuId, Constant.CURRENT_TIME);
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }

        public void PublishMessage(DelegatePublishMessage publishMessage)
        {
            EventPublishMessage += publishMessage;
        }

        static SingletonPublishThread()
        {

        }

        private SingletonPublishThread()
        {

        }

        public static IPublishMessageThread Instance
        {
            get
            {
                if (intance == null)
                {
                    lock (objLock)
                    {
                        if (intance == null)
                        {
                            intance = new SingletonPublishThread();
                        }
                    }
                }
                return intance;
            }
        }
    }
}
