using System;

namespace MessageBroker
{
    public class Messages
    {
        private DateTime _date;
        private string _message;
        private string _sender;

        public Messages(string message, DateTime date, string sender)
        {
            _message = message;
            _date = date;
            _sender = sender;
        }
    }
}