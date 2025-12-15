namespace client
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.startclients = new System.Windows.Forms.Button();
            this.messagepush = new System.Windows.Forms.Button();
            this.startbox = new System.Windows.Forms.TextBox();
            this.messagebox = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // startclients
            // 
            this.startclients.Location = new System.Drawing.Point(190, 48);
            this.startclients.Name = "startclients";
            this.startclients.Size = new System.Drawing.Size(155, 23);
            this.startclients.TabIndex = 0;
            this.startclients.Text = "подключится";
            this.startclients.UseVisualStyleBackColor = true;
            this.startclients.Click += new System.EventHandler(this.startclients_Click);
            // 
            // messagepush
            // 
            this.messagepush.Location = new System.Drawing.Point(190, 77);
            this.messagepush.Name = "messagepush";
            this.messagepush.Size = new System.Drawing.Size(155, 23);
            this.messagepush.TabIndex = 1;
            this.messagepush.Text = "отправить сообщение";
            this.messagepush.UseVisualStyleBackColor = true;
            this.messagepush.Click += new System.EventHandler(this.messagepush_Click);
            // 
            // startbox
            // 
            this.startbox.Location = new System.Drawing.Point(13, 50);
            this.startbox.Name = "startbox";
            this.startbox.Size = new System.Drawing.Size(171, 20);
            this.startbox.TabIndex = 2;
            // 
            // messagebox
            // 
            this.messagebox.Location = new System.Drawing.Point(13, 79);
            this.messagebox.Name = "messagebox";
            this.messagebox.Size = new System.Drawing.Size(171, 20);
            this.messagebox.TabIndex = 3;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(486, 61);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(237, 342);
            this.listBox1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.messagebox);
            this.Controls.Add(this.startbox);
            this.Controls.Add(this.messagepush);
            this.Controls.Add(this.startclients);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startclients;
        private System.Windows.Forms.Button messagepush;
        private System.Windows.Forms.TextBox startbox;
        private System.Windows.Forms.TextBox messagebox;
        private System.Windows.Forms.ListBox listBox1;
    }
}

