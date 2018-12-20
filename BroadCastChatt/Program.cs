using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BroadCastChatt
{
    class Program
    {
        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd/HH-mm ");
        }
        private const int ListenPort = 11000;
        static void Main(string[] args)
        {

            //Skapar en tråd som körs samtidigt med huvudprogrammet
            var listenThread = new Thread(Listener);
            listenThread.Start();


            //Skapar en lokal anslutning via udp och IPv4
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            
            IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, 11000);

            Thread.Sleep(1000);

            while(true)
            {
                Console.Write(">");
                string msg = Console.ReadLine();

                byte[] sendbuf = Encoding.UTF8.GetBytes(msg);
                socket.SendTo(sendbuf, ep);
                Thread.Sleep(200);
            }
        }
        static void Listener()
        {
            //Skapar ett objekt som ska lyssna efter meddelanden
            UdpClient listener = new UdpClient(ListenPort);
            string timeStamp = GetTimestamp(DateTime.Now);

            
            string myIP = "192.168.80.45";

            try
            {
                while (true)
                {
                    
                    //Skapar objekt som lyssnar efter trafik från valfri ip-address men via port
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ListenPort);

                    //Tar emot meddelande i en array
                    byte[] bytes = listener.Receive(ref groupEP);
                   string grupp = groupEP.ToString();
                    if (myIP != grupp)

                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                    }
                    Console.ForegroundColor = ConsoleColor.White;

                    //Skrivet ut meddelandet
                    Console.WriteLine(timeStamp + "Received broadcast from {0} : {1}\n", groupEP.ToString(), Encoding.UTF8.GetString(bytes, 0, bytes.Length));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }


    }
}
