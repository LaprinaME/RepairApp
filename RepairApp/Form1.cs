using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Вход в систему";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            
            this.BackColor = Color.White;
            this.Font = new Font("Comic Sans MS", 12);

            
            button1.Text = "Авторизация";
            button1.Location = new Point(50, 50);
            button1.Size = new Size(200, 40);
            button1.BackColor = Color.FromArgb(73, 140, 81);
            button1.ForeColor = Color.White;
            button1.Click += button1_Click;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            loginForm.BringToFront();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            
            CenterButtons();
        }

        private void CenterButtons()
        {
            button1.Location = new Point(
                (this.ClientSize.Width - button1.Width) / 2,
                (this.ClientSize.Height - button1.Height) / 2 - 30
            );
        }
    }
}
