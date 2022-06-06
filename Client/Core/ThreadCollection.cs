using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IotSystem.Core
{
    public class ThreadCollection : List<Thread>
    {
        public delegate void DelegateThread(CancellationToken cancellation);

        public ThreadCollection()
        {

        }

        public void AddThread(DelegateThread delegateThread, CancellationToken cancellation)
        {
            var thread = new Thread(() => delegateThread(cancellation));            
            this.Add(thread);
        }

        public void AddThread(DelegateThread delegateThread, CancellationToken cancellation, string threadName = null)
        {
            var thread = new Thread(() => delegateThread(cancellation));
            thread.Name = threadName;
            this.Add(thread);
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

                if (thread.ThreadState == ThreadState.Stopped)
                {
                    thread.IsBackground = false;
                    thread.Start();
                    count++;
                }
            }
        }

        public int RunningCount => this.FindAll(r => r.ThreadState == ThreadState.Running).Count;

        public void Join()
        {
            foreach (Thread thread in this)
            {
                thread.Join();
            }
        }
    }
}
