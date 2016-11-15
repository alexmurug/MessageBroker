using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker
{
    public class TransportService : IOperation
    {
        private readonly UdpClient _transporClient = new UdpClient();

        public TransportService(IPEndPoint broker)
        {
            _transporClient.Connect(broker);
        }

        public async Task<string> AsyncRead()
        {
            var rec = await _transporClient.ReceiveAsync();
            return Encoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);
        }

        public async Task AsyncWrite(string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            await _transporClient.SendAsync(bytes, bytes.Length);
        }

        public Task ReSendMessage()
        {
            throw new System.NotImplementedException();
        }
    }
}