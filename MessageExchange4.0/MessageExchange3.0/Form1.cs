using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessageExchange3._0
{
    public partial class Form1 : Form
    {
        string ip = "127.0.0.1";
        int port = 22316;
        IPAddress ipAddr;
        IPEndPoint endPoint;
        Socket socket;
        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[0].Text = "Messages:";
            listView1.Columns[0].Width =410;

            ipAddr = IPAddress.Parse(ip);
            endPoint = new IPEndPoint(ipAddr, port);
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.IP);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (socket.Connected==false)
            {
                if (textBox1.Text != "")
                {
                    try
                    {
                        socket.Connect(endPoint);
                        if (socket.Connected)
                        {
                            SendMessage("New client");

                            listView1.Items.Add(ReceivedMessage()+ " You are connected!");

                            SendMessage(textBox1.Text);

                            char delimiter = ';';
                            string[] substrings = ReceivedMessage().Split(delimiter);

                            foreach (var item in substrings)
                            {
                                listBox1.Items.Add(item);
                            }
                            textBox1.Enabled = false;
                            button1.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Введите login!");
                }
            }
        }        

        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                try
                {
                    if (socket.Connected)
                    {
                        if ((textBox3.Text != "") && (listBox1.SelectedIndex!=-1))
                        {
                            SendMessage("Send messages");
                            SendMessage((listBox1.SelectedItem
                                +";"+ DateTime.Now.ToLongTimeString() + ": "+ textBox1.Text + " > " + textBox3.Text+"&"));
                            listView1.Items.Add((DateTime.Now.ToLongTimeString() +
                                ": " + listBox1.SelectedItem + " < " + textBox3.Text));
                        }
                        else
                        {
                            MessageBox.Show("Выбериле получателя и введите сообщение!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Подключитесь к серверу!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Введите login, подключитесь к серверу и выберите получателя!");
            }
            textBox3.Text = "";            
        }

        public void SendMessage(string s)
        {
            string msg = s;
            byte[] sendBytes = Encoding.Default.GetBytes(msg);
            socket.Send(sendBytes);
        }

        public string ReceivedMessage()
        {
            int bytesReceived = 0;
            byte[] bytes = new byte[255];
            bytesReceived = socket.Receive(bytes);
            string res = Encoding.Default.GetString(bytes).TrimEnd('\0');
            return res;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            socket.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            SendMessage("Get clients");

            char delimiter = ';';
            string[] substrings = ReceivedMessage().Split(delimiter);

            foreach (var item in substrings)
            {
                if (item!="")
                {
                    listBox1.Items.Add(item);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                try
                {
                    if (socket.Connected)
                    {
                        SendMessage("Receive messages");
                        SendMessage(textBox1.Text);
                                                
                        char delimiter = '&';
                        string[] substrings = ReceivedMessage().Split(delimiter);

                        for (int i = 0; i < substrings.Length; i++)
                        {
                            if (substrings[i]!="")
                            {
                                listView1.Items.Add((substrings[i]));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Подключитесь к серверу!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Введите login, подключитесь к серверу и выберите получателя!");
            }
        }
    }
}
