using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ObjectClient
    {
        public Socket socketClient { get; set; }
        public string name { get; set; }


        public ObjectClient(Socket socket)
        {
            socketClient = socket.Accept();
        }
    }
    
    
}
