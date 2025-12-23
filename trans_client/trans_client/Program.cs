using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
class Client
{
    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;

    public async Task RunAsync()
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 5000);
            stream = client.GetStream();
            isConnected = true;

            WriteLine("Подключение к серверу установлено.");

            var receiveTask = ReceiveMessagesAsync();

            await SendCommandsAsync();

            await receiveTask;
        }
        catch (Exception ex)
        {
            WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            Disconnect();
        }
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024];
        try
        {
            while (isConnected)
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0)
                {
                    WriteLine("Разъединено от сервера.");
                    break;
                }
                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                WriteLine($"[Сервер]: {message}");
            }
        }
        catch (Exception)
        {
            if (isConnected)
            {
                WriteLine("Потеря соединения.");
            }
        }
        finally
        {
            isConnected = false;
        }
    }

    private async Task SendCommandsAsync()
    {
        WriteLine("Введите команды:");
        WriteLine("Команды:");
        WriteLine("  SUBSCRIBE <тип>");
        WriteLine("  UNSUBSCRIBE <тип>");
        WriteLine("  MESSAGE <текст>");
        WriteLine("  EMERGENCY <текст>");
        WriteLine("  /exit");
        WriteLine("Чтобы отписаться, введите команду снова");
        WriteLine("Для отправки сообщения введите MESSAGE или EMERGENCY");
        WriteLine();

        while (isConnected)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
            {
                await SendMessageAsync("REMOVE");
                break;
            }
            else
            {
                await SendMessageAsync(input);
            }
        }
    }

    private async Task SendMessageAsync(string message)
    {
        if (stream == null || !stream.CanWrite) return;
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
    }

    private void Disconnect()
    {
        isConnected = false;
        try
        {
            stream?.Close();
            client?.Close();
        }
        catch { }
        WriteLine("Отключено от сервера.");
    }

    static async Task Main(string[] args)
    {
        Client client = new Client();
        await client.RunAsync();
    }
}