using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RepairApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Вход в систему";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            
            textBox1.Font = new Font("Comic Sans MS", 12);
            textBox2.Font = new Font("Comic Sans MS", 12);

            textBox1.BackColor = Color.FromArgb(118, 227, 131);
            textBox2.BackColor = Color.FromArgb(118, 227, 131);

           
            textBox1.Size = new Size(155, 25);
            textBox2.Size = new Size(155, 25);

           
            this.Controls.Add(textBox1);
            this.Controls.Add(textBox2);

            
            button1.Text = "Войти";
            button1.Font = new Font("Comic Sans MS", 12);
            button1.BackColor = Color.FromArgb(73, 140, 81);
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Size = new Size(155, 45);

           
            label1.Text = "Введите логин:";
            label1.Font = new Font("Comic Sans MS", 12);
            label1.ForeColor = Color.Black;

            label2.Text = "Введите пароль:";
            label2.Font = new Font("Comic Sans MS", 12);
            label2.ForeColor = Color.Black;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            string connectionString = @"Data Source=DESKTOP-DFJ77GS;Initial Catalog=RepairBD;Integrated Security=True"; // Подключение к вашей БД RepairBD

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    
                    string query = "SELECT roleID FROM Users WHERE login = @login AND password = @password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        var roleCode = command.ExecuteScalar();

                        if (roleCode != null)
                        {
                            Form nextForm = GetNextFormByRole(roleCode.ToString());
                            if (nextForm != null)
                            {
                                nextForm.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Form GetNextFormByRole(string roleCode)
        {
            switch (roleCode)
            {
                case "1":
                    return new ManagerMenu();  // Менеджер
                case "2":
                    return new WorkerMenu();  // Мастер
                case "3":
                    return new OperatorMenu();  // Оператор
                default:
                    MessageBox.Show("Неизвестный код роли", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
            }
        }
    }
}
