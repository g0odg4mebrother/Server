using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class AsyncServer
{
    static async Task Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 8888;

        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(new IPEndPoint(ipAddress, port));
            listener.Listen(10);
            Console.WriteLine("Асинхронный сервер запущен...");

            while (true)
            {
                Socket handler = await listener.AcceptAsync();
                _ = Task.Run(() => HandleClientAsync(handler));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static async Task HandleClientAsync(Socket handler)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRec = await handler.ReceiveAsync(buffer, SocketFlags.None);
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);

            Console.WriteLine($"В {DateTime.Now:HH:mm} от [{((IPEndPoint)handler.RemoteEndPoint).Address}] получена строка: {data}");

            string response = "Привет, клиент!";
            byte[] msg = Encoding.UTF8.GetBytes(response);
            await handler.SendAsync(msg, SocketFlags.None);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
        }
    }
}