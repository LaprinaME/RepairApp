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

            // Настраиваем кнопку авторизации
            button1.Text = "Авторизация";
            button1.Location = new Point(50, 50);
            button1.Size = new Size(200, 40);
            button1.Click += button1_Click;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Открываем форму авторизации
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            loginForm.BringToFront();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            // Центрируем кнопки
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
