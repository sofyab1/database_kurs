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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace kurs
{
    public partial class MoveOut : Form
    {
        public MoveOut()
        {
            InitializeComponent();
        }
        private SqlConnection sqlConnection = null;
        private void MoveOut_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();

            // Загрузка данных из таблицы Guests в dataGridView1
            SqlDataAdapter guestsDataAdapter = new SqlDataAdapter("SELECT surname, name, middleName, roomNumber, checkIn, checkOut, cost FROM Guests", sqlConnection);
            DataSet guestsDataSet = new DataSet();
            guestsDataAdapter.Fill(guestsDataSet);
            dataGridView1.DataSource = guestsDataSet.Tables[0];
            dataGridView1.ReadOnly = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите номер из таблицы для продолжения.");
                return;
            }
            // Проверяем, что в таблице останется хотя бы одна строка после удаления
            if (dataGridView1.Rows.Count == 1)
            {
                MessageBox.Show("Нельзя выселить всех. В отеле должен кто-то жить.");
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string roomNumber = selectedRow.Cells["roomNumber"].Value.ToString();
            string checkIn = selectedRow.Cells["checkIn"].Value.ToString();
            string checkOut = selectedRow.Cells["checkOut"].Value.ToString();
            string surname = selectedRow.Cells["surname"].Value.ToString();
            string name = selectedRow.Cells["name"].Value.ToString();
            string middleName = selectedRow.Cells["middleName"].Value.ToString();
            string cost = selectedRow.Cells["cost"].Value.ToString();
           // string category = selectedRow.Cells["category"].Value.ToString();

            Check check = new Check(roomNumber, checkIn, checkOut, surname, name, middleName, cost, /*category, */this);
            //check.FormClosed += Check_FormClosed; // Добавляем обработчик события FormClosed
            //check.ShowDialog();
            DialogResult dialogResult = check.ShowDialog();

            // Проверяем результат диалога
            if (dialogResult == DialogResult.OK)
            {
                // ваш код для удаления строки из DataGridView1
                dataGridView1.Rows.Remove(selectedRow);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {          
            string filter = "";
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                DataTable dataTable = (dataGridView1.DataSource as DataTable);
                // Фильтруем только по столбцу с названием "surname" и только по началу слова
                filter = $"surname LIKE '{textBox2.Text}%'";
            }

             (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = filter;
        }
    }
}
