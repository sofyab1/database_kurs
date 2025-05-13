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
    public partial class Reg : Form
    {
        public Reg(string surname, string name, string middleName, string sex, string document,
                   string serie, string number, string whoGave, DateTime date,
                   string roomNumber, DateTime checkIn, DateTime checkOut, string cost)
        {
            InitializeComponent();

            // Заполнение элементов управления данными
            textBox3.Text = surname;
            textBox4.Text = name;
            textBox5.Text = middleName;
            comboBox1.SelectedItem = sex;
            comboBox2.SelectedItem = document;
            textBox6.Text = serie;
            textBox7.Text = number;
            textBox8.Text = whoGave;
            dateTimePicker3.Value = date;
            textBox1.Text = roomNumber;
            dateTimePicker1.Value = checkIn;
            dateTimePicker2.Value = checkOut;
            textBox2.Text = cost;

            // Установка свойства ReadOnly для всех текстовых полей
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;

            // Заблокировать возможность изменения значения в комбобоксах
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;

            // Заблокировать возможность изменения значений в dateTimePicker
            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;
            dateTimePicker3.Enabled = false;
        }
        private SqlConnection sqlConnection = null;

        private void Reg_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string document = comboBox2.SelectedItem.ToString();
            switch (document)
            {
                case "Паспорт": break;
                case "Загран": break;
                default:
                    MessageBox.Show("Выбран неизвестный документ.");
                    break;
            }
        }
    }
}
