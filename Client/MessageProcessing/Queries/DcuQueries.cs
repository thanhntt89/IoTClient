/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: DcuQueries.cs
* Created date:2022/6/7 12:10 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/

namespace IotSystem.MessageProcessing.Queries
{
    public class DcuQueries
    {
        internal static string InsertDcuQuery(string dcuCode)
        {
            string query = string.Empty;
            query = string.Format("exec [dbo].[IOT_SYSTEM_INSERT_DCU]{0}", dcuCode);
            return query;
        }
    }
}
