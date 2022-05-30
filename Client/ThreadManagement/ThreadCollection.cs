using System.Collections.Generic;
using System.Threading;
using static IotClient.ClientEvent;

namespace IotClient.ThreadManagement
{
    public class ThreadCollection : List<Thread>
    {
        public ThreadCollection()
        {

        }

        public void AddThread(DelegateThread delegateThread, CancellationToken cancellation)
        {
            this.Add(new Thread(() => delegateThread(cancellation)));
        }

        public void StartThread()
        {
            foreach (Thread thread in this)
            {
                thread.Start();
            }
        }

        public void StopThread()
        {
            foreach (Thread thread in this)
            {
                thread.Join();
            }
        }
    }
}
