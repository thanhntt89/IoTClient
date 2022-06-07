/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: DcuMessageProcessing.cs
* Created date:2022/5/27 1:07 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.Core;
using IotSystem.MessageProcessing.Queries;
using System;
using System.Data;

namespace IotSystem.MessageProcessing.DcuMessage
{
    public class DcuMessageProcessing : BusinessBase, IDcuMessageProcessing
    {

        public void InsertDcu(DataTable dcuTable)
        {
            try
            {
                SqlHelpers.TransactionCreating(ConnectionString);

                for (int i = 0; i < dcuTable.Rows.Count; i++)
                {
                    SqlHelpers.TransactionAdd(DcuQueries.InsertDcuQuery(dcuTable.Rows[i]["DcuCode"].ToString()));
                }

                SqlHelpers.TransactionCommit();

            }catch(Exception ex)
            {
                SqlHelpers.TransactionRollback();
                LogError(ex);
            }
        }

        public void UpdateDcuMeter()
        {
            
        }
    }
}
