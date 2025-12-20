using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class QuoteServer
{
    private UdpClient udpServer;
    private Dictionary<string, ClientInfo> clients = new Dictionary<string, ClientInfo>();
    private Dictionary<string, UserCredentials> validUsers = new Dictionary<string, UserCredentials>();
    private List<string> quotes = new List<string>();
    private int maxClients = 5;
    private int maxQuotesPerUser = 3;
    private object lockObj = new object();

    public QuoteServer(int port)
    {
        udpServer = new UdpClient(port);
        InitializeData();
    }

    private void InitializeData()
    {
        quotes.AddRange(new[]
        {
            "Жизнь - это то, что происходит с тобой, пока ты строишь другие планы.",
            "Будь изменением, которое ты хочешь видеть в мире.",
            "Успех - это способность переходить от одной неудачи к другой без потери энтузиазма.",
            "Единственный способ сделать великую работу - любить то, что ты делаешь.",
            "Не бойтесь совершенства. Вам его никогда не достичь."
        });

        validUsers.Add("user1", new UserCredentials("user1", "pass1"));
        validUsers.Add("user2", new UserCredentials("user2", "pass2"));
        validUsers.Add("admin", new UserCredentials("admin", "admin123"));
    }

    public void Start()
    {
        Console.WriteLine($"Сервер запущен на порту {((IPEndPoint)udpServer.Client.LocalEndPoint).Port}");

        while (true)
        {
            IPEndPoint clientEndPoint = null;
            byte[] data = udpServer.Receive(ref clientEndPoint);
            string clientId = $"{clientEndPoint.Address}:{clientEndPoint.Port}";

            ThreadPool.QueueUserWorkItem(_ => ProcessClient(data, clientEndPoint, clientId));
        }
    }

    private void ProcessClient(byte[] data, IPEndPoint clientEndPoint, string clientId)
    {
        string message = Encoding.UTF8.GetString(data);

        lock (lockObj)
        {
            if (clients.Count >= maxClients && !clients.ContainsKey(clientId))
            {
                SendMessage(clientEndPoint, "SERVER_BUSY:Сервер перегружен. Попробуйте позже.");
                Log($"Клиент {clientId} отклонен - сервер перегружен");
                return;
            }

            if (!clients.ContainsKey(clientId))
            {
                var parts = message.Split(':');
                if (parts.Length != 3 || parts[0] != "AUTH")
                {
                    SendMessage(clientEndPoint, "ERROR:Неверный формат аутентификации");
                    return;
                }

                string username = parts[1];
                string password = parts[2];

                if (!validUsers.ContainsKey(username) || validUsers[username].Password != password)
                {
                    SendMessage(clientEndPoint, "ERROR:Неверные учетные данные");
                    Log($"Неудачная аутентификация: {clientId}");
                    return;
                }

                clients[clientId] = new ClientInfo
                {
                    Username = username,
                    ConnectionTime = DateTime.Now,
                    QuoteCount = 0,
                    EndPoint = clientEndPoint
                };

                Log($"Клиент подключился: {username} ({clientId}) в {DateTime.Now}");
                SendMessage(clientEndPoint, "AUTH_OK:Добро пожаловать!");
                return;
            }

            var client = clients[clientId];

            if (client.QuoteCount >= maxQuotesPerUser)
            {
                SendMessage(clientEndPoint, "LIMIT_REACHED:Достигнут лимит цитат. Соединение разрывается.");
                Log($"Клиент {client.Username} достиг лимита цитат");
                clients.Remove(clientId);
                return;
            }

            if (message == "GET_QUOTE")
            {
                if (quotes.Count == 0)
                {
                    SendMessage(clientEndPoint, "ERROR:Нет доступных цитат");
                    return;
                }

                Random rnd = new Random();
                int index = rnd.Next(quotes.Count);
                string quote = quotes[index];

                client.QuoteCount++;
                client.Quotes.Add(quote);

                SendMessage(clientEndPoint, $"QUOTE:{quote}");
                Log($"Отправлена цитата клиенту {client.Username}. Цитат отправлено: {client.QuoteCount}");
            }
            else if (message == "DISCONNECT")
            {
                client.DisconnectionTime = DateTime.Now;
                Log($"Клиент отключился: {client.Username} в {client.DisconnectionTime}");
                clients.Remove(clientId);
                SendMessage(clientEndPoint, "DISCONNECT_OK:До свидания!");
            }
        }
    }

    private void SendMessage(IPEndPoint endPoint, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpServer.Send(data, data.Length, endPoint);
    }

    private void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}

public class ClientInfo
{
    public string Username { get; set; }
    public DateTime ConnectionTime { get; set; }
    public DateTime? DisconnectionTime { get; set; }
    public List<string> Quotes { get; set; } = new List<string>();
    public int QuoteCount { get; set; }
    public IPEndPoint EndPoint { get; set; }
}

public class UserCredentials
{
    public string Username { get; }
    public string Password { get; }

    public UserCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        QuoteServer server = new QuoteServer(12345);
        server.Start();
    }
}