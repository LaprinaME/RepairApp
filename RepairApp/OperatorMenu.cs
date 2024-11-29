using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepairApp
{
    public partial class OperatorMenu : Form
    {
        private string connectionString = @"Data Source=DESKTOP-DFJ77GS;Initial Catalog=RepairBD;Integrated Security=True;MultipleActiveResultSets=True";
        private SqlDataAdapter adapter;
        private DataSet ds;
        private string sql = "SELECT TOP (1000) [requestID], [startDate], [orgTechID], [problemDescription], [statusID], [completionDate], [partID], [masterID], [clientID] FROM [RepairBD].[dbo].[Requests]";

        public OperatorMenu()
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

        private async void OperatorMenu_Load(object sender, EventArgs e)
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
           
            string requestID = Prompt.ShowDialog("ID заявки:", "Добавление заявки");
            if (string.IsNullOrEmpty(requestID)) return;

            string startDate = Prompt.ShowDialog("Дата начала:", "Добавление заявки");
            if (string.IsNullOrEmpty(startDate)) return;

            string orgTechID = Prompt.ShowDialog("ID технической организации:", "Добавление заявки");
            if (string.IsNullOrEmpty(orgTechID)) return;

            string problemDescription = Prompt.ShowDialog("Описание проблемы:", "Добавление заявки");
            if (string.IsNullOrEmpty(problemDescription)) return;

            string statusID = Prompt.ShowDialog("ID статуса:", "Добавление заявки");
            if (string.IsNullOrEmpty(statusID)) return;

            string completionDate = Prompt.ShowDialog("Дата завершения:", "Добавление заявки");
            if (string.IsNullOrEmpty(completionDate)) return;

            string partID = Prompt.ShowDialog("ID детали:", "Добавление заявки");
            if (string.IsNullOrEmpty(partID)) return;

            string masterID = Prompt.ShowDialog("ID мастера:", "Добавление заявки");
            if (string.IsNullOrEmpty(masterID)) return;

            string clientID = Prompt.ShowDialog("ID клиента:", "Добавление заявки");
            if (string.IsNullOrEmpty(clientID)) return;

            
            await AddRequestAsync(requestID, startDate, orgTechID, problemDescription, statusID, completionDate, partID, masterID, clientID);

            
            await LoadDataAsync();
        }

        private async Task AddRequestAsync(string requestID, string startDate, string orgTechID, string problemDescription, string statusID, string completionDate, string partID, string masterID, string clientID)
        {
            string insertSql = @"
    INSERT INTO Requests (requestID, startDate, orgTechID, problemDescription, statusID, completionDate, partID, masterID, clientID) 
    VALUES (@requestID, @startDate, @orgTechID, @problemDescription, @statusID, @completionDate, @partID, @masterID, @clientID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                {
                    insertCommand.Parameters.AddWithValue("@requestID", requestID);
                    insertCommand.Parameters.AddWithValue("@startDate", startDate);
                    insertCommand.Parameters.AddWithValue("@orgTechID", orgTechID);
                    insertCommand.Parameters.AddWithValue("@problemDescription", problemDescription);
                    insertCommand.Parameters.AddWithValue("@statusID", statusID);
                    insertCommand.Parameters.AddWithValue("@completionDate", completionDate);
                    insertCommand.Parameters.AddWithValue("@partID", partID);
                    insertCommand.Parameters.AddWithValue("@masterID", masterID);
                    insertCommand.Parameters.AddWithValue("@clientID", clientID);
                    int numberOfInsertedRows = await insertCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"Добавлено объектов: {numberOfInsertedRows}");
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int requestID = (int)dataGridView1.SelectedRows[0].Cells[0].Value; 
                await DeleteRequestAsync(requestID);
                await LoadDataAsync(); 
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку для удаления.");
            }
        }

        private async Task DeleteRequestAsync(int requestID)
        {
            string deleteSql = "DELETE FROM Requests WHERE requestID = @requestID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@requestID", requestID);
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
                int requestID = (int)dataGridView1.SelectedRows[0].Cells[0].Value; 

                string startDate = Prompt.ShowDialog("Дата начала:", "Обновление заявки");
                if (string.IsNullOrEmpty(startDate)) return;

                string orgTechID = Prompt.ShowDialog("ID технической организации:", "Обновление заявки");
                if (string.IsNullOrEmpty(orgTechID)) return;

                string problemDescription = Prompt.ShowDialog("Описание проблемы:", "Обновление заявки");
                if (string.IsNullOrEmpty(problemDescription)) return;

                string statusID = Prompt.ShowDialog("ID статуса:", "Обновление заявки");
                if (string.IsNullOrEmpty(statusID)) return;

                string completionDate = Prompt.ShowDialog("Дата завершения:", "Обновление заявки");
                if (string.IsNullOrEmpty(completionDate)) return;

                string partID = Prompt.ShowDialog("ID детали:", "Обновление заявки");
                if (string.IsNullOrEmpty(partID)) return;

                string masterID = Prompt.ShowDialog("ID мастера:", "Обновление заявки");
                if (string.IsNullOrEmpty(masterID)) return;

                string clientID = Prompt.ShowDialog("ID клиента:", "Обновление заявки");
                if (string.IsNullOrEmpty(clientID)) return;

                await UpdateRequestAsync(requestID, startDate, orgTechID, problemDescription, statusID, completionDate, partID, masterID, clientID);

                await LoadDataAsync(); 
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заявку для обновления.");
            }
        }

        private async Task UpdateRequestAsync(int requestID, string startDate, string orgTechID, string problemDescription, string statusID, string completionDate, string partID, string masterID, string clientID)
        {
            string updateSql = @"
    UPDATE Requests 
    SET startDate = @startDate, orgTechID = @orgTechID, problemDescription = @problemDescription, 
        statusID = @statusID, completionDate = @completionDate, partID = @partID, masterID = @masterID, clientID = @clientID 
    WHERE requestID = @requestID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@requestID", requestID);
                    updateCommand.Parameters.AddWithValue("@startDate", startDate);
                    updateCommand.Parameters.AddWithValue("@orgTechID", orgTechID);
                    updateCommand.Parameters.AddWithValue("@problemDescription", problemDescription);
                    updateCommand.Parameters.AddWithValue("@statusID", statusID);
                    updateCommand.Parameters.AddWithValue("@completionDate", completionDate);
                    updateCommand.Parameters.AddWithValue("@partID", partID);
                    updateCommand.Parameters.AddWithValue("@masterID", masterID);
                    updateCommand.Parameters.AddWithValue("@clientID", clientID);
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

        }
    }
}
