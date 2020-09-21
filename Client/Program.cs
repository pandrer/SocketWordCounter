using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{

    class Program
    {
        static void Main(string[] args)
        {
            ExecuteClient();
        }

        static void ExecuteClient()
        {

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];


                try
                {

                    while (true)
                    {
                        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
                        Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        sender.Connect(localEndPoint);
                        Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());
                        Console.Write("Escriba el nombre del archivo: ");
                        var fileName = Console.ReadLine();
                        Console.WriteLine("Your input: {0}", fileName);
                        byte[] messageSent = Encoding.ASCII.GetBytes(fileName);
                        int byteSent = sender.Send(messageSent);

                        byte[] messageReceived = new byte[2048];

                        int byteRecv = sender.Receive(messageReceived);
                        string msgReturn = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
                        Console.WriteLine(msgReturn);
                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }

                }

                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
    }
}
