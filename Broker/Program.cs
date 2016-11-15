using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MessageBroker;

namespace Broker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("HELLO. I'm Broker...");

            IOperation brokerOperation = new BrokerService();

            Task t = Task.Factory.StartNew(async () =>
            {
                string m;
                while ((m = await brokerOperation.AsyncRead()) != "quit b")
                {
                    var m1 = m;
                    await Task.Factory.StartNew(()=>brokerOperation.AsyncWrite(m1));
                }
            });
            t.Wait();

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await brokerOperation.ReSendMessage();
                    Thread.Sleep(5000);
                }
            });

            Console.ReadLine();
        }
    }
}