using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class QuoteClient
{
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
    private string clientId;

    public QuoteClient(string serverIp, int port)
    {
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
        udpClient.Client.ReceiveTimeout = 5000;
    }

    public bool Authenticate(string username, string password)
    {
        string authMessage = $"AUTH:{username}:{password}";
        byte[] data = Encoding.UTF8.GetBytes(authMessage);
        udpClient.Send(data, data.Length, serverEndPoint);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        byte[] responseData = udpClient.Receive(ref sender);
        string response = Encoding.UTF8.GetString(responseData);

        return response.StartsWith("AUTH_OK");
    }

    public string GetQuote()
    {
        byte[] data = Encoding.UTF8.GetBytes("GET_QUOTE");
        udpClient.Send(data, data.Length, serverEndPoint);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        byte[] responseData = udpClient.Receive(ref sender);
        string response = Encoding.UTF8.GetString(responseData);

        if (response.StartsWith("QUOTE:"))
        {
            return response.Substring(6);
        }
        else if (response == "LIMIT_REACHED:Достигнут лимит цитат. Соединение разрывается.")
        {
            Disconnect();
            return "Достигнут лимит цитат. Соединение закрыто.";
        }
        else if (response == "SERVER_BUSY:Сервер перегружен. Попробуйте позже.")
        {
            return "Сервер перегружен. Попробуйте позже.";
        }

        return "Ошибка при получении цитаты";
    }

    public void Disconnect()
    {
        byte[] data = Encoding.UTF8.GetBytes("DISCONNECT");
        udpClient.Send(data, data.Length, serverEndPoint);

        try
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            udpClient.Receive(ref sender);
        }
        catch { }

        udpClient.Close();
    }
}

public class ConsoleUI
{
    private QuoteClient client;

    public void Run()
    {
        Console.WriteLine("=== Генератор цитат ===");
        Console.Write("Введите IP сервера (по умолчанию 127.0.0.1): ");
        string ip = Console.ReadLine();
        if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

        Console.Write("Введите порт сервера (по умолчанию 12345): ");
        if (!int.TryParse(Console.ReadLine(), out int port)) port = 12345;

        client = new QuoteClient(ip, port);

        Console.Write("Введите имя пользователя: ");
        string username = Console.ReadLine();

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        if (!client.Authenticate(username, password))
        {
            Console.WriteLine("Ошибка аутентификации!");
            return;
        }

        Console.WriteLine("Аутентификация успешна! Введите 'q' для выхода.");

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nНажмите Enter для получения цитаты или 'q' для выхода...");
            var input = Console.ReadLine();

            if (input?.ToLower() == "q")
            {
                running = false;
            }
            else
            {
                string quote = client.GetQuote();
                Console.WriteLine($"\nЦитата: {quote}");
            }
        }

        client.Disconnect();
        Console.WriteLine("Соединение закрыто.");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        ConsoleUI ui = new ConsoleUI();
        ui.Run();
    }
}