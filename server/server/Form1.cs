using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private CancellationTokenSource cts;
        private Random rand = new Random();
        
        private readonly string[] responses = {
            "Понял вас.",
            "Интересно.",
            "Могу я помочь вам?",
            "Не совсем понятно.",
            "Давайте ещё раз.",
            "Хорошо.",
            "Это интересно.",
            "Учитываю.",
            "Что ещё?",
            "Пожалуй, да."
        };

        public Form1()
        {
            InitializeComponent();
        }

        private async void startserver_Click(object sender, EventArgs e)
        {
            int port;
            if (!int.TryParse(numerport.Text, out port))
            {
                MessageBox.Show("Введите корректный порт");
                return;
            }

            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            AppendText($"Сервер запущен на порту {port}. Ожидание клиента...");

            cts = new CancellationTokenSource();

            try
            {
                client = await server.AcceptTcpClientAsync();
                AppendText("Клиент подключился.");

                var networkStream = client.GetStream();
                reader = new StreamReader(networkStream, Encoding.UTF8);
                writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };

                _ = Task.Run(() => ReceiveMessagesAsync(cts.Token));
            }
            catch (Exception ex)
            {
                AppendText($"Ошибка: {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && client.Connected)
                {
                    string message = await reader.ReadLineAsync();
                    if (message == null)
                        break;

                    AppendText($"Клиент: {message}");

                    if (message.Trim().Equals("Bye", StringComparison.OrdinalIgnoreCase))
                    {
                        AppendText("Соединение закрыто по команде клиента.");
                        StopServer();
                        break;
                    }

                    if (message.ToLower().Contains("ответ"))
                    {
                        string response = GetRandomResponse();
                        await SendMessageAsync($"Сервер: {response}");
                        AppendText($"Сервер: {response}");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendText($"Ошибка при получении данных: {ex.Message}");
            }
        }

        private string GetRandomResponse()
        {
            int index = rand.Next(responses.Length);
            return responses[index];
        }

        private async Task SendMessageAsync(string message)
        {
            if (writer != null && client != null && client.Connected)
            {
                await writer.WriteLineAsync(message);
            }
        }

        private void stopserver_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            try
            {
                cts?.Cancel();
                reader?.Close();
                writer?.Close();
                client?.Close();
                server?.Stop();
                AppendText("Сервер остановлен.");
            }
            catch (Exception ex)
            {
                AppendText($"Ошибка при остановке: {ex.Message}");
            }
        }

        private void AppendText(string message)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new Action(() => {
                    listBox1.Items.Add(message);
                }));
            }
            else
            {
                listBox1.Items.Add(message);
            }
        }

        private async void otpravkasoobshenia_Click(object sender, EventArgs e)
        {
            string messageToSend = vvodtexta.Text;
            if (string.IsNullOrWhiteSpace(messageToSend))
            {
                MessageBox.Show("Введите сообщение для отправки.");
                return;
            }

            await SendMessageAsync($"Сервер: {messageToSend}");
            AppendText($"Вы: {messageToSend}");
            vvodtexta.Clear();
        }
    }
}
