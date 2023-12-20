using System.Data.OleDb;

namespace OnlinePharmacy
{
    public partial class CartForm : Form
    {

        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=OnlinePharmacy_db.accdb;Persist Security Info=False;";
        private Dictionary<string, int> cartItems;
        private Form1 mainForm;
        private decimal totalCost; // Переменная для отслеживания общей стоимости

        public CartForm(Dictionary<string, int> cartItems, Form1 mainForm)
        {
            InitializeComponent();
            this.cartItems = cartItems;
            this.mainForm = mainForm;
            FillCartListBox();
            UpdateTotalCost(); // Инициализация общей стоимости
        }

        private void FillCartListBox()
        {
            foreach (var item in cartItems)
            {
                listBox1.Items.Add($"{item.Key} - {item.Value} шт."); // Добавляем стоимость товара
            }

            listBox1.DoubleClick += ListBox1_DoubleClick;
            UpdateTotalCost(); // Обновление общей стоимости при инициализации
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedCartItem = listBox1.SelectedItem.ToString();
                string itemName = selectedCartItem.Split('-')[0].Trim();

                if (cartItems.ContainsKey(itemName))
                {
                    cartItems[itemName]--;
                    if (cartItems[itemName] == 0)
                    {
                        cartItems.Remove(itemName);
                    }

                    UpdateCartDisplay();
                    UpdateTotalCost(); // Обновление общей стоимости
                    if (cartItems.Count == 0)
                    {
                        this.Close();
                    }
                }
            }
        }

        private void UpdateCartDisplay()
        {
            listBox1.Items.Clear();
            FillCartListBox();
            mainForm.UpdateCartLabel();
        }

        private void UpdateTotalCost()
        {
            totalCost = CalculateTotalCost();
            label1.Text = $"Итог: {totalCost} тг"; // Обновление метки с общей стоимостью
        }

        private decimal CalculateTotalCost()
        {
            decimal total = 0;
            foreach (var item in cartItems)
            {
                total += GetItemCostFromDatabase(item.Key) * item.Value; // Умножаем стоимость товара на количество
            }
            return total;
        }

        private decimal GetItemCostFromDatabase(string itemName)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT FullPrice FROM Pills WHERE NamePill = @ItemName";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ItemName", itemName);

                    object result = command.ExecuteScalar();

                    if (result != null && decimal.TryParse(result.ToString(), out decimal itemCost))
                    {
                        return itemCost;
                    }
                }
            }
            return 0;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Заказ подтвержден!");
            cartItems.Clear();
            UpdateCartDisplay();
            UpdateTotalCost();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Корзина очищена!");
            cartItems.Clear();
            UpdateCartDisplay();
            UpdateTotalCost();
            this.Close();
        }
    }
}
