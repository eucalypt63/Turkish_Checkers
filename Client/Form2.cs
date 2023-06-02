using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form2 : Form
    {
        Form Forms;
        public Form2(Form form)
        {
            InitializeComponent();
            Forms = form;
        }

        public string RoomName;
        public string FirstT;

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            Forms.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (RoomNameText.Text != "" && comboBox1.Text != "")
            {
                Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
                FirstT = ((Form1)Application.OpenForms["Form1"]).FirstT;
                string NickName = ((Form1)Application.OpenForms["Form1"]).NickName;

                RoomName = RoomNameText.Text;
                FirstT = comboBox1.Text;

                string str = "CreateR " + RoomNameText.Text + " " + comboBox1.Text + " " + NickName;
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

                if (s == "Available")
                {
                    this.Hide();
                    Form3 form3 = new Form3(Forms);
                    form3.Show();
                }
                else if (s == "NotAvailable")
                {
                    MessageBox.Show("Room Name is Not Available");
                }
            }
        }
    }
}
