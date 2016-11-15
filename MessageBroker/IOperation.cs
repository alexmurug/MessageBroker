using System.Threading.Tasks;

namespace MessageBroker
{
    public interface IOperation
    {
        Task<string> AsyncRead();
        Task AsyncWrite(string message);
        Task ReSendMessage();
    }
}