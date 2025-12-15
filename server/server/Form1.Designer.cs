namespace server
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
            this.startserver = new System.Windows.Forms.Button();
            this.stopserver = new System.Windows.Forms.Button();
            this.otpravkasoobshenia = new System.Windows.Forms.Button();
            this.soobchenie = new System.Windows.Forms.TextBox();
            this.numerport = new System.Windows.Forms.TextBox();
            this.vvodtexta = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // startserver
            // 
            this.startserver.Location = new System.Drawing.Point(12, 12);
            this.startserver.Name = "startserver";
            this.startserver.Size = new System.Drawing.Size(75, 23);
            this.startserver.TabIndex = 0;
            this.startserver.Text = "старт ";
            this.startserver.UseVisualStyleBackColor = true;
            this.startserver.Click += new System.EventHandler(this.startserver_Click);
            // 
            // stopserver
            // 
            this.stopserver.Location = new System.Drawing.Point(93, 12);
            this.stopserver.Name = "stopserver";
            this.stopserver.Size = new System.Drawing.Size(75, 23);
            this.stopserver.TabIndex = 1;
            this.stopserver.Text = "стоп";
            this.stopserver.UseVisualStyleBackColor = true;
            this.stopserver.Click += new System.EventHandler(this.stopserver_Click);
            // 
            // otpravkasoobshenia
            // 
            this.otpravkasoobshenia.Location = new System.Drawing.Point(253, 137);
            this.otpravkasoobshenia.Name = "otpravkasoobshenia";
            this.otpravkasoobshenia.Size = new System.Drawing.Size(188, 23);
            this.otpravkasoobshenia.TabIndex = 2;
            this.otpravkasoobshenia.Text = "отправить сообщение";
            this.otpravkasoobshenia.UseVisualStyleBackColor = true;
            this.otpravkasoobshenia.Click += new System.EventHandler(this.otpravkasoobshenia_Click);
            // 
            // soobchenie
            // 
            this.soobchenie.Location = new System.Drawing.Point(12, 113);
            this.soobchenie.Name = "soobchenie";
            this.soobchenie.Size = new System.Drawing.Size(235, 20);
            this.soobchenie.TabIndex = 3;
            // 
            // numerport
            // 
            this.numerport.Location = new System.Drawing.Point(13, 87);
            this.numerport.Name = "numerport";
            this.numerport.Size = new System.Drawing.Size(234, 20);
            this.numerport.TabIndex = 4;
            // 
            // vvodtexta
            // 
            this.vvodtexta.Location = new System.Drawing.Point(12, 139);
            this.vvodtexta.Name = "vvodtexta";
            this.vvodtexta.Size = new System.Drawing.Size(235, 20);
            this.vvodtexta.TabIndex = 5;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(485, 33);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(303, 381);
            this.listBox1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.vvodtexta);
            this.Controls.Add(this.numerport);
            this.Controls.Add(this.soobchenie);
            this.Controls.Add(this.otpravkasoobshenia);
            this.Controls.Add(this.stopserver);
            this.Controls.Add(this.startserver);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startserver;
        private System.Windows.Forms.Button stopserver;
        private System.Windows.Forms.Button otpravkasoobshenia;
        private System.Windows.Forms.TextBox soobchenie;
        private System.Windows.Forms.TextBox numerport;
        private System.Windows.Forms.TextBox vvodtexta;
        private System.Windows.Forms.ListBox listBox1;
    }
}

