using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server 
{
    internal class Program
    {
        const string ip1 = "127.0.0.1";
        const int port = 8000;

        IPEndPoint tcpEndPoind = new IPEndPoint(IPAddress.Parse(ip1), port);
        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public List<Player> MyClassList = new List<Player>();

        public class Player
        {
            public string playerNickName = "";
            public string clientIP;
            public int clientPort;
            public string RoomName = "";
            public Socket listener;
            public Player PlayerConnected = null;
            public string FirstTurn = "";

            public Player(string ClientIP, int ClientPort, Socket Listener)
            {
                clientIP = ClientIP;
                clientPort = ClientPort;
                listener = Listener;
            }
        }

        static void Main(string[] args)
        {
            var mc = new Program();
            mc.tcpSocket.Bind(mc.tcpEndPoind);
            mc.tcpSocket.Listen(1000);

            while (true)
            {
                Socket listener = mc.tcpSocket.Accept();

                string clientIP = ((IPEndPoint)listener.RemoteEndPoint).Address.ToString();
                int clientPort = ((IPEndPoint)listener.RemoteEndPoint).Port;
                Console.WriteLine("Client: " + clientIP + " " + clientPort + " Connected");

                Player myObject = new Player(clientIP, clientPort, listener);
                mc.MyClassList.Add(myObject);

                Thread thread = new Thread(new ThreadStart(mc.Worker));
                thread.Start();
            }
        }

        private void Worker()
        {
            Player player = MyClassList[MyClassList.Count - 1];

            while (player.listener.Connected)
            {
                var buffer = new byte[48];
                var data = new StringBuilder();
                try
                {
                    do
                    {
                        var size = player.listener.Receive(buffer);
                        data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                    }
                    while (tcpSocket.Available > 0);
                }
                catch
                {
                    break;
                }
                string s = data.ToString();
                string[] parts = s.Split(' ');

                if (parts[0] == "EntryR")//
                {
                    Player SecondPlayer = null;
                    Boolean Flag = false;
                    foreach (Player Player in MyClassList)
                    {
                        if (Player.RoomName == parts[1])
                        {
                            SecondPlayer = Player;
                            Player.PlayerConnected = player;
                            Flag = true;
                            break;
                        }
                    }
                    string str;

                    if (Flag)
                    {
                        player.PlayerConnected = SecondPlayer;
                        player.PlayerConnected.PlayerConnected = player;
                        player.playerNickName = parts[2];

                        player.RoomName = parts[1];
                        str = "RoomFound " + player.PlayerConnected.FirstTurn + " " + player.PlayerConnected.playerNickName;
                        Console.WriteLine("Game Started: Player 1 - " + player.PlayerConnected.clientPort + " / Player 2 - " + player.clientPort);

                        string strs = "PlayerFound " + player.playerNickName;
                        char[] charArrayS = strs.ToCharArray();
                        var datass = Encoding.UTF8.GetBytes(charArrayS);
                        player.PlayerConnected.listener.Send(datass); 
                    }
                    else
                    {
                        str = "RoomNotFound";
                    }
                    char[] charArray = str.ToCharArray();
                    var datas = Encoding.UTF8.GetBytes(charArray);
                    player.listener.Send(datas);

                }
                else if (parts[0] == "CreateR")
                {
                    Boolean Flag = true;
                    foreach (Player Player in MyClassList)
                    {
                        if (Player.RoomName == parts[1])
                        {
                            Flag = false;
                            break;
                        }
                    }

                    string str = "";
                    if (Flag)
                    {
                        player.RoomName = parts[1];
                        player.FirstTurn = parts[2];
                        player.playerNickName = parts[3];

                        Console.WriteLine("Room: " + parts[1] + " created");
                        str = "Available";
                    }
                    else
                    {
                        str = "NotAvailable";
                    }

                    char[] charArray = str.ToCharArray();
                    var datas = Encoding.UTF8.GetBytes(charArray);
                    player.listener.Send(datas);
                }
                else if (parts[0] == "GameTurn")
                {
                    string str = "SecondPlayerTurn " + parts[1] + " " + parts[2] + " " + parts[3] + " " + parts[4];
                    if (parts.Length == 6)
                    {
                        str += " -";
                    }
                    char[] charArray = str.ToCharArray();
                    var datas = Encoding.UTF8.GetBytes(charArray);
                    player.PlayerConnected.listener.Send(datas);
                }
                else if (parts[0] == "Disconnect")
                {
                    break;
                }
                else if (parts[0] == "DisconnectG")
                {
                    player.RoomName = "";

                    if (player.PlayerConnected != null)
                    {
                        string str = "EnemyDisconnected D";

                        char[] charArray = str.ToCharArray();
                        var datas = Encoding.UTF8.GetBytes(charArray);
                        player.PlayerConnected.listener.Send(datas);

                        player.PlayerConnected.RoomName = "";
                        player.PlayerConnected.PlayerConnected = null;
                        player.PlayerConnected = null;
                    }
                }
                else if (parts[0] == "Surrend")
                {
                    string str = "Surrend S";

                    char[] charArray = str.ToCharArray();
                    var datas = Encoding.UTF8.GetBytes(charArray);
                    player.PlayerConnected.listener.Send(datas);

                    player.RoomName = "";
                    player.PlayerConnected.RoomName = "";
                    player.PlayerConnected.PlayerConnected = null;
                    player.PlayerConnected = null;
                }
                else if (parts[0] == "EnemyL")
                {
                    string str = "EnemyW L";

                    char[] charArray = str.ToCharArray();
                    var datas = Encoding.UTF8.GetBytes(charArray);
                    player.PlayerConnected.listener.Send(datas);

                    player.RoomName = "";
                    player.PlayerConnected.RoomName = "";
                    player.PlayerConnected.PlayerConnected = null;
                    player.PlayerConnected = null;
                }
            }

            Boolean FlagClose = false;
            int i = 0;
            foreach (Player Player in MyClassList)
            {
                if (Player == player)
                {
                    FlagClose = true;
                    break;
                }
                i++;
            }

            if (FlagClose)
            {
                Console.WriteLine("Client: " + MyClassList[i].clientIP + " " + MyClassList[i].clientPort + " Disconnected");
                MyClassList.RemoveAt(i);
            }
        }
    }
}