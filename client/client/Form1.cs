using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Task receiveTask; 

        public Form1()
        {
            InitializeComponent();
        }

        private async void startclients_Click(object sender, EventArgs e)
        {
            string ipAddress = "127.0.0.1";
            int port;
            if (!int.TryParse(startbox.Text, out port))
            {
                MessageBox.Show("Введите корректный порт");
                return;
            }

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ipAddress, port);
                var networkStream = client.GetStream();
                reader = new StreamReader(networkStream, Encoding.UTF8);
                writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true };

                MessageBox.Show("Подключено к серверу");

                receiveTask = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        private async void messagepush_Click(object sender, EventArgs e)
        {
            if (writer != null && client != null && client.Connected)
            {
                string messageToSend = messagebox.Text;
                try
                {
                    await writer.WriteLineAsync(messageToSend);
                    messagebox.Clear();

                    if (messageToSend.Trim().Equals("Bye", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Соединение закрыто по команде.");
                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Сначала подключитесь к серверу");
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            try
            {
                while (client.Connected)
                {
                    string message = await reader.ReadLineAsync();
                    if (message == null)
                        break;

                    Invoke(new Action(() =>
                    {
                        listBox1.Items.Add($"Сервер: {message}");
                    }));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    listBox1.Items.Add($"Ошибка: {ex.Message}");
                }));
            }
        }
    }
}