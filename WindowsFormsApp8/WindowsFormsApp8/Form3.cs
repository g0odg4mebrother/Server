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
    public partial class Form3 : Form
    {
        private string login;
        private string selectedUser;
        private string user1; 
        private string user2;
        public static List<string> messages = new List<string>();
        public Form3(string login, string selectedUser)
        {
            InitializeComponent();
           

            RefreshMessages();
        }
         private void RefreshMessages()
    {
        listBox1.Items.Clear();
        foreach (var msg in messages)
        {
            if (IsMessageForChat(msg))
            {
                listBox1.Items.Add(msg);
            }
        }
    }

    private bool IsMessageForChat(string message)
    {
        if (message.StartsWith("[" + user1 + "]") || message.StartsWith("[" + user2 + "]"))
        {
            return true;
        }
        return false;
    }
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
