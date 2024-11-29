using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairApp
{
    public partial class WorkerMenu : Form
    {
        private string connectionString = @"Data Source=DESKTOP-DFJ77GS;Initial Catalog=RepairBD;Integrated Security=True;MultipleActiveResultSets=True";
        private SqlDataAdapter adapter;
        private DataSet ds;
        private string sql = "SELECT * FROM Comments"; // SQL-запрос для загрузки комментариев

        public WorkerMenu()
        {
            InitializeComponent();
            this.Text = "Меню менеджера";
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Настроим DataGridView
            dataGridView1.Font = new Font("Comic Sans MS", 12);
            dataGridView1.BackgroundColor = Color.FromArgb(230, 230, 250);
            dataGridView1.GridColor = Color.FromArgb(180, 180, 200);

            // Настроим кнопку "Добавить комментарий"
            button1.Text = "Добавить комментарий";
            button1.Size = new Size(150, 40);
            button1.BackColor = Color.FromArgb(73, 140, 81);
            button1.ForeColor = Color.White;
            button1.Font = new Font("Comic Sans MS", 12);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(250, 200); // Устанавливаем начальное положение кнопки

            // Настроим кнопку "Удалить комментарий"
            button2.Text = "Удалить комментарий";
            button2.Size = new Size(150, 40);
            button2.BackColor = Color.FromArgb(73, 140, 81);
            button2.ForeColor = Color.White;
            button2.Font = new Font("Comic Sans MS", 12);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(250, 250); // Располагаем кнопку ниже с отступом


            // Добавляем элементы на форму
            dataGridView1.Dock = DockStyle.Fill;
            this.Controls.Add(dataGridView1);
        }

        private async void WorkerMenu_Load(object sender, EventArgs e)
        {
            // Заполнение данных в DataGridView с использованием асинхронной загрузки.
            this.commentsTableAdapter.Fill(this.repairBDDataSet2.Comments);
            await LoadDataAsync();  // Теперь это корректно вызывает асинхронный метод.
        }

        private async Task LoadDataAsync()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                adapter = new SqlDataAdapter(sql, connection);
                ds = new DataSet();
                await Task.Run(() => adapter.Fill(ds));
                dataGridView1.DataSource = ds.Tables[0];
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            // Запрашиваем данные в правильной последовательности и только один раз.
            string commentID = Prompt.ShowDialog("ID комментария:", "Добавление комментария");
            if (string.IsNullOrEmpty(commentID)) return;

            string message = Prompt.ShowDialog("Сообщение комментария:", "Добавление комментария");
            if (string.IsNullOrEmpty(message)) return;

            string masterID = Prompt.ShowDialog("ID мастера:", "Добавление комментария");
            if (string.IsNullOrEmpty(masterID)) return;

            string requestID = Prompt.ShowDialog("ID заявки:", "Добавление комментария");
            if (string.IsNullOrEmpty(requestID)) return;

            // Выполняем добавление в базу данных только после всех вводов
            await AddCommentAsync(commentID, message, masterID, requestID);

            // Обновляем данные только после успешного добавления
            await LoadDataAsync();
        }

        private async Task AddCommentAsync(string commentID, string message, string masterID, string requestID)
        {
            string insertSql = @"
    INSERT INTO Comments (commentID, message, masterID, requestID) 
    VALUES (@commentID, @message, @masterID, @requestID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                {
                    insertCommand.Parameters.AddWithValue("@commentID", commentID);
                    insertCommand.Parameters.AddWithValue("@message", message);
                    insertCommand.Parameters.AddWithValue("@masterID", masterID);
                    insertCommand.Parameters.AddWithValue("@requestID", requestID);
                    int numberOfInsertedRows = await insertCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"Добавлено объектов: {numberOfInsertedRows}");
                }
            }
        }



        // button2 — удаление выбранного комментария
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int commentID = (int)dataGridView1.SelectedRows[0].Cells[0].Value; // ID комментария в первом столбце
                await DeleteCommentAsync(commentID);
                await LoadDataAsync(); // Обновляем данные после удаления
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите комментарий для удаления.");
            }
        }

        // Асинхронный метод для удаления комментария
        private async Task DeleteCommentAsync(int commentID)
        {
            string deleteSql = "DELETE FROM Comments WHERE commentID = @commentID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@commentID", commentID);
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }
        }
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 150,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };

                Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 400 };
                TextBox inputBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 80 };

                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirmation);

                prompt.ShowDialog();
                return inputBox.Text;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
