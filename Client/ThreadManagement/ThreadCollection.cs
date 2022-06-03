using System.Collections.Generic;
using System.Threading;

namespace IotSystem.ThreadManagement
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
                if (thread.ThreadState == ThreadState.Unstarted)
                {
                    thread.IsBackground = false;
                    thread.Start();
                }
            }
        }

        public void StartThread(int threadRunningNumber)
        {
            if (this.RunningCount > 0 && threadRunningNumber > this.RunningCount)
                return;

            int count = 0;
            foreach (Thread thread in this)
            {
                if (count >= threadRunningNumber)
                    break;

                if (thread.ThreadState == ThreadState.Unstarted)
                {
                    thread.IsBackground = false;
                    thread.Start();
                    count++;
                }
            }
        }

        public int RunningCount => this.FindAll(r => r.ThreadState == ThreadState.Running).Count;

        public void KeepOneThread()
        {
            if (this.RunningCount == 1)
                return;
            int count = 0;

            foreach (Thread thread in this)
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    if (count > 0)
                    {                        
                        thread.Abort();
                    }
                    count++;
                }
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
