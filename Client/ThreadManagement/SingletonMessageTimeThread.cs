using IotSystem.Queues;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing
{
    public class SingletonMessageTimeThread
    {
        public event DelegatePublishMessage eventPublishMessage;
        public event DelegateShowMessage eventShowMessage;

        private static SingletonMessageTimeThread intance;
        private static readonly object objLock = new object();

        public string Topic { get; set; }

        public void ThreadDecode(CancellationToken cancellation)
        {
            eventShowMessage?.Invoke("ThreadDecodeMessageTime: Started!!!");
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

            eventShowMessage?.Invoke("ThreadDecodeMessageTime: Stopped!!!");
        }

        private void MessageProcessing(MessageData message)
        {
            string dcuId = message.Topic.Split('/')[1];
            eventPublishMessage?.Invoke(dcuId, Constant.CURRENT_TIME);
        }

        static SingletonMessageTimeThread()
        {

        }

        private SingletonMessageTimeThread()
        {

        }

        public static SingletonMessageTimeThread Instance
        {
            get
            {
                if (intance == null)
                {
                    lock (objLock)
                    {
                        if (intance == null)
                        {
                            intance = new SingletonMessageTimeThread();
                        }
                    }
                }
                return intance;
            }
        }
    }
}
