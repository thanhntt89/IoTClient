using static IotClient.ClientEvent;

namespace IotClient
{
    public interface IClient
    {
        void ShowMessage(DelegateShowMessage showMessage);      
        void Start();
        void Stop();        
    }
}
