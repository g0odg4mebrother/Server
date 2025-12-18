using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Timers;

public class ClientSession
{
    public string ClientId { get; set; }
    public DateTime ConnectTime { get; set; }
    public DateTime? DisconnectTime { get; set; }
    public int RequestCount { get; set; }
    public DateTime LastRequestTime { get; set; }
    public bool IsBlocked { get; set; }
}

public class CurrencyServer
{
    private TcpListener listener;
    private Dictionary<string, double> rates = new Dictionary<string, double>
    {
        {"USD", 1.0}, {"EUR", 0.92}, {"GBP", 0.79}, {"JPY", 147.5}, {"RUB", 92.5}
    };

    private Dictionary<string, string> users = new Dictionary<string, string>
    {
        {"admin", "123456"},
        {"user", "password"},
        {"test", "test123"}
    };

    private Dictionary<string, ClientSession> activeSessions = new Dictionary<string, ClientSession>();
    private object sessionLock = new object();
    private System.Timers.Timer cleanupTimer;

    private int maxClients = 10;
    private int maxRequestsPerSession = 5;
    private int requestBlockTime = 60000;

    public void Start()
    {
        Console.WriteLine("=== Сервер курсов валют ===");
        Console.WriteLine($"Максимум клиентов: {maxClients}");
        Console.WriteLine($"Максимум запросов на сессию: {maxRequestsPerSession}");
        Console.WriteLine("Доступные пользователи:");
        foreach (var user in users.Keys)
        {
            Console.WriteLine($"  Логин: {user}");
        }

        cleanupTimer = new System.Timers.Timer(30000); // 30 секунд
        cleanupTimer.Elapsed += CleanupOldSessions;
        cleanupTimer.AutoReset = true;
        cleanupTimer.Enabled = true;

        listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();

        Console.WriteLine($"Сервер запущен на порту 8888...");

        while (true)
        {
            try
            {
                var client = listener.AcceptTcpClient();

                lock (sessionLock)
                {
                    if (activeSessions.Count >= maxClients)
                    {
                        Log("Достигнут лимит подключений", ConsoleColor.Red);
                        SendRejection(client, "SERVER_FULL");
                        client.Close();
                        continue;
                    }
                }

                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
            catch (Exception ex)
            {
                Log($"Ошибка при подключении: {ex.Message}", ConsoleColor.Red);
            }
        }
    }

    private void SendRejection(TcpClient client, string reason)
    {
        try
        {
            var stream = client.GetStream();
            byte[] response = System.Text.Encoding.UTF8.GetBytes($"REJECT:{reason}");
            stream.Write(response, 0, response.Length);
        }
        catch { }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        string clientId = client.Client.RemoteEndPoint.ToString();
        ClientSession session = null;

        try
        {
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string authData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (!authData.Contains(":"))
                {
                    SendResponse(stream, "ERROR:Invalid auth format");
                    return;
                }

                string[] authParts = authData.Split(':');
                if (authParts.Length != 2)
                {
                    SendResponse(stream, "ERROR:Invalid auth format");
                    return;
                }

                string username = authParts[0];
                string password = authParts[1];

                if (!users.ContainsKey(username) || users[username] != password)
                {
                    SendResponse(stream, "ERROR:Invalid credentials");
                    Log($"Неудачная попытка входа: {username}", ConsoleColor.Yellow);
                    return;
                }

                lock (sessionLock)
                {
                    if (activeSessions.ContainsKey(username) && activeSessions[username].IsBlocked)
                    {
                        var blockedSession = activeSessions[username];
                        var timeSinceBlock = DateTime.Now - blockedSession.LastRequestTime;
                        if (timeSinceBlock.TotalMilliseconds < requestBlockTime)
                        {
                            SendResponse(stream, $"BLOCKED:{(int)(requestBlockTime - timeSinceBlock.TotalMilliseconds) / 1000}");
                            return;
                        }
                        else
                        {
                            activeSessions.Remove(username);
                        }
                    }
                }

                session = new ClientSession
                {
                    ClientId = clientId,
                    ConnectTime = DateTime.Now,
                    RequestCount = 0,
                    LastRequestTime = DateTime.Now,
                    IsBlocked = false
                };

                lock (sessionLock)
                {
                    activeSessions[username] = session;
                }

                SendResponse(stream, "AUTH_SUCCESS");
                Log($"Клиент подключился: {username} ({clientId})", ConsoleColor.Green);

                while (client.Connected)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    if (request.ToUpper() == "EXIT")
                    {
                        SendResponse(stream, "DISCONNECT");
                        break;
                    }

                    lock (sessionLock)
                    {
                        if (activeSessions.ContainsKey(username))
                        {
                            var currentSession = activeSessions[username];
                            currentSession.RequestCount++;
                            currentSession.LastRequestTime = DateTime.Now;

                            if (currentSession.RequestCount > maxRequestsPerSession)
                            {
                                currentSession.IsBlocked = true;
                                SendResponse(stream, $"LIMIT_EXCEEDED:{requestBlockTime / 1000}");
                                Log($"Лимит запросов превышен для: {username}", ConsoleColor.Yellow);
                                break;
                            }
                        }
                    }

                    string[] currencies = request.Split(' ');
                    if (currencies.Length == 2)
                    {
                        string from = currencies[0].ToUpper();
                        string to = currencies[1].ToUpper();

                        if (rates.ContainsKey(from) && rates.ContainsKey(to))
                        {
                            double rate = rates[from] / rates[to];
                            string response = $"{rate:F4}";
                            SendResponse(stream, response);
                            Log($"Запрос: {username} - {from}/{to} = {response}", ConsoleColor.Cyan);
                        }
                        else
                        {
                            SendResponse(stream, "ERROR:Invalid currency");
                        }
                    }
                    else
                    {
                        SendResponse(stream, "ERROR:Invalid format. Use: USD EUR");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Ошибка с клиентом {clientId}: {ex.Message}", ConsoleColor.Red);
        }
        finally
        {
            if (session != null)
            {
                session.DisconnectTime = DateTime.Now;
                Log($"Клиент отключился: {session.ClientId}. " +
                    $"Запросов: {session.RequestCount}. " +
                    $"Время работы: {(session.DisconnectTime - session.ConnectTime)?.TotalSeconds:F1} сек",
                    ConsoleColor.Gray);
            }
            client.Close();
        }
    }

    private void SendResponse(NetworkStream stream, string message)
    {
        byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(message);
        stream.Write(responseBytes, 0, responseBytes.Length);
    }

    private void CleanupOldSessions(object sender, ElapsedEventArgs e)
    {
        lock (sessionLock)
        {
            var toRemove = new List<string>();
            foreach (var kvp in activeSessions)
            {
                var session = kvp.Value;
                var timeSinceLast = DateTime.Now - session.LastRequestTime;

                if (timeSinceLast.TotalMinutes > 5)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var key in toRemove)
            {
                activeSessions.Remove(key);
            }

            if (toRemove.Count > 0)
            {
                Log($"Очищено {toRemove.Count} неактивных сессий", ConsoleColor.DarkGray);
            }
        }
    }

    private void Log(string message, ConsoleColor color = ConsoleColor.White)
    {
        lock (Console.Out)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            Console.ResetColor();
        }
    }
}

class Server
{
    static void Main()
    {
        CurrencyServer server = new CurrencyServer();
        server.Start();
    }
}