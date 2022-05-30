using System.Collections.Concurrent;

namespace IotClient.DataProcessing
{
    public class SingletonMessageQueue<T> : ConcurrentQueue<T>
    {
        private static SingletonMessageQueue<T> _instance;
        private static object synObject = new object();

        static SingletonMessageQueue() { }
        private SingletonMessageQueue() { }

        public static SingletonMessageQueue<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (synObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SingletonMessageQueue<T>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
