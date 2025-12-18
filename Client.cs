using System;
using System.Net.Sockets;

public class CurrencyClient
{
    private string username;
    private string password;

    public CurrencyClient(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public void Start()
    {
        Console.WriteLine($"Клиент запущен. Пользователь: {username}");
        Console.WriteLine("Доступные валюты: USD, EUR, GBP, JPY, RUB");
        Console.WriteLine("Пример запроса: USD EUR");
        Console.WriteLine("Для отключения введите 'exit'");
        Console.WriteLine();

        while (true)
        {
            try
            {
                using (TcpClient client = new TcpClient("localhost", 8888))
                using (NetworkStream stream = client.GetStream())
                {
                    string authData = $"{username}:{password}";
                    byte[] authBytes = System.Text.Encoding.UTF8.GetBytes(authData);
                    stream.Write(authBytes, 0, authBytes.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string authResponse = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (authResponse == "ERROR:Invalid credentials")
                    {
                        Console.WriteLine("Ошибка: Неверные учетные данные");
                        return;
                    }
                    else if (authResponse.StartsWith("BLOCKED:"))
                    {
                        string[] parts = authResponse.Split(':');
                        int seconds = int.Parse(parts[1]);
                        Console.WriteLine($"Сервер временно заблокирован. Попробуйте через {seconds} секунд");
                        return;
                    }
                    else if (authResponse == "ERROR:Invalid auth format")
                    {
                        Console.WriteLine("Ошибка формата аутентификации");
                        return;
                    }

                    Console.WriteLine("Успешно подключено к серверу!");
                    Console.WriteLine();

                    while (true)
                    {
                        Console.Write("Введите пару валют > ");
                        string input = Console.ReadLine().Trim();

                        if (input.ToLower() == "exit")
                        {
                            byte[] exitBytes = System.Text.Encoding.UTF8.GetBytes("EXIT");
                            stream.Write(exitBytes, 0, exitBytes.Length);
                            Console.WriteLine("Отключение...");
                            break;
                        }

                        byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(input);
                        stream.Write(requestBytes, 0, requestBytes.Length);

                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string response = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        if (response.StartsWith("LIMIT_EXCEEDED:"))
                        {
                            string[] parts = response.Split(':');
                            int seconds = int.Parse(parts[1]);
                            Console.WriteLine($"Лимит запросов превышен. Новая сессия через {seconds} секунд");
                            break;
                        }
                        else if (response.StartsWith("ERROR:"))
                        {
                            Console.WriteLine($"Ошибка: {response.Substring(6)}");
                        }
                        else
                        {
                            Console.WriteLine($"Курс: {response}");
                        }
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
            {
                Console.WriteLine("Не удалось подключиться к серверу. Убедитесь, что сервер запущен.");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.Write("Повторить подключение? (y/n): ");
            if (Console.ReadLine().ToLower() != "y") break;
        }

        Console.WriteLine("Работа клиента завершена");
    }
}

class Client
{
    static void Main()
    {
        Console.WriteLine("=== Клиент курсов валют ===");

        Console.Write("Введите имя пользователя: ");
        string username = Console.ReadLine();

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        CurrencyClient client = new CurrencyClient(username, password);
        client.Start();
    }
}