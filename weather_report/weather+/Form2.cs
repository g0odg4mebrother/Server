using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace weather_
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string filmName = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(filmName))
            {
                MessageBox.Show("Пожалуйста, введите название фильма");
                return;
            }

            string apiKey = "";//вставить свой апикей
            string url = $"http://www.omdbapi.com/?t={Uri.EscapeDataString(filmName)}&apikey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string jsonResponse = await client.GetStringAsync(url);
                    dynamic data = JsonConvert.DeserializeObject(jsonResponse);

                    if (data.Response == "False")
                    {
                        listBox1.Items.Clear();
                        listBox1.Items.Add($"Ошибка: {data.Error}");
                        return;
                    }

                    listBox1.Items.Clear();
                    listBox1.Items.Add($"Название: {data.Title}");
                    listBox1.Items.Add($"Год: {data.Year}");
                    listBox1.Items.Add($"Режиссер: {data.Director}");
                    listBox1.Items.Add($"Жанр: {data.Genre}");
                    listBox1.Items.Add($"Рейтинг IMDb: {data.imdbRating}");
                    listBox1.Items.Add($"Описание: {data.Plot}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка запроса: {ex.Message}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string toAddress = textBox2.Text.Trim();   
            string subject = textBox3.Text.Trim();    
            string bodyText = textBox3.Text.Trim();   

            string attachmentPath = @"results.txt";

            if (!System.IO.File.Exists(attachmentPath))
            {
                System.IO.File.WriteAllLines(attachmentPath, listBox1.Items.Cast<object>().Select(o => o.ToString()));
            }

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("your_email@gmail.com");

            mail.To.Add(toAddress);

            mail.Subject = subject;
            mail.Body = bodyText;

            mail.Attachments.Add(new Attachment(attachmentPath));

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("your_email@gmail.com", "your_app_password"),
                EnableSsl = true
            };

            try
            {
                smtp.Send(mail);
                MessageBox.Show("Письмо успешно отправлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке письма: {ex.Message}");
            }
        }
    }
}
