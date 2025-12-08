using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        string serverIp = "127.0.0.1";
        int port = 8888;

        try
        {
            IPAddress ipAddress = IPAddress.Parse(serverIp);
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(new IPEndPoint(ipAddress, port));

            string message = "Привет, сервер!";
            byte[] msg = Encoding.UTF8.GetBytes(message);
            sender.Send(msg);

            byte[] buffer = new byte[1024];
            int bytesRec = sender.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRec);

            Console.WriteLine($"В {DateTime.Now:HH:mm} от [{serverIp}] получена строка: {response}");

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.ReadKey();
    }
}