using System.Threading;

namespace IotClient
{
    public class ClientEvent
    {
        public delegate void DelegateShowMessage(string message);

        public delegate void DelegateThread(CancellationToken cancellation);
    }
}
