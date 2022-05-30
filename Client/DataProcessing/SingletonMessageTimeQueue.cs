using System.Collections.Concurrent;

namespace IotClient.DataProcessing
{
    public class SingletonMessageTimeQueue<T>: ConcurrentQueue<T>
    {
        private static SingletonMessageTimeQueue<T> _instance;
        private static object synObject = new object();

        static SingletonMessageTimeQueue() { }
        private SingletonMessageTimeQueue() { }

        public static SingletonMessageTimeQueue<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (synObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SingletonMessageTimeQueue<T>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
