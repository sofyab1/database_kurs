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

namespace kurs
{
    public partial class Check : Form
    {
        public Check()
        {
            InitializeComponent();
        }

        private SqlConnection sqlConnection = null;
        private void Check_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();
            // Блокируем возможность редактирования текстовых полей и датапикеров
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox9.Enabled = false;
            textBox10.Enabled = false;
            textBox8.Enabled = false;
            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;

            // Подсчитываем количество суток между датами и записываем результат в textBox9
            TimeSpan difference = dateTimePicker2.Value.Date.Subtract(dateTimePicker1.Value.Date);
            textBox9.Text = difference.TotalDays.ToString();

            // Вычисляем сумму textBox10 и textBox7
            if (int.TryParse(textBox10.Text, out int textBox10Value))
            {
                // Записываем сумму в textBox8
                textBox8.Text = textBox10Value.ToString();
            }

            // Определяем значение для textBox5 в зависимости от roomNumber
            if (int.TryParse(textBox4.Text, out int roomNumber))
            {
                if (roomNumber >= 1 && roomNumber <= 20)
                {
                    textBox5.Text = "Стандарт";
                }
                else if (roomNumber >= 21 && roomNumber <= 40)
                {
                    textBox5.Text = "Стандарт с балконом";
                }
                else if (roomNumber >= 41 && roomNumber <= 60)
                {
                    textBox5.Text = "Студия";
                }
                else if (roomNumber >= 61 && roomNumber <= 80)
                {
                    textBox5.Text = "Полулюкс";
                }
                else if (roomNumber >= 81 && roomNumber <= 100)
                {
                    textBox5.Text = "Люкс";
                }
                else
                {
                    textBox5.Text = "Неизвестно"; // Если roomNumber не входит ни в один из диапазонов
                }
            }
        }

        // Конструктор для 
        public Check(string roomNumber, string checkIn, string checkOut, string surname, string name, string middleName, string cost, /*string category,*/ MoveOut moveOutForm)
        {
            InitializeComponent();

            textBox4.Text = roomNumber;
            textBox3.Text = surname;
            textBox1.Text = name;
            textBox2.Text = middleName;
           // textBox5.Text = category;
            textBox10.Text = cost;
            dateTimePicker1.Value = Convert.ToDateTime(checkIn);
            dateTimePicker2.Value = Convert.ToDateTime(checkOut);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand(
                  "INSERT INTO [check] " +
                  "(surname, name, middleName, roomNumber, category, damage, totalcost, checkIn, checkOut)" +
                  "VALUES" +
                  "(@surname, @name, @middleName, @roomNumber, @category, @damage, @totalcost, @checkIn, @checkOut)",
                  sqlConnection);

            // Добавляем параметры с использованием SqlParameter
            command.Parameters.AddWithValue("@surname", textBox3.Text);
            command.Parameters.AddWithValue("@name", textBox1.Text);
            command.Parameters.AddWithValue("@middleName", textBox2.Text);
            command.Parameters.AddWithValue("@roomNumber", textBox4.Text);
            command.Parameters.AddWithValue("@category", textBox5.Text);
            command.Parameters.AddWithValue("@damage", textBox6.Text);
            command.Parameters.AddWithValue("@totalcost", textBox8.Text);
            command.Parameters.AddWithValue("@checkIn", dateTimePicker1.Value.Date);
            command.Parameters.AddWithValue("@checkOut", dateTimePicker2.Value.Date);

            command.ExecuteNonQuery();
            MessageBox.Show("Данные успешно удалены!");

            // После успешного внесения данных, удаляем строку из таблицы 
            SqlCommand deleteCommand = new SqlCommand(
                "DELETE FROM [Guests] WHERE roomNumber = @roomNumber AND checkIn = @checkIn AND checkOut = @checkOut",
                sqlConnection);

            // Добавляем параметры для удаления записи
            deleteCommand.Parameters.AddWithValue("@roomNumber", textBox4.Text);
            deleteCommand.Parameters.AddWithValue("@checkIn", dateTimePicker1.Value.Date);
            deleteCommand.Parameters.AddWithValue("@checkOut", dateTimePicker2.Value.Date);

            deleteCommand.ExecuteNonQuery();
            // Установка DialogResult в OK
            this.DialogResult = DialogResult.OK;
            // Закрываем форму после выполнения всех действий
            this.Close();
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем, является ли вводимый символ цифрой или символом Backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                // Если символ не является цифрой и не является символом Backspace, отменяем его ввод
                e.Handled = true;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, что оба текстовых поля textBox7 и textBox10 содержат числовые значения
            if (int.TryParse(textBox10.Text, out int textBox10Value))
            {
                // Если textBox7 пуст, обновляем textBox8 значением из textBox10
                if (string.IsNullOrWhiteSpace(textBox7.Text))
                {
                    textBox8.Text = textBox10Value.ToString();
                }
                else
                {
                    // Проверяем, что textBox7 содержит числовое значение
                    if (int.TryParse(textBox7.Text, out int textBox7Value))
                    {
                        // Вычисляем сумму textBox10 и textBox7
                        int sum = textBox7Value + textBox10Value;
                        // Записываем сумму в textBox8
                        textBox8.Text = sum.ToString();
                    }
                }
            }
        }

    }
}
