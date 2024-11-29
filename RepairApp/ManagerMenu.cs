using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairApp
{
    public partial class ManagerMenu : Form
    {
        private string connectionString = @"Data Source=DESKTOP-DFJ77GS;Initial Catalog=RepairBD;Integrated Security=True;MultipleActiveResultSets=True";
        private SqlDataAdapter adapter;
        private DataSet ds;
        private string sql = "SELECT * FROM Comments"; 

        public ManagerMenu()
        {
            InitializeComponent();
            this.Text = "Меню менеджера";
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            
            dataGridView1.Font = new Font("Comic Sans MS", 12);
            dataGridView1.BackgroundColor = Color.FromArgb(189, 236, 182);
            dataGridView1.GridColor = Color.FromArgb(180, 180, 200);

            
            button1.Text = "Добавить комментарий";
            button1.Size = new Size(150, 40);
            button1.BackColor = Color.FromArgb(73, 140, 81);
            button1.ForeColor = Color.White;
            button1.Font = new Font("Comic Sans MS", 12);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(250, 200);


            button2.Text = "Удалить комментарий";
            button2.Size = new Size(150, 40);
            button2.BackColor = Color.FromArgb(73, 140, 81);
            button2.ForeColor = Color.White;
            button2.Font = new Font("Comic Sans MS", 12);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(250, 250);


            button3.Text = "Сохранить изменения";
            button3.Size = new Size(150, 40); ;
            button3.BackColor = Color.FromArgb(73, 140, 81);
            button3.ForeColor = Color.White;
            button3.Font = new Font("Comic Sans MS", 12);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(250, 300);


            button4.Text = "Обновить комментарий";
            button4.Size = new Size(150, 40); ;
            button4.BackColor = Color.FromArgb(73, 140, 81);
            button4.ForeColor = Color.White;
            button4.Font = new Font("Comic Sans MS", 12);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(250, 350);




            dataGridView1.Dock = DockStyle.Fill;
            this.Controls.Add(dataGridView1);
        }


        private async void ManagerMenu_Load(object sender, EventArgs e)
        {
            await LoadDataAsync(); 
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
            
            string commentID = Prompt.ShowDialog("ID комментария:", "Добавление комментария");
            if (string.IsNullOrEmpty(commentID)) return;

            string message = Prompt.ShowDialog("Сообщение комментария:", "Добавление комментария");
            if (string.IsNullOrEmpty(message)) return;

            string masterID = Prompt.ShowDialog("ID мастера:", "Добавление комментария");
            if (string.IsNullOrEmpty(masterID)) return;

            string requestID = Prompt.ShowDialog("ID заявки:", "Добавление комментария");
            if (string.IsNullOrEmpty(requestID)) return;

           
            await AddCommentAsync(commentID, message, masterID, requestID);

            
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



        
        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int commentID = (int)dataGridView1.SelectedRows[0].Cells[0].Value; 
                await DeleteCommentAsync(commentID);
                await LoadDataAsync(); 
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите комментарий для удаления.");
            }
        }

        
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

        
        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int commentID = (int)dataGridView1.SelectedRows[0].Cells[0].Value;

                string message = Prompt.ShowDialog("Сообщение комментария:", "Обновление комментария");
                if (string.IsNullOrEmpty(message)) return;

                string masterID = Prompt.ShowDialog("ID мастера:", "Обновление комментария");
                if (string.IsNullOrEmpty(masterID)) return;

                string requestID = Prompt.ShowDialog("ID заявки:", "Обновление комментария");
                if (string.IsNullOrEmpty(requestID)) return;

                await UpdateCommentAsync(commentID, message, masterID, requestID);
                await LoadDataAsync(); 
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите комментарий для обновления.");
            }
        }

        
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

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
