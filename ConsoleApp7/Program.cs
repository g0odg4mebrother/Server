using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TimeServer
{
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 9999;

        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(new IPEndPoint(ipAddress, port));
            listener.Listen(10);
            Console.WriteLine("Сервер времени запущен...");

            while (true)
            {
                Socket handler = listener.Accept();
                byte[] buffer = new byte[1024];
                int bytesRec = handler.Receive(buffer);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRec);

                string response = "";

                if (request.ToLower() == "дата")
                    response = DateTime.Now.ToString("dd.MM.yyyy");
                else if (request.ToLower() == "время")
                    response = DateTime.Now.ToString("HH:mm:ss");
                else
                    response = "Неизвестный запрос. Используйте 'дата' или 'время'";

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