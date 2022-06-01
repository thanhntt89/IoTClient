/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonMessageTimeQueue.cs
* Created date:2022/6/1 2:49 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Collections.Concurrent;

namespace IotClient.Queues
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
