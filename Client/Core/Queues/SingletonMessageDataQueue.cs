/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonMessageDataQueue.cs
* Created date:2022/6/1 2:49 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Collections.Concurrent;

namespace IotSystem.Core.Queues
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
