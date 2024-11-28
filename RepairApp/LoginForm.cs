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
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Дополнительные настройки могут быть добавлены здесь
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // код обработки
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // код обработки
        }

        // Кнопка для входа
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
                    // Измените запрос под таблицу Users
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

        // Функция для выбора следующей формы в зависимости от роли
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
                case "4":
                    return new ClientMenu();  // Заказчик
                default:
                    MessageBox.Show("Неизвестный код роли", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
            }
        }
    }
}
