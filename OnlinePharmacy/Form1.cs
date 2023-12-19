using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlinePharmacy
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=OnlinePharmacy_db.accdb;Persist Security Info=False;";

        public Form1()
        {
            InitializeComponent();
            FillCategoriesListBox();
        }

        private void FillCategoriesListBox()
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT NameType FROM PillType";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add(reader["NameType"].ToString());
                        }
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Очистим блок с таблетками перед загрузкой новых данных
            panel1.Controls.Clear();

            // Получим выбранную категорию
            string selectedCategory = listBox1.SelectedItem.ToString();

            // Запрос к базе данных для получения таблеток выбранной категории
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Pills WHERE TypePill = (SELECT ID FROM PillType WHERE NameType = @NameType)";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NameType", selectedCategory);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        // Создаем блок для каждой категории
                        Panel categoryPanel = new Panel();
                        categoryPanel.Size = new Size(1290, 600);
                        categoryPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
                        categoryPanel.AutoScroll = true;

                        int x = 10;
                        int y = 10;

                        while (reader.Read())
                        {
                            decimal PriceOfPlastin = Convert.ToDecimal(reader["PriceOfPlastin"]);

                            // Создаем блок для отображения таблетки
                            Panel pillPanel = new Panel();
                            pillPanel.BorderStyle = BorderStyle.FixedSingle;
                            pillPanel.Size = new Size(1100, 150);
                            pillPanel.Location = new Point(x, y);

                            PictureBox pictureBox = new PictureBox();
                            //   pictureBox.Image = GetPillImage(reader["ImageFileName"].ToString());
                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            pictureBox.Size = new Size(60, 60);
                            pictureBox.Location = new Point(10, 10);

                            Label nameLabel = new Label();
                            nameLabel.Text = reader["NamePill"].ToString();
                            nameLabel.Location = new Point(80, 10);
                            nameLabel.Size = new Size(200, 30);

                            Label priceLabel = new Label();
                            priceLabel.Text = $"Цена: {reader["FullPrice"].ToString()} тг.";
                            priceLabel.Location = new Point(80, 40);
                            priceLabel.Size = new Size(210, 30);

                            if (PriceOfPlastin > 0)
                            {
                                Label pricePlastinLabel = new Label();
                                pricePlastinLabel.Text = $"Цена за пластину: {reader["PriceOfPlastin"].ToString()} тг.";
                                pricePlastinLabel.Location = new Point(80, 65);
                                pricePlastinLabel.Size = new Size(210, 30);
                                pillPanel.Controls.Add(pricePlastinLabel);
                            }

                            Button buyButton = new Button();
                            buyButton.Text = "Купить";
                            buyButton.Location = new Point(80, 100);
                            buyButton.Tag = reader["ID"].ToString();
                            buyButton.Size = new Size(100, 30);
                            // buyButton.Click += BuyButtonClick;

                            // Добавляем элементы в блок
                            pillPanel.Controls.Add(pictureBox);
                            pillPanel.Controls.Add(nameLabel);
                            pillPanel.Controls.Add(priceLabel);
                            pillPanel.Controls.Add(buyButton);

                            categoryPanel.Controls.Add(pillPanel);

                            // Увеличиваем координаты для следующей таблетки
                            x += 100;

                            if (x > 100) // Если достигли предела по ширине, переходим на следующую строку
                            {
                                x = 10;
                                y += 220;
                            }
                        }

                        // Добавляем блок с категорией в основной блок
                        panel1.Controls.Add(categoryPanel);
                    }
                }
            }
        }
        private void ScrollToTop()
        {
            panel1.AutoScrollPosition = new Point(0, 0);
        }

        private void ScrollToBottom()
        {
            panel1.AutoScrollPosition = new Point(0, panel1.VerticalScroll.Maximum);
        }

    }
}
