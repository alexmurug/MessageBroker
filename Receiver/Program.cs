using System;
using System.Net;
using System.Threading.Tasks;
using MessageBroker;

namespace Receiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("HELLO! I'm Receiver...");

            IOperation receiverOperation = new TransportService(new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 32123));

            Task t = Task.Factory.StartNew(async () =>
            {
                var message = ReceiverInfo();
                await receiverOperation.AsyncWrite(message);
                string m;
                while ((m = await receiverOperation.AsyncRead()) != "quit r")
                    Console.WriteLine(Environment.NewLine + m);
            });
            t.Wait();
            Console.ReadLine();
        }

        private static string ReceiverInfo()
        {
            var user = new SendMessage();
            Console.Write("Your Name: ");
            user.SenderName = Console.ReadLine();
            Console.Write("Your subject : ");
            user.Subject = Console.ReadLine();
            //Console.Write("Your Action : ");
            user.Message = "subscribe";
            return SendMessage.SerializeUserData(user);
        }
    }
}