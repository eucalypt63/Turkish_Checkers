using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client 
{
    public partial class Form3 : Form
    {
        Form Forms;
        public Form3(Form form)
        {
            InitializeComponent();
            Forms = form;
        }

        int CountEn = 16;
        string FirstT;
        Boolean Flag;
        Boolean FirstClick = true;
        Boolean FlagCont = false;
        PictureBox FirstPicture = null;
        PictureBox SecondPicture = null;
        char Color;
        char ColorEn;
        int ICont = 10;
        int JCont = 10;

        Control FirstConrol = null;
        Control SecondConrol = null;

        string[,] GameArr = new string[8, 8];
        public List<TurnsCord> TurnsCords = new List<TurnsCord>();

        public class TurnsCord
        {
            public int CordI;
            public int CordJ;

            public TurnsCord(int Cordi, int Cordj)
            {
                CordI = Cordi;
                CordJ = Cordj;
            }
        }

        private void Game()
        {
            Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
            while (true)
            {
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

                if (parts[0] == "SecondPlayerTurn")
                {
                    int.TryParse(parts[1], out int i1);
                    int.TryParse(parts[2], out int j1);

                    int.TryParse(parts[3], out int i2);
                    int.TryParse(parts[4], out int j2);

                    PictureBox P1 = (PictureBox)this.Controls.Find("pictureBox" + i1 + j1, true)[0];
                    PictureBox P2 = (PictureBox)this.Controls.Find("pictureBox" + i2 + j2, true)[0];

                    P2.Image = P1.Image;
                    P1.Image = null;

                    GameArr[i2, j2] = GameArr[i1, j1];
                    GameArr[i1, j1] = "-";

                    Boolean Flag2 = false;
                    if (Math.Abs(i1 - i2) >= 2)
                    {
                        int max = Math.Max(i1, i2);
                        int min = Math.Min(i1, i2);
                        min += 1;
                        while (min < max)
                        {
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + min + j1, true)[0];
                            pictureBox.Image = null;
                            if (GameArr[min, j1][0] == Color)
                            {
                                Flag2 = true;
                            }
                            GameArr[min, j1] = "-";
                            min += 1;
                        }
                    }
                    else if (Math.Abs(j1 - j2) >= 2)
                    {
                        int max = Math.Max(j1, j2);
                        int min = Math.Min(j1, j2);
                        min += 1;
                        while (min < max)
                        {
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i1 + min, true)[0];
                            pictureBox.Image = null;
                            if (GameArr[i1, min][0] == Color)
                            {
                                Flag2 = true;
                            }
                            GameArr[i1, min] = "-";
                            min += 1;
                        }
                    }

                    if (!Flag2 || parts.Length == 6) 
                    {
                        label2.Invoke((MethodInvoker)delegate {
                            label7.Text = "Your Turn";
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                            string resourceName = Color + "0";
                            pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);
                        });
                        Flag = true;
                    }

                    if (i2 == 0)
                    {
                        GameArr[i2, j2] = ColorEn + "1";
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i2 + j2, true)[0];
                        if (ColorEn == 'B')
                        {
                            pictureBox.Image = Properties.Resources.B1;
                        }
                        else { pictureBox.Image = Properties.Resources.W1; }
                    }
                }
                else if (parts[0] == "PlayerFound")
                {
                    string PlayerFlag = ((Form1)Application.OpenForms["Form1"]).PlayerFlag;

                    label1.Invoke((MethodInvoker)delegate {
                        label5.Text = parts[1];
                    });
                }
                else if (parts[0] == "Surrend")
                {
                    MessageBox.Show("You Won, Enemy Surrended");


                    string str = "Disconnect D";
                    char[] charArray = str.ToCharArray();
                    var data = Encoding.UTF8.GetBytes(charArray);
                    tcpSocket.Send(data);
                    Application.Exit();

                }
                else if (parts[0] == "EnemyDisconnected")
                {
                    MessageBox.Show("You Won, Enemy left the Game");

                    string str = "Disconnect D";
                    char[] charArray = str.ToCharArray();
                    var data = Encoding.UTF8.GetBytes(charArray);
                    tcpSocket.Send(data);
                    Application.Exit();

                }
                else if (parts[0] == "EnemyW")
                {
                    MessageBox.Show("You Lose");

                    string str = "Disconnect D";
                    char[] charArray = str.ToCharArray();
                    var data = Encoding.UTF8.GetBytes(charArray);
                    tcpSocket.Send(data);
                    Application.Exit();

                }
            }
        }

        private void CheckEnd (int Count)
        {
            if (Count == 0)
            {
                Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
                string str = "EnemyL D";
                char[] charArray = str.ToCharArray();
                var data = Encoding.UTF8.GetBytes(charArray);
                tcpSocket.Send(data);
                MessageBox.Show("You won");

                string strs = "Disconnect D";
                char[] charArrays = strs.ToCharArray();
                var datas = Encoding.UTF8.GetBytes(charArrays);
                tcpSocket.Send(datas);
                Application.Exit();

            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameArr[i, j] = "-";
                }
            }

            string PlayerFlag = ((Form1)Application.OpenForms["Form1"]).PlayerFlag;
            if (PlayerFlag == "Creator")
            {
                label3.Text = ((Form1)Application.OpenForms["Form1"]).NickName;
                string RoomName = ((Form2)Application.OpenForms["Form2"]).RoomName;
                FirstT = ((Form2)Application.OpenForms["Form2"]).FirstT;

                if (FirstT == "Player1")
                {
                    Color = 'W';
                    ColorEn = 'B';
                    label7.Text = "Your Turn";

                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                    string resourceName = Color + "0";
                    pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                    Flag = true;
                    for (int i = 1; i != 3; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.W0;
                            GameArr[i, j] = "W0";
                        }
                    }

                    for (int i = 5; i != 7; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.B0;
                            GameArr[i, j] = "B0";
                        }
                    }

                }
                else if (FirstT == "Player2")
                {
                    Color = 'B';
                    ColorEn = 'W';
                    label7.Text = "Opponent's Turn";

                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                    string resourceName = ColorEn + "0";
                    pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                    Flag = false;

                    for (int i = 1; i != 3; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.B0;
                            GameArr[i, j] = "B0";
                        }
                    }

                    for (int i = 5; i != 7; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.W0;
                            GameArr[i, j] = "W0";
                        }
                    }
                }
            }
            else if (PlayerFlag == "Player")
            {
                label5.Text = ((Form1)Application.OpenForms["Form1"]).NickName;
                label3.Text = ((Form1)Application.OpenForms["Form1"]).NickNameSecond;
                FirstT = ((Form1)Application.OpenForms["Form1"]).FirstT;
                if (FirstT == "Player1")
                {
                    Color = 'B';
                    ColorEn = 'W';
                    label7.Text = "Opponent's Turn";

                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                    string resourceName = ColorEn + "0";
                    pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                    Flag = false;
                    for (int i = 1; i != 3; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.B0;
                            GameArr[i, j] = "B0";
                        }
                    }

                    for (int i = 5; i != 7; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.W0;
                            GameArr[i, j] = "W0";
                        }
                    }
                }   
                else if (FirstT == "Player2")
                {
                    Color = 'W';
                    ColorEn = 'B';
                    label7.Text = "Your Turn";

                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                    string resourceName = Color + "0";
                    pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                    Flag = true;
                    for (int i = 1; i != 3; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.W0;
                            GameArr[i, j] = "W0";
                        }
                    }

                    for (int i = 5; i != 7; i++)
                    {
                        for (int j = 0; j != 8; j++)
                        {
                            pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + j, true)[0];
                            pictureBox.Image = Properties.Resources.B0;
                            GameArr[i, j] = "B0";
                        }
                    }
                }
            }
            Thread thread = new Thread(new ThreadStart(Game));
            thread.Start();
        }

        private Boolean TurnsCont(int i, int j)
        {
            TurnsCords.Clear();
            Boolean Cont = false;
            if (GameArr[i, j][1] == '0') //Пешка
            {
                //Ход вверх
                if (i < 7)
                {
                    if (GameArr[i + 1, j][0] == ColorEn && i < 6 && GameArr[i + 2, j][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i + 2, j));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (i + 2) + j, true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        Cont = true;
                    }
                }

                //Ход влево
                if (j > 0)
                {
                    if (GameArr[i, j - 1][0] == ColorEn && j > 1 && GameArr[i, j - 2][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i, j - 2));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (j - 2), true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        Cont = true;
                    }
                }

                //Ход вправо
                if (j < 7)
                {
                    if (GameArr[i, j + 1][0] == ColorEn && j < 6 && GameArr[i, j + 2][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i, j + 2));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (j + 2), true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        Cont = true;
                    }
                }

            }
            else if (GameArr[i, j][1] == '1') //Дамка
            {
                //Ход вверх
                if (i < 7)
                {
                    int TempNum = 2;
                    int t = i;
                    while (TempNum != 0 && t < 7)
                    {
                        if (TempNum == 1 && GameArr[t + 1, j][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(t + 1, j));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (t + 1) + j, true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            Cont = true;
                            t++;
                        }
                        else if (GameArr[t + 1, j][0] == ColorEn)
                        {
                            TempNum--;
                            t++;
                        }
                        else if (GameArr[t + 1, j][0] == Color)
                        {
                            break;
                        }
                        else
                        {
                            t++;
                        }
                    }
                }

                //Ход влево
                if (j > 0)
                {
                    int TempNum = 2;
                    int t = j;
                    while (TempNum != 0 && t > 0)
                    {
                        if (TempNum == 1 && GameArr[i, t - 1][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(i, t - 1));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (t - 1), true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            Cont = true;
                            t--;
                        }
                        else if (GameArr[i, t - 1][0] == ColorEn)
                        {
                            TempNum--;
                            t--;
                        }
                        else if (GameArr[i, t - 1][0] == Color)
                        {
                            break;
                        }
                        else
                        {
                            t--;
                        }
                    }
                }

                //Ход вправо
                if (j < 7)
                {
                    int TempNum = 2;
                    int t = j;
                    while (TempNum != 0 && t < 7)
                    {
                        if (TempNum == 1 && GameArr[i, t + 1][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(i, t + 1));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (t + 1), true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            Cont = true;
                            t++;
                        }
                        else if (GameArr[i, t + 1][0] == ColorEn)
                        {
                            TempNum--;
                            t++;
                        }
                        else if (GameArr[i, t + 1][0] == Color)
                        {
                            break;
                        }
                        else
                        {
                            t++;
                        }
                    }
                }

                //Ход вниз
                if (i > 0)
                {
                    int TempNum = 2;
                    int t = i;
                    while (TempNum != 0 && t > 0)
                    {
                        if (TempNum == 1 && GameArr[t - 1, j][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(t - 1, j));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (t - 1) + j, true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            Cont = true;
                            t--;
                        }
                        else if (GameArr[t - 1, j][0] == ColorEn)
                        {
                            TempNum--;
                            t--;
                        }
                        else if (GameArr[t - 1, j][0] == Color)
                        {
                            break;
                        }
                        else
                        {
                            t--;
                        }
                    }
                }
            }
            return Cont;
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
            string str = "DisconnectG D";
            char[] charArray = str.ToCharArray();
            var data = Encoding.UTF8.GetBytes(charArray);
            tcpSocket.Send(data);

            string strs = "Disconnect D";
            char[] charArrays = strs.ToCharArray();
            var datas = Encoding.UTF8.GetBytes(charArrays);
            tcpSocket.Send(datas);
            Application.Exit();
        }

        private void Turns(int i, int j)
        {
            TurnsCords.Clear();
            if (GameArr[i, j][1] == '0') //Пешка
            {
                //Ход вверх
                if (i < 7)
                {
                    if (GameArr[i + 1, j][0] == '-')
                    {
                        //Ход прямо
                        TurnsCords.Add(new TurnsCord(i + 1, j));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (i+1) + j, true)[0];
                        pictureBox.Image = Properties.Resources.C3;

                    }
                    else if (GameArr[i + 1, j][0] == ColorEn && i < 6 && GameArr[i + 2, j][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i + 2, j));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (i+2) + j, true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        //Удар врага прямо
                    }
                }

                //Ход влево
                if (j > 0)
                {
                    if (GameArr[i, j - 1][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i, j - 1));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (j-1), true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        //Ход влево
                    }
                    else if (GameArr[i, j - 1][0] == ColorEn && j > 1 && GameArr[i, j - 2][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i, j - 2));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (j - 2), true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        //Удар врага влево
                    }
                }

                //Ход вправо
                if (j < 7)
                {
                    if (GameArr[i, j + 1][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i, j + 1));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (j + 1), true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        //Ход вправо
                    }
                    else if (GameArr[i, j + 1][0] == ColorEn && j < 6 && GameArr[i, j + 2][0] == '-')
                    {
                        TurnsCords.Add(new TurnsCord(i, j + 2));
                        PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (j + 2), true)[0];
                        pictureBox.Image = Properties.Resources.C3;
                        //Удар врага вправо
                    }
                }

            }
            else if (GameArr[i, j][1] == '1') //Дамка
            {
                //Ход вверх
                if (i < 7)
                {
                    int TempNum = 2;
                    int t = i;
                    while (TempNum != 0 && t < 7)
                    {
                        if (GameArr[t + 1, j][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(t + 1, j));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (t + 1) + j, true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            t++;
                        }
                        else if (GameArr[t + 1, j][0] == ColorEn)
                        {
                            TempNum--;
                            t++;
                        }
                        else if (GameArr[t + 1, j][0] == Color)
                        {
                            break;
                        }
                    }
                }

                //Ход влево
                if (j > 0)
                {
                    int TempNum = 2;
                    int t = j;
                    while (TempNum != 0 && t > 0)
                    {
                        if (GameArr[i, t - 1][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(i, t - 1));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (t - 1), true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            t--;
                        }
                        else if (GameArr[i, t - 1][0] == ColorEn)
                        {
                            TempNum--;
                            t--;
                        }
                        else if (GameArr[i, t - 1][0] == Color)
                        {
                            break;
                        }
                    }
                }

                //Ход вправо
                if (j < 7)
                {
                    int TempNum = 2;
                    int t = j;
                    while (TempNum != 0 && t < 7)
                    {
                        if (GameArr[i, t + 1][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(i, t + 1));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i + (t + 1), true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            t++;
                        }
                        else if (GameArr[i, t + 1][0] == ColorEn)
                        {
                            TempNum--;
                            t++;
                        }
                        else if (GameArr[i, t + 1][0] == Color)
                        {
                            break;
                        }
                    }
                }

                //Ход вниз
                if (i > 0)
                {
                    int TempNum = 2;
                    int t = i;
                    while (TempNum != 0 && t > 0)
                    {
                        if (GameArr[t - 1, j][0] == '-')
                        {
                            TurnsCords.Add(new TurnsCord(t - 1, j));
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + (t - 1) + j, true)[0];
                            pictureBox.Image = Properties.Resources.C3;
                            t--;
                        }
                        else if (GameArr[t - 1, j][0] == ColorEn)
                        {
                            TempNum--;
                            t--;
                        }
                        else if (GameArr[t - 1, j][0] == Color)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (Flag)
            {
                if (FirstClick)
                {
                    try
                    {
                        Point mouseLocation = panel1.PointToClient(Control.MousePosition);
                        Control control = panel1.GetChildAtPoint(mouseLocation);
                        PictureBox P1 = (PictureBox)this.Controls.Find(control.Name, true)[0];

                        int.TryParse(control.Name.Substring(control.Name.Length - 2, 1), out int i);
                        int.TryParse(control.Name.Substring(control.Name.Length - 1, 1), out int j);

                        if (control != null && GameArr[i, j][0] == Color)
                        {
                            FirstPicture = P1;
                            SecondPicture = null;
                            FirstClick = false;
                            FirstConrol = control;
                            Turns(i, j);
                        }
                        else
                        {
                            FirstConrol = null;
                            FirstPicture = null;
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    try
                    {
                        Point mouseLocation = panel1.PointToClient(Control.MousePosition);
                        Control control = panel1.GetChildAtPoint(mouseLocation);
                        PictureBox P1 = (PictureBox)this.Controls.Find(control.Name, true)[0];

                        int.TryParse(control.Name.Substring(control.Name.Length - 2, 1), out int i);
                        int.TryParse(control.Name.Substring(control.Name.Length - 1, 1), out int j);
                        Boolean Flag1 = false;
                        foreach (TurnsCord TurnsC in TurnsCords)
                        {
                            if (TurnsC.CordI == i && TurnsC.CordJ == j)
                            {
                                Flag1 = true;
                            }
                        }

                        if (control != null && Flag1)
                        {
                            foreach (TurnsCord TurnsC in TurnsCords)
                            {
                                PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + TurnsC.CordI + TurnsC.CordJ, true)[0];
                                pictureBox.Image = null;
                            }
                            TurnsCords.Clear();

                            Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
                            SecondPicture = P1;
                            FirstClick = true;
                            SecondConrol = control;

                            int.TryParse(FirstConrol.Name.Substring(FirstConrol.Name.Length - 2, 1), out int i1);
                            int.TryParse(FirstConrol.Name.Substring(FirstConrol.Name.Length - 1, 1), out int j1);

                            int.TryParse(SecondConrol.Name.Substring(SecondConrol.Name.Length - 2, 1), out int i2);
                            int.TryParse(SecondConrol.Name.Substring(SecondConrol.Name.Length - 1, 1), out int j2);

                            //Функция хода для пешки
                            string Flag2 = "Turn";

                            if (Math.Abs(i1 - i2) >= 2)
                            {
                                int max = Math.Max(i1, i2);
                                int min = Math.Min(i1, i2);
                                min += 1;
                                while (min < max)
                                {
                                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + min + j1, true)[0];
                                    pictureBox.Image = null;
                                    if (GameArr[min, j1][0] == ColorEn)
                                    {
                                        Flag2 = "TurnB";
                                        CountEn--;
                                    }
                                    GameArr[min, j1] = "-";
                                    min += 1;
                                }
                            }
                            else if (Math.Abs(j1 - j2) >= 2)
                            {
                                int max = Math.Max(j1, j2);
                                int min = Math.Min(j1, j2);
                                min += 1;
                                while (min < max)
                                {
                                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i1 + min, true)[0];
                                    pictureBox.Image = null;
                                    if (GameArr[i1, min][0] == ColorEn)
                                    {
                                        Flag2 = "TurnB";
                                        CountEn--;
                                    }
                                    GameArr[i1, min] = "-";
                                    min += 1;
                                }
                            }

                            GameArr[i2, j2] = GameArr[i1, j1];
                            GameArr[i1, j1] = "-";

                            if (i2 == 7)
                            {
                                GameArr[i2, j2] = Color + "1";
                                PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i2 + j2, true)[0];
                                if (Color == 'B')
                                {
                                    pictureBox.Image = Properties.Resources.B1;
                                }
                                else { pictureBox.Image = Properties.Resources.W1; }
                            }
                            else
                            {
                                SecondPicture.Image = FirstPicture.Image;
                            }

                            string str = "GameTurn " + (7 - i1) + " " + (7 - j1) + " " + (7 - i2) + " " + (7 - j2);

                            FirstPicture.Image = null;
                            if (Flag2 == "Turn")
                            {
                                label7.Text = "Opponent's Turn";
                                PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                                string resourceName = ColorEn + "0";
                                pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                                FirstConrol = null;
                                SecondConrol = null;
                               
                                SecondPicture = null;
                                Flag = false;
                            }
                            else if (Flag2 == "TurnB")
                            {
                                if (TurnsCont(i2, j2)) //
                                {
                                    ICont = i2;
                                    JCont = j2;
                                    FlagCont = true;
                                    Flag = false;
                                    FirstPicture = SecondPicture;
                                }
                                else
                                {
                                    label7.Text = "Opponent's Turn";
                                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                                    string resourceName = ColorEn + "0";
                                    pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                                    FirstConrol = null;
                                    SecondConrol = null;

                                    SecondPicture = null;
                                    Flag = false;

                                    str += " -";
                                }
                            }
                            char[] charArray = str.ToCharArray();
                            var data = Encoding.UTF8.GetBytes(charArray);
                            tcpSocket.Send(data);

                        }
                        else
                        {
                            FirstClick = true;
                            FirstPicture = null;

                            foreach (TurnsCord TurnsC in TurnsCords)
                            {
                                PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + TurnsC.CordI + TurnsC.CordJ, true)[0];
                                pictureBox.Image = null;
                            }
                            TurnsCords.Clear();
                        }
                    }
                    catch
                    { }
                    CheckEnd(CountEn);
                }
            }
            else if (FlagCont)
            {
                try
                {
                    Point mouseLocation = panel1.PointToClient(Control.MousePosition);
                    Control control = panel1.GetChildAtPoint(mouseLocation);
                    PictureBox P1 = (PictureBox)this.Controls.Find(control.Name, true)[0];

                    int.TryParse(control.Name.Substring(control.Name.Length - 2, 1), out int i);
                    int.TryParse(control.Name.Substring(control.Name.Length - 1, 1), out int j);
                    Boolean Flag1 = false;
                    foreach (TurnsCord TurnsC in TurnsCords)
                    {
                        if (TurnsC.CordI == i && TurnsC.CordJ == j)
                        {
                            Flag1 = true;
                            break;
                        }
                    }

                    if (control != null && Flag1)
                    {
                        int i2 = i;
                        int j2 = j;
                        int i1 = ICont;
                        int j1 = JCont;
                        foreach (TurnsCord TurnsC in TurnsCords)
                        {
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + TurnsC.CordI + TurnsC.CordJ, true)[0];
                            pictureBox.Image = null;
                        }
                        TurnsCords.Clear();

                        Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
                        SecondPicture = P1;
                        FirstClick = true; //

                        if (Math.Abs(i1 - i2) >= 2)
                        {
                            int max = Math.Max(i1, i2);
                            int min = Math.Min(i1, i2);
                            min += 1;
                            while (min < max)
                            {
                                PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + min + j1, true)[0];
                                pictureBox.Image = null;                          
                                if (GameArr[min, j1][0] == ColorEn)
                                    CountEn--;
                                GameArr[min, j1] = "-";
                                min += 1;
                            }
                        }
                        else if (Math.Abs(j1 - j2) >= 2)
                        {
                            int max = Math.Max(j1, j2);
                            int min = Math.Min(j1, j2);
                            min += 1;
                            while (min < max)
                            {
                                PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i1 + min, true)[0];
                                pictureBox.Image = null;
                                if (GameArr[i1, min][0] == ColorEn)
                                    CountEn--;
                                GameArr[i1, min] = "-";
                                min += 1;
                            }
                        }

                        GameArr[i2, j2] = GameArr[i1, j1];
                        GameArr[i1, j1] = "-";

                        if (i2 == 7)
                        {
                            GameArr[i2, j2] = Color + "1";
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i2 + j2, true)[0];
                            if (Color == 'B')
                            {
                                pictureBox.Image = Properties.Resources.B1;
                            }
                            else { pictureBox.Image = Properties.Resources.W1; }
                        }
                        else
                        {
                            SecondPicture.Image = FirstPicture.Image;
                        }

                        FirstPicture.Image = null;
                        string str = "GameTurn " + (7 - i1) + " " + (7 - j1) + " " + (7 - i2) + " " + (7 - j2);
                        if (TurnsCont(i2, j2))
                        {
                            ICont = i2;
                            JCont = j2;
                            FirstPicture = SecondPicture;
                        }
                        else
                        {
                            label7.Text = "Opponent's Turn";
                            PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBoxTurn", true)[0];
                            string resourceName = ColorEn + "0";
                            pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                            FirstConrol = null;
                            SecondConrol = null;
                            FirstPicture.Image = null;
                            SecondPicture = null;

                            Flag = true;
                            FlagCont = false;

                            str += " -";
                        }

                        char[] charArray = str.ToCharArray();
                        var data = Encoding.UTF8.GetBytes(charArray);
                        tcpSocket.Send(data);
                    }
                }
                catch { }
                CheckEnd(CountEn);
            }        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Socket tcpSocket = ((Form1)Application.OpenForms["Form1"]).tcpSocket;
            string str = "Surrend D";
            char[] charArray = str.ToCharArray();
            var data = Encoding.UTF8.GetBytes(charArray);
            tcpSocket.Send(data);
            MessageBox.Show("You lose");

            try
            {
                string strs = "Disconnect D";
                char[] charArrays = strs.ToCharArray();
                var datas = Encoding.UTF8.GetBytes(charArrays);
                tcpSocket.Send(datas);
                Application.Exit();
            }
            catch
            { }
        }
    }
}