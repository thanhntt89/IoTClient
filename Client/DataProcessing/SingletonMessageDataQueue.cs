using System.Collections.Concurrent;

namespace IotClient.DataProcessing
{
    public class SingletonMessageDataQueue<T> : ConcurrentQueue<T>
    {
        private static SingletonMessageDataQueue<T> _instance;
        private static object synObject = new object();

        static SingletonMessageDataQueue() { }
        private SingletonMessageDataQueue() { }

        public static SingletonMessageDataQueue<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (synObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SingletonMessageDataQueue<T>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
