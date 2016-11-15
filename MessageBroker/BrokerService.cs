using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MessageBroker
{
    public class BrokerService : IOperation
    {
        //Lista cu mesajele primite
        private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
        //Lista cu receiveri
        private readonly HashSet<Receivers> _receivers = new HashSet<Receivers>();
        private readonly UdpClient _tranportClient;
        //Lista cu mesajele netrimise
        public readonly ConcurrentQueue<string> UnreceivedList = new ConcurrentQueue<string>();

        public BrokerService()
        {
            _tranportClient = new UdpClient(32123);
        }

        public async Task<string> AsyncRead()
        {
            var rec = await _tranportClient.ReceiveAsync();
            var s = Encoding.ASCII.GetString(rec.Buffer, 0, rec.Buffer.Length);

            Console.WriteLine(s);
            SendMessage client;
            DeserializeMessage(s, out client);
            if (client.Message.Equals("subscribe"))
            {
                AddReceiver(rec, client);
                return "";
            }

            _messageQueue.Enqueue(s);
            return s;
        }

        public async Task AsyncWrite(string message)
        {
            var mess = message;
            if (message.Length > 0)
            {
                SendMessage client;

                DeserializeMessage(message, out client);
                var curSubject = SubjectDeterminator.GetSubject(client.Subject);
                var recName = SubjectDeterminator.GetUnicast(client.Subject);
                var curTime = DateTime.Now;
                var sendMsg = new SendMessage
                {
                    Message = client.Message,
                    Subject = client.Subject,
                    Date = curTime.ToString(CultureInfo.InvariantCulture),
                    SenderName = client.SenderName
                };
                var xmlMsg = SendMessage.SerializeUserData(sendMsg);
                var bytes = Encoding.ASCII.GetBytes(client.SenderName + ": " + client.Message+" ["+curTime+"]");

                

                var receiver = _receivers.ToList().Find(rec => rec.Name == recName);
                var receivers = _receivers.ToList().FindAll(rec => rec.Subject == curSubject);

                if (curSubject == "broadcast") 
                {
                    foreach (var rec in _receivers)
                        await _tranportClient.SendAsync(bytes, bytes.Length, rec.IpEndPoint);
                    _messageQueue.TryDequeue(out message);
                   // Console.WriteLine("broascasta message");
                }

                else if ((receivers.Count != 0) && recName.Length.Equals(0))
                {
                    foreach (var rec in receivers)
                        await _tranportClient.SendAsync(bytes, bytes.Length, rec.IpEndPoint);
                   // Console.WriteLine("subject message");
                    _messageQueue.TryDequeue(out message);
                }
                else if (receiver != null)
                {
                    //Console.WriteLine("unicast message");
                    await _tranportClient.SendAsync(bytes, bytes.Length, receiver.IpEndPoint);
                    _messageQueue.TryDequeue(out message);
                }
                else
                {
                    //Console.WriteLine(" message not sended");
                    UnreceivedList.Enqueue(xmlMsg);
                    _messageQueue.TryDequeue(out message);
                }
            }
        }

        public async Task ReSendMessage()
        {
            foreach (var mess in UnreceivedList)
            {
                SendMessage newUserData;
                DeserializeMessage(mess, out newUserData);
                var user = SubjectDeterminator.GetUnicast(newUserData.Subject);
                var bytes = Encoding.ASCII.GetBytes(newUserData.SenderName + ": " + newUserData.Message + " [" + newUserData.Date + "]");

                var rc1 = _receivers.ToList().Find(y => y.Name == user);

                if (rc1 != null)
                {
                    await _tranportClient.SendAsync(bytes, bytes.Length, rc1.IpEndPoint);
                    string mess1;
                    UnreceivedList.TryDequeue(out mess1);
                    Console.WriteLine("Resending was done successfull!!!");
                }
                else
                {
                    Console.WriteLine("Failed!!");
                }
            }
        }

        private static void DeserializeMessage(string message, out SendMessage user)
        {
            var formatter = new XmlSerializer(typeof(SendMessage));
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(message);
                writer.Flush();
                stream.Position = 0;

                user = (SendMessage) formatter.Deserialize(stream);
            }
        }


        private void AddReceiver(UdpReceiveResult rec, SendMessage user)
        {
            var receiver = new Receivers
            {
                Name = user.SenderName,
                IpEndPoint = rec.RemoteEndPoint,
                Subject = user.Subject
            };
            _receivers.Add(receiver);
        }
    }
}