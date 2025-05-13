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
    public partial class Registration : Form
    {
        private DateTime moveInDate;
        private DateTime moveOutDate;

        // Конструктор для dataGridView1
        public Registration(string roomNumber, string cost, DateTime moveIn, DateTime moveOut)
        {
            InitializeComponent();

            // Устанавливаем значения в соответствующие элементы управления на форме Registration
            textBox1.Text = roomNumber;
            textBox2.Text = cost;

            dateTimePicker1.Value = moveIn;
            dateTimePicker2.Value = moveOut;

            // Сохраняем значения дат заезда и выезда
            moveInDate = moveIn;
            moveOutDate = moveOut;

            // Вызываем метод UpdateTotalCost() для корректного отображения начальной стоимости
            UpdateTotalCost();
        }

    

        // Конструктор для dataGridView2
        public Registration(string roomNumber, string checkIn, string checkOut, string surname, string cost, string serie, string number)
        {
            InitializeComponent();
            // Устанавливаем значения в соответствующие элементы управления на форме Registration
            textBox1.Text = roomNumber;
            textBox3.Text = surname;
            textBox6.Text = serie;
            textBox7.Text = number;
            dateTimePicker1.Value = Convert.ToDateTime(checkIn);
            dateTimePicker2.Value = Convert.ToDateTime(checkOut);
            textBox2.Text = cost;
            // Вызываем метод UpdateTotalCost() для корректного отображения начальной стоимости
            UpdateTotalCost();
        }
        private SqlConnection sqlConnection = null;
        private void Registration_Load(object sender, EventArgs e)
        {

            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();
            // Блокируем возможность редактирования текстовых полей и датапикеров
            textBox1.Enabled = false;
            textBox2.Enabled = false;
          //  textBox6.Enabled = false;
           // textBox7.Enabled = false;
            textBox9.Enabled = false;
            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList; // Запрещаем пользовательский ввод в комбобоксе
            comboBox2.SelectedItem = "Паспорт";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверяем, что все поля заполнены
            if (AreAllFieldsFilled())
            {
                // Проверяем, что в textBox2 введено ровно 4 символа
                if (textBox6.Text.Length != 4)
                {
                    MessageBox.Show("Серия паспорта должна содержать 4 символа.");
                    return;
                }

                // Проверяем, что в textBox3 введено ровно 6 символов
                if (textBox7.Text.Length != 6)
                {
                    MessageBox.Show("Номер паспорта должен содержать 6 символов.");
                    return;
                }
                // Проверяем, что в ComboBox выбран элемент
                if ((comboBox1.SelectedItem != null) || (comboBox2.SelectedItem != null))
                {
                    // Получаем выбранное значение из ComboBox
                    string sex = comboBox1.SelectedItem.ToString();
                    string document = comboBox2.SelectedItem.ToString();

                    // Получаем стоимость из textBox9
                    int cost = int.Parse(textBox9.Text);

                    SqlCommand command = new SqlCommand(
                        "INSERT INTO [Guests] " +
                        "(surname, name, middleName, sex, document, serie, number, whoGave, date, roomNumber, checkIn, checkOut, cost)" +
                        "VALUES" +
                        "(@surname, @name, @middleName, @sex, @document, @serie, @number, @whoGave, @date, @roomNumber, @checkIn, @checkOut, @cost)",
                        sqlConnection);

                    // Добавляем параметры с использованием SqlParameter
                    command.Parameters.AddWithValue("@surname", textBox3.Text);
                    command.Parameters.AddWithValue("@name", textBox4.Text);
                    command.Parameters.AddWithValue("@middleName", textBox5.Text);
                    command.Parameters.AddWithValue("@sex", sex);
                    command.Parameters.AddWithValue("@document", document);
                    command.Parameters.AddWithValue("@serie", textBox6.Text);
                    command.Parameters.AddWithValue("@number", textBox7.Text);
                    command.Parameters.AddWithValue("@whoGave", textBox8.Text);
                    command.Parameters.AddWithValue("@date", dateTimePicker3.Value.Date);
                    command.Parameters.AddWithValue("@roomNumber", textBox1.Text);
                    command.Parameters.AddWithValue("@checkIn", dateTimePicker1.Value.Date);
                    command.Parameters.AddWithValue("@checkOut", dateTimePicker2.Value.Date);
                    command.Parameters.AddWithValue("@cost", cost); // Используем стоимость из textBox9

                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно внесены!");

                    // После успешного внесения данных, удаляем строку из таблицы резервдрумс
                    SqlCommand deleteCommand = new SqlCommand(
                        "DELETE FROM [reservedRooms] WHERE roomNumber = @roomNumber AND checkIn = @checkIn AND checkOut = @checkOut",
                        sqlConnection);

                    // Добавляем параметры для удаления записи
                    deleteCommand.Parameters.AddWithValue("@roomNumber", textBox1.Text);
                    deleteCommand.Parameters.AddWithValue("@checkIn", dateTimePicker1.Value.Date);
                    deleteCommand.Parameters.AddWithValue("@checkOut", dateTimePicker2.Value.Date);

                    deleteCommand.ExecuteNonQuery();

                    // Очищаем значения текстбоксов и комбобоксов
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";
                    comboBox1.SelectedItem = null;
                    comboBox2.SelectedItem = null;

                    // Установка DialogResult в OK
                    this.DialogResult = DialogResult.OK;
                    // Закрываем форму после выполнения всех действий
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля перед добавлением данных.");
            }
        }

        // Метод для проверки заполнения всех полей
        private bool AreAllFieldsFilled()
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text) ||
       string.IsNullOrWhiteSpace(textBox4.Text) ||
       string.IsNullOrWhiteSpace(textBox6.Text) ||
       string.IsNullOrWhiteSpace(textBox7.Text) ||
       string.IsNullOrWhiteSpace(textBox8.Text) ||
       comboBox1.SelectedItem == null ||
       comboBox2.SelectedItem == null ||
       string.IsNullOrWhiteSpace(textBox9.Text))
            {
                return false;
            }
            return true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string sex = comboBox1.SelectedItem.ToString();
                switch (sex)
                {
                    case "Муж": break;
                    case "Жен": break;
                    default:
                        MessageBox.Show("Выбран неизвестный пол.");
                        break;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                string document = comboBox2.SelectedItem.ToString();
                switch (document)
                {
                    case "Паспорт": break; 
                    default:
                        MessageBox.Show("Выбран неизвестный документ.");
                        break;
                }
            }
        }

        private void UpdateTotalCost()
        {
            int baseCost;
            if (!int.TryParse(textBox2.Text, out baseCost))
            {
                MessageBox.Show("Введите корректную базовую стоимость.");
                return;
            }

            // Вычисляем количество ночей между датами заезда и выезда
            int numberOfNights = (dateTimePicker2.Value - dateTimePicker1.Value).Days;

            // Вычисляем общую стоимость (стоимость за одну ночь * количество ночей)
            int totalCost = baseCost/* * numberOfNights*/;

            // Добавляем дополнительные расходы
            int additionalCost = 0;
            if (checkBox1.Checked) additionalCost += 500;
            if (checkBox2.Checked) additionalCost += 900;
            if (checkBox3.Checked) additionalCost += 800;
            if (checkBox4.Checked) additionalCost += 500;
            if (checkBox5.Checked) additionalCost += 300;
            if (checkBox6.Checked) additionalCost += 300;

            // Общая стоимость включает стоимость за ночи и дополнительные расходы
            totalCost += additionalCost;

            // Обновляем значение в текстбоксе 2
            textBox9.Text = totalCost.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, что количество введенных цифр не превышает 4
            if (textBox6.Text.Length > 4)
            {
                // Обрезаем строку до 4 символов
                textBox6.Text = textBox6.Text.Substring(0, 4);
                // Перемещаем курсор в конец текстового поля
                textBox6.SelectionStart = textBox6.Text.Length;
            }
            else
            {
                // Проверяем каждый символ в текстовом поле
                foreach (char c in textBox6.Text)
                {
                    // Если символ не является цифрой, то удаляем его
                    if (!char.IsDigit(c))
                    {
                        // Удаляем символ из текстового поля
                        textBox6.Text = textBox6.Text.Remove(textBox6.Text.Length - 1);
                        // Перемещаем курсор в конец текстового поля
                        textBox6.SelectionStart = textBox6.Text.Length;
                        break; // Прерываем цикл, чтобы обработать только один символ
                    }
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, что количество введенных цифр не превышает 6
            if (textBox7.Text.Length > 6)
            {
                // Обрезаем строку до 6 символов
                textBox7.Text = textBox7.Text.Substring(0, 6);
                // Перемещаем курсор в конец текстового поля
                textBox7.SelectionStart = textBox7.Text.Length;
            }
            else
            {
                // Проверяем каждый символ в текстовом поле
                foreach (char c in textBox7.Text)
                {
                    // Если символ не является цифрой, то удаляем его
                    if (!char.IsDigit(c))
                    {
                        // Удаляем символ из текстового поля
                        textBox7.Text = textBox7.Text.Remove(textBox7.Text.Length - 1);
                        // Перемещаем курсор в конец текстового поля
                        textBox7.SelectionStart = textBox7.Text.Length;
                        break; // Прерываем цикл, чтобы обработать только один символ
                    }
                }
            }
        }
    }
}
