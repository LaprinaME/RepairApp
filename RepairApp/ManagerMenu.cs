using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairApp
{
    public partial class ManagerMenu : Form
    {
        private string connectionString = @"Data Source=DESKTOP-DFJ77GS;Initial Catalog=RepairBD;Integrated Security=True;MultipleActiveResultSets=True";
        private SqlDataAdapter adapter;
        private DataSet ds;
        private string sql = "SELECT * FROM Comments"; // SQL-запрос для загрузки комментариев

        public ManagerMenu()
        {
            InitializeComponent();
            this.MaximizeBox = false; // Запрещаем максимизацию формы
        }

        private async void ManagerMenu_Load(object sender, EventArgs e)
        {
            await LoadDataAsync(); // Загрузка данных при запуске формы
        }

        // Асинхронный метод для загрузки данных из базы в DataGridView
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

        // button1 — добавление нового комментария
        private async void button1_Click(object sender, EventArgs e)
        {
            string message = Prompt.ShowDialog("Сообщение комментария:", "Добавление комментария");
            if (string.IsNullOrEmpty(message)) return;

            string masterID = Prompt.ShowDialog("ID мастера:", "Добавление комментария");
            if (string.IsNullOrEmpty(masterID)) return;

            string requestID = Prompt.ShowDialog("ID заявки:", "Добавление комментария");
            if (string.IsNullOrEmpty(requestID)) return;

            await AddCommentAsync(message, masterID, requestID);
            await LoadDataAsync(); // Обновляем данные после добавления
        }

        // Асинхронный метод для добавления нового комментария
        private async Task AddCommentAsync(string message, string masterID, string requestID)
        {
            string insertSql = @"
            INSERT INTO Comments (message, masterID, requestID) 
            VALUES (@message, @masterID, @requestID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                {
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

        // button3 — сохранение изменений в базе данных
        private async void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                adapter = new SqlDataAdapter(sql, connection);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                await Task.Run(() => adapter.Update(ds));
            }
            MessageBox.Show("Изменения сохранены.");
        }

        // button4 — обновление выбранного комментария
        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int commentID = (int)dataGridView1.SelectedRows[0].Cells[0].Value; // ID комментария в первом столбце

                string message = Prompt.ShowDialog("Сообщение комментария:", "Обновление комментария");
                if (string.IsNullOrEmpty(message)) return;

                string masterID = Prompt.ShowDialog("ID мастера:", "Обновление комментария");
                if (string.IsNullOrEmpty(masterID)) return;

                string requestID = Prompt.ShowDialog("ID заявки:", "Обновление комментария");
                if (string.IsNullOrEmpty(requestID)) return;

                await UpdateCommentAsync(commentID, message, masterID, requestID);
                await LoadDataAsync(); // Обновляем данные после изменения
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите комментарий для обновления.");
            }
        }

        // Асинхронный метод для обновления комментария
        private async Task UpdateCommentAsync(int commentID, string message, string masterID, string requestID)
        {
            string updateSql = @"
            UPDATE Comments 
            SET message = @message, masterID = @masterID, requestID = @requestID
            WHERE commentID = @commentID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@message", message);
                    updateCommand.Parameters.AddWithValue("@masterID", masterID);
                    updateCommand.Parameters.AddWithValue("@requestID", requestID);
                    updateCommand.Parameters.AddWithValue("@commentID", commentID);
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public class Prompt : Form
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 300,
                    Text = caption
                };
                Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(textLabel);
                prompt.ShowDialog();
                return textBox.Text;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Код для обработки кликов по ячейкам DataGridView
        }
    }
}
