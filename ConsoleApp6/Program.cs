using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 8888;

        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(new IPEndPoint(ipAddress, port));
            listener.Listen(10);
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                Socket handler = listener.Accept();
                byte[] buffer = new byte[1024];
                int bytesRec = handler.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);

                Console.WriteLine($"В {DateTime.Now:HH:mm} от [{((IPEndPoint)handler.RemoteEndPoint).Address}] получена строка: {data}");

                string response = "Привет, клиент!";
                byte[] msg = Encoding.UTF8.GetBytes(response);
                handler.Send(msg);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}