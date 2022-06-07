/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: BusinessBase.cs
* Created date:2022/6/7 12:05 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotSystem.MessageProcessing
{
    public class BusinessBase
    {
        public string ConnectionString { get; set; }
        public BusinessBase()
        {
            ConnectionString = SqlHelpers.ConnectionString;
        }

        protected void LogError(Exception ex)
        {

        }
    }
}
