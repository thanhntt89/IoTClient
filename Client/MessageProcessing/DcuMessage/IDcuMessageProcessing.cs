using System.Data;

namespace IotSystem.MessageProcessing.DcuMessage
{
    public interface IDcuMessageProcessing
    {
        void InsertDcu(DataTable dcuTable);
        void UpdateDcuMeter();
    }
}
