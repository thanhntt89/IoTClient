using IotSystem.Core;
using IotSystem.Core.Queues;
using IotSystem.Core.ThreadManagement;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class MeterDecodeMessageThread : IDecodeDataThread
    {
        public event DelegateShowMessage EventShowMessage;
        private MessageType messageType;

        public MeterDecodeMessageThread(MessageType type)
        {
            messageType = type;
        }

        /// <summary>
        /// Set time to process message
        /// </summary>
        private int TIME_PROCESSING_MESSAGE = 200;

        private int TimeProcessMessage(int countdata)
        {
            TIME_PROCESSING_MESSAGE = 100;

            if (countdata > 5000 && countdata < 10000)
            {
                TIME_PROCESSING_MESSAGE = 50;
            }
            else if (countdata <= 100000 && countdata > 50000)
            {
                TIME_PROCESSING_MESSAGE = 10;
            }
            else if (countdata >= 200000)
            {
                TIME_PROCESSING_MESSAGE = 5;
            }

            return TIME_PROCESSING_MESSAGE;
        }

        public void ThreadDecode(CancellationToken cancellation)
        {
            MessageBase message = new MessageBase();
            int countData = 0;
            Thread currentThread = Thread.CurrentThread;
            EventShowMessage?.Invoke($"ThreadDecode-{currentThread.Name}:Started!!!");
            FactoryMeterMessageProcessing factoryMeterMessage = new FactoryMeterMessageProcessing(messageType);
            while (true)
            {
                countData = SingletonMessageDataQueue<MessageBase>.Instance.Count;
                if (cancellation.IsCancellationRequested && countData == 0)
                {
                    EventShowMessage?.Invoke($"ThreadDecode-{currentThread.Name}:Stopped!!!");
                    break;
                }

                //Get data from messagequeue
                if (SingletonMessageDataQueue<MessageBase>.Instance.TryDequeue(out message) && message != null)
                {
                    int code = factoryMeterMessage.ProcessingMessage(message).GetHashCode();
                    EventShowMessage?.Invoke($"ThreadDecode-{currentThread.Name}-ProcessingMessage-HashCode:{code}");

                    Thread.Sleep(TimeProcessMessage(countData));
                    continue;
                }
                //Sleep thread 10sec if queue has no data
                Thread.Sleep(10000);
            }
        }

        public void ThreadDecodeByTraffic(CancellationToken cancellation)
        {
            MessageBase message = new MessageBase();
            Thread currentThread = Thread.CurrentThread;

            EventShowMessage?.Invoke($"ThreadDecodeByTraffic-{currentThread.Name}:Started!!!");
            int countData = 0;
            FactoryMeterMessageProcessing factoryMeterMessage = new FactoryMeterMessageProcessing(messageType);
            while (true)
            {
                countData = SingletonMessageDataQueue<MessageBase>.Instance.Count;
                if (cancellation.IsCancellationRequested || countData == 0)
                {
                    EventShowMessage?.Invoke($"ThreadDecodeByTraffic-{currentThread.Name}:Stopped!!!");
                    break;
                }

                //Get data from messagequeue
                if (SingletonMessageDataQueue<MessageBase>.Instance.TryDequeue(out message) && message != null)
                {
                    int code = factoryMeterMessage.ProcessingMessage(message).GetHashCode();
                    EventShowMessage?.Invoke($"ThreadDecodeByTraffic-{currentThread.Name}-ProcessingMessage-HashCode:{code}");

                    Thread.Sleep(TimeProcessMessage(countData));
                    continue;
                }
                //Sleep thread 10sec if queue has no data
                Thread.Sleep(10000);
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }
    }
}
