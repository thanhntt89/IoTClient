using System.Collections.Generic;
using System.Threading;

namespace IotClient.ThreadManagement
{
    public class ThreadCollection : List<Thread>
    {
        public delegate void DelegateThread(CancellationToken cancellation);

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
                if (thread.ThreadState == ThreadState.Running)
                    continue;
                thread.IsBackground = false;
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
