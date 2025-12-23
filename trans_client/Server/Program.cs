using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Console;
class Server
{
    static TcpListener listener;
    static List<ClientHandler> clients = new List<ClientHandler>();
    static object lockObj = new object();

    static void Main()
    {
        int port = 5000;
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        WriteLine($"Сервер запущен на порту {port}.");

        while (true)
        {
            try
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                ClientHandler client = new ClientHandler(tcpClient);
                lock (lockObj)
                {
                    clients.Add(client);
                }
                Thread thread = new Thread(client.Process);
                thread.IsBackground = true; 
                thread.Start();
            }
            catch (Exception ex)
            {
                WriteLine($"Ошибка подключения клиента: {ex.Message}");
            }
        }
    }

    public static void Broadcast(string message, string messageType = null, bool isEmergency = false)
    {
        lock (lockObj)
        {
            List<ClientHandler> отключённыеКлиенты = new List<ClientHandler>();
            foreach (var client in clients)
            {
                try
                {
                    if (isEmergency || (messageType != null && client.SubscribedTypes.Contains(messageType)))
                    {
                        client.SendMessage(message);
                    }
                }
                catch
                {
                    отключённыеКлиенты.Add(client);
                }
            }
            foreach (var dc in отключённыеКлиенты)
            {
                clients.Remove(dc);
            }
        }
    }

    public static void RemoveClient(ClientHandler client)
    {
        lock (lockObj)
        {
            clients.Remove(client);
        }
    }

    public static void SendToAll(string message)
    {
        Broadcast($"Внимание: {message}", null, true);
    }
}

class ClientHandler
{
    TcpClient client;
    NetworkStream stream;
    public HashSet<string> SubscribedTypes { get; private set; } = new HashSet<string>();

    public ClientHandler(TcpClient client)
    {
        this.client = client;
        stream = client.GetStream();
    }

    public void Process()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int byteCount;

            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                HandleMessage(message);
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            Close();
        }
    }

    public void HandleMessage(string message)
    {
        string[] parts = message.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string command = parts[0].ToUpper();

        switch (command)
        {
            case "SUBSCRIBE":
                if (parts.Length > 1)
                {
                    SubscribedTypes.Add(parts[1]);
                    SendMessage($"Подписка на {parts[1]} оформлена");
                }
                break;
            case "UNSUBSCRIBE":
                if (parts.Length > 1)
                {
                    SubscribedTypes.Remove(parts[1]);
                    SendMessage($"Отписка от {parts[1]} завершена");
                }
                break;
            case "MESSAGE":
                if (parts.Length > 1)
                {
                    Server.Broadcast($"Сообщение: {parts[1]}");
                }
                break;
            case "EMERGENCY":
                if (parts.Length > 1)
                {
                    Server.SendToAll($"Внимание! Срочное сообщение: {parts[1]}");
                }
                break;
            case "REMOVE":
                Close();
                break;
            default:
                SendMessage("Неизвестная команда");
                break;
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch
        {
            Close();
        }
    }

    public void Close()
    {
        Server.RemoveClient(this);
        try
        {
            stream.Close();
            client.Close();
        }
        catch { }
    }
}