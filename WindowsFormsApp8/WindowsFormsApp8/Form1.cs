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
    public partial class Form1 : Form
    {
        public static List<string> messages = new List<string>();
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
            {
                Form2 form2 = new Form2(textBox1.Text, textBox2.Text);
                form2.Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль перед открытием следующей формы.");
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form4  form4 =  new Form4();
            form4.Show();
        }
    }
}
