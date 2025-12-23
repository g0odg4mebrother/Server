using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp8
{
    public partial class Form2 : Form
    {
        private string login;
        private string password;
        private System.Windows.Forms.Timer updateTimer;

        private List<string> bannedWords = new List<string> { "плохое_слово", "запрещено" };

        public Form2(string login, string password)
        {
            InitializeComponent();
            this.login = login;
            this.password = password;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            RefreshMessages();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            RefreshMessages();
        }

        private void RefreshMessages()
        {
            listBox1.Items.Clear();

            foreach (var msg in Form1.messages)
            {
                listBox1.Items.Add(msg);
            }
        }
**********
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }
        }

        private string CensorMessage(string message, out bool isCensored)
        {
            isCensored = false;
            string[] words = message.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                foreach (var badWord in bannedWords)
                {
                    if (string.Equals(words[i], badWord, StringComparison.OrdinalIgnoreCase))
                    {
                        words[i] = "###";
                        isCensored = true;
                    }
                }
            }
            return string.Join(" ", words);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                bool censored = false;
                string censoredMsg = CensorMessage(message, out censored);
                string formattedMessage = $"[{login}]: {censoredMsg}";

                Form1.messages.Add(formattedMessage);
                listBox1.Items.Add(formattedMessage);
                textBox1.Clear();

                if (censored)
                {
                    MessageBox.Show($"Пользователь {login} послал запрещённое слово");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите сообщение.", "Предупреждение");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedUser = listBox1.SelectedItem.ToString();
                Form3 chatForm = new Form3(login, selectedUser);
                chatForm.Show();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для чата");
            }
        }
    }
}
