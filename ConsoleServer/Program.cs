using ConsoleServer;
using GTSBizObjects;
using System;
using System.Net.Sockets;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverMode = System.Configuration.ConfigurationSettings.AppSettings["Server_Mode"];

            if (serverMode.ToLower() == "tcp")
            {
                TCPServer serve = new TCPServer();
            }
            else if (serverMode.ToLower() == "udp")
            {
                UDPServer serv = new UDPServer();
            }
            else
            {
                Utilities.writeLine("Config does not specify mode! Defaulting to TCP");
                TCPServer serve = new TCPServer();
            }
        }
    }
}
