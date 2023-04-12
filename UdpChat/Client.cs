using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpChat
{
    internal class Client
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public Client(string name)
        {
            Name= name;
            ID = Guid.NewGuid().ToString();
        }
    }
}
