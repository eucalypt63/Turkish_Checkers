namespace Client
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.RoomNameJoin = new System.Windows.Forms.TextBox();
            this.NameText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(82, 157);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 51);
            this.button1.TabIndex = 0;
            this.button1.Text = "Создать Игру";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(273, 157);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 51);
            this.button2.TabIndex = 1;
            this.button2.Text = "Присоедениться к игре";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // RoomNameJoin
            // 
            this.RoomNameJoin.Location = new System.Drawing.Point(273, 125);
            this.RoomNameJoin.Name = "RoomNameJoin";
            this.RoomNameJoin.Size = new System.Drawing.Size(146, 26);
            this.RoomNameJoin.TabIndex = 2;
            // 
            // NameText
            // 
            this.NameText.Location = new System.Drawing.Point(82, 125);
            this.NameText.Name = "NameText";
            this.NameText.Size = new System.Drawing.Size(141, 26);
            this.NameText.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Nick Name:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(539, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NameText);
            this.Controls.Add(this.RoomNameJoin);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox RoomNameJoin;
        private System.Windows.Forms.TextBox NameText;
        private System.Windows.Forms.Label label1;
    }
}

