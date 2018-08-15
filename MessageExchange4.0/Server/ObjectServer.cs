using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ObjectServer
    {
        string ip;
        int port;
        IPAddress ipAddr;
        IPEndPoint endPoint;
        Socket socket;
        ObjectClient client;
        public static List<ObjectClient> clients =new List<ObjectClient>();
        public static Dictionary<string, string> messages;

        public ObjectServer()
        {
            ip = "127.0.0.1";
            port = 22316;
            messages = new Dictionary<string, string>();
            messages.Add("test", "test > Hello!");
            ServerStart();
        }
        private void ServerStart()
        {
            ipAddr = IPAddress.Parse(ip);
            endPoint = new IPEndPoint(ipAddr, port);
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.IP);
            socket.Bind(endPoint);
            socket.Listen(10);
            Console.WriteLine("Server started...");
            do
            {
                client = new ObjectClient(socket);
                    clients.Add(client);
                    Task.Factory.StartNew(() => ProcessClient(client, socket));
            } while (true);
        }

        static void ProcessClient(ObjectClient client, Socket socket)
        {
            int bytesReceived = 0;
            try
            {
                do
                {
                    byte[] bytes = new byte[255];
                    bytesReceived = client.socketClient.Receive(bytes);
                    string msg = Encoding.Default.GetString(bytes).TrimEnd('\0');
                                                           
                    if (msg== "New client")
                    {
                        string currentTime = DateTime.Now.ToLongTimeString();
                        byte[] data = Encoding.Default.GetBytes(currentTime);
                        client.socketClient.Send(data);

                        byte[] bytes2 = new byte[255];
                        bytesReceived = client.socketClient.Receive(bytes2);
                        string msg2 = Encoding.Default.GetString(bytes2).TrimEnd('\0');
                        client.name = msg2;

                        string allClients = "";
                        for (int i = 0; i < clients.Count; i++)
                        {
                            allClients += clients[i].name+";";
                        }
                        byte[] msgAllClients = Encoding.Default.GetBytes(allClients);
                        client.socketClient.Send(msgAllClients);
                    }
                    else if (msg=="Get clients")
                    {
                        string allClients="";
                        for (int i = 0; i < clients.Count; i++)
                        {
                            allClients += clients[i].name + ";";
                        }
                        byte[] msgAllClients = Encoding.Default.GetBytes(allClients);
                        client.socketClient.Send(msgAllClients);
                    }
                    else if (msg == "Send messages")
                    {                        
                        byte[] bytes2 = new byte[255];
                        bytesReceived = client.socketClient.Receive(bytes2);
                        string msg2 = Encoding.Default.GetString(bytes2).TrimEnd('\0');

                        char delimiter1 = ';';
                        string[] mess = msg2.Split(delimiter1);

                        if (messages.ContainsKey(mess[0]))
                        {
                            messages[mess[0]] += mess[1];
                        }
                        else
                        {
                            messages.Add(mess[0], (mess[1]));
                        }
                    }
                    else if (msg == "Receive messages")
                    {
                        byte[] bytes2 = new byte[255];
                        bytesReceived = client.socketClient.Receive(bytes2);
                        string msg2 = Encoding.Default.GetString(bytes2).TrimEnd('\0');
                 
                        if (messages.ContainsKey(msg2))
                        {
                            byte[] data2 = Encoding.Default.GetBytes(messages[msg2]);
                            client.socketClient.Send(data2);
                            messages.Remove(msg2);
                            messages.Remove("test");
                        }
                        else if (messages.ContainsKey("test"))
                        {
                            byte[] data2 = Encoding.Default.GetBytes(DateTime.Now.ToLongTimeString()
                                +": "+messages["test"]);
                            client.socketClient.Send(data2);
                            messages.Remove("test");
                        }
                        else
                        {
                            byte[] data3 = Encoding.Default.GetBytes(DateTime.Now.ToLongTimeString() 
                                +": "+ "no messages");
                            client.socketClient.Send(data3);
                        }
                    }
                } while (bytesReceived > 0);
            }
            catch (Exception ex)
            {
                if (ex.Message!="")
                {
                    Console.WriteLine(ex.Message);
                }
                client.socketClient.Close();
            }
        }


    }
}
