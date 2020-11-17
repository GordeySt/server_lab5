using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.CMD
{
    [Serializable]
    public class Message
    {
        public string From { get; set; }
        public string To { get; }
        public string Subject { get; }
        public string Mes { get; }

        public Message(string to, string from, string subject,
            string message)
        {
            To = to;
            From = from;
            Subject = subject;
            Mes = message;
        }        
    }
}
