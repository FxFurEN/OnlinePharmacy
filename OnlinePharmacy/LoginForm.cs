

using System.Data.OleDb;

namespace OnlinePharmacy
{
    public partial class LoginForm : Form
    {
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=OnlinePharmacy_db.accdb;Persist Security Info=False;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string FIO = textBox1.Text;

            int userId = IsValidUser(FIO);

            if (userId != -1)
            {
                MessageBox.Show("Авторизация успешна!");
                //Form1 mainForm = new Form1(userId);
                Form1 mainForm = new Form1();
                this.Hide();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Неверные учетные данные. Пожалуйста, повторите попытку.");
            }
        }
        private int IsValidUser(string FIO)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT ID FROM [Users] WHERE FIO = @FIO";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FIO", FIO);

                    try
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return -1; // Или другое значение, которое будет обозначать неудачу.
                        }
                    }
                    catch (OleDbException ex)
                    {
                        MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                        return -1; // Или другое значение, которое будет обозначать неудачу.
                    }
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
