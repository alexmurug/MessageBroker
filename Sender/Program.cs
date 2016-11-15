using System;
using System.Net;
using System.Threading.Tasks;
using MessageBroker;

namespace Sender
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("HELLO! I'm a sender...");
            IOperation senderOperation = new TransportService(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 32123));

            var user = new SendMessage();
            Auth(user);

            var t = Task.Factory.StartNew(() =>
            {
                string message;
                BuildMessage(user, out message);
                while (user.Message != "quit")
                {
                    senderOperation.AsyncWrite(message);
                    BuildMessage(user, out message);
                }
            });
            t.Wait();
        }


        private static void Auth(SendMessage user)
        {
            Console.Write("Your Name: ");
            user.SenderName = Console.ReadLine();
        }

        private static void BuildMessage(SendMessage user, out string message)
        {
            Console.Write("\nSubject: ");
            var curSubject = Console.ReadLine();
            user.Subject = curSubject;
            Console.Write("Message: ");
            user.Message = Console.ReadLine();
            message = SendMessage.SerializeUserData(user);
        }
    }
}