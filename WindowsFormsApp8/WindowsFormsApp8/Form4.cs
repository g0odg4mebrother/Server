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
    public partial class Form4 : Form
    {
        public List<string> users = new List<string>() { "User1", "User2", "User3" };
        private Dictionary<string, DateTime?> blockedUsers = new Dictionary<string, DateTime?>();
        private List<string> bannedWords = new List<string>() { "плохое_слово", "запрещено" };

        public Form4()
        {
            InitializeComponent();

            listBox1.DataSource = users;
        }

        private void RefreshUserList()
        {
            listBox1.DataSource = null;
            listBox1.DataSource = users;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedUser = listBox1.SelectedItem as string;
            if (selectedUser != null)
            {
                if (users.Contains(selectedUser))
                {
                    users.Remove(selectedUser);
                    RefreshUserList();
                    MessageBox.Show($"{selectedUser} удалён навечно");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selectedUser = listBox1.SelectedItem as string;
            if (selectedUser != null)
            {
                if (blockedUsers.ContainsKey(selectedUser))
                {
                    blockedUsers[selectedUser] = null;
                    MessageBox.Show($"{selectedUser} разблокирован");
                }
                else
                {
                    MessageBox.Show($"{selectedUser} не заблокирован");
                }
            }
        }

        private void buttonShowUsers_Click(object sender, EventArgs e)
        {
            RefreshUserList();
        }

        private void BlockUser(string username, TimeSpan duration)
        {
            blockedUsers[username] = DateTime.Now.Add(duration);
            MessageBox.Show($"{username} заблокирован на {duration.TotalMinutes} минут");
        }
    }
}
