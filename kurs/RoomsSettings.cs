using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace kurs
{
    public partial class RoomsSettings : Form
    {
        public RoomsSettings()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверяем, что все поля заполнены
            if (AreAllFieldsFilled())
            {
                int roomNumber;
                if (!int.TryParse(textBox3.Text, out roomNumber) || roomNumber < 1 || roomNumber > 50)
                {
                    MessageBox.Show("Номер комнаты должен быть целым числом от 1 до 50.");
                    return;
                }

                // Проверка наличия номера комнаты в таблице
                if (IsRoomNumberExists(roomNumber))
                {
                    MessageBox.Show($"Номер комнаты {roomNumber} уже существует в базе данных.");
                    return;
                }

                // Получаем выбранное значение из ComboBox, предварительно проверив, что оно не null
                string category = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : "";
                string amountPeople = comboBox2.SelectedItem != null ? comboBox2.SelectedItem.ToString() : "";
                string floor = comboBox3.SelectedItem != null ? comboBox3.SelectedItem.ToString() : "";

                SqlCommand command = new SqlCommand(
                    $"INSERT INTO [Rooms] " +
                    $"(roomNumber, category, amountPeople, floor, cost)" +
                    $"VALUES" +
                    $"(N'{textBox3.Text}', N'{category}', N'{amountPeople}', N'{floor}', N'{textBox1.Text}')",
                    sqlConnection);

                command.ExecuteNonQuery();
                MessageBox.Show("Данные успешно внесены!");

                // Сбрасываем значения в комбобоксах и текстбоксах
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;
                textBox1.Text = "";
                textBox3.Text = "";
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля перед добавлением комнаты.");
            }
        }

        // Метод для проверки наличия номера комнаты в таблице
        private bool IsRoomNumberExists(int roomNumber)
        {
            string query = $"SELECT COUNT(*) FROM [Rooms] WHERE roomNumber = {roomNumber}";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }

        private SqlConnection sqlConnection = null;
        private void RoomsSettings_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string category = comboBox1.SelectedItem.ToString();
                switch (category)
                {
                    case "Стандарт":
                        textBox1.Text = "2900";
                        comboBox2.SelectedItem = "1";
                        break;
                    case "Стандарт с балконом":
                        textBox1.Text = "3500";
                        comboBox2.SelectedItem = "1";
                        break;
                    case "Студия":
                        textBox1.Text = "4000";
                        comboBox2.SelectedItem = "1";
                        break;
                    case "Полулюкс":
                        textBox1.Text = "5000";
                        comboBox2.SelectedItem = "1";
                        break;
                    case "Люкс":
                        textBox1.Text = "7000";
                        comboBox2.SelectedItem = "1";
                        break;
                    default:
                        MessageBox.Show("Выбран неизвестный вид номера.");
                        break;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                string amountPeople = comboBox2.SelectedItem.ToString();
                switch (amountPeople)
                {
                    case "1": break;
                    default:
                        MessageBox.Show("Выбран неизвестный номер.");
                        break;
                }
            }
        }

        private bool AreAllFieldsFilled()
        {
            // Проверяем, что все необходимые поля заполнены
            return !string.IsNullOrWhiteSpace(textBox3.Text) &&
                   comboBox1.SelectedItem != null &&
                   comboBox2.SelectedItem != null &&
                   comboBox3.SelectedItem != null &&
                   !string.IsNullOrWhiteSpace(textBox1.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox3.SelectedItem != null)
            {
                string floor = comboBox3.SelectedItem.ToString();
                switch (floor)
                {
                    case "1": break;
                    case "2": break;
                    case "3": break;
                    default:
                        MessageBox.Show("Выбран неизвестный номер.");
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int roomNumber;
            if (int.TryParse(textBox3.Text, out roomNumber))
            {
                if (roomNumber >= 1 && roomNumber <= 10)
                {
                    comboBox3.SelectedItem = "1";
                    comboBox1.SelectedItem = "Стандарт";
                }
                else if (roomNumber >= 11 && roomNumber <= 20)
                {
                    comboBox3.SelectedItem = "1";
                    comboBox1.SelectedItem = "Стандарт с балконом";
                }
                else if (roomNumber >= 21 && roomNumber <= 30)
                {
                    comboBox3.SelectedItem = "2";
                    comboBox1.SelectedItem = "Студия";
                }
                else if (roomNumber >= 31 && roomNumber <= 40)
                {
                    comboBox3.SelectedItem = "2";
                    comboBox1.SelectedItem = "Полулюкс";
                }
                else if (roomNumber >= 41 && roomNumber <= 50)
                {
                    comboBox3.SelectedItem = "3";
                    comboBox1.SelectedItem = "Люкс";
                }

                // Заблокировать элементы управления после установки их значений
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                textBox1.Enabled = false;
            }
            else
            {
                // Если введено некорректное значение, очистите выбор в комбобоксах
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;

                // Разблокировать элементы управления для ввода
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                textBox1.Enabled = true;
            }
        }
    }
}
