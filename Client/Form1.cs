using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string NickName;
        public string PlayerFlag;
        public string FirstT;
        public string NickNameSecond;

        const string ip = "127.0.0.1";
        const int port = 8000;

        private IPEndPoint tcpEndPoind = new IPEndPoint(IPAddress.Parse(ip), port);
        public Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private void Form1_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            NameText.Text = "Player" + random.Next(10) + random.Next(10) + random.Next(10) + random.Next(10) + random.Next(10) + random.Next(10);

            tcpSocket.Connect(tcpEndPoind);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (NameText.Text != "")
            {
                NickName = NameText.Text;
                PlayerFlag = "Creator";

                this.Hide();                
                Form2 form2 = new Form2(this);
                form2.Show();
            }
            else { MessageBox.Show("Incorrect Nick Name"); }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            PlayerFlag = "Player";
            NickName = NameText.Text;
            string str = "EntryR " + RoomNameJoin.Text + " " + NickName;
            char[] charArray = str.ToCharArray();
            var data = Encoding.UTF8.GetBytes(charArray);
            tcpSocket.Send(data);

            var buffer = new byte[48];
            var datas = new StringBuilder();

            do
            {
                var size = tcpSocket.Receive(buffer);
                datas.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (tcpSocket.Available > 0);

            string s = datas.ToString();
            string[] parts = s.Split(' ');

            if (parts[0] == "RoomFound")
            {
                FirstT = parts[1];
                NickNameSecond = parts[2];

                this.Hide();
                Form3 form3 = new Form3(this);
                form3.Show();
            }
            else if (s == "RoomNotFound")
            {
                MessageBox.Show("Room doesn't exist");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                string str = "Disconnect D";
                char[] charArray = str.ToCharArray();
                var data = Encoding.UTF8.GetBytes(charArray);
                tcpSocket.Send(data);
                Application.Exit();
            }
            catch
            { }
        }
    }
}