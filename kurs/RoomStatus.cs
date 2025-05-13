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
    public partial class RoomStatus : Form
    {
        public RoomStatus()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем данные из выбранной строки
                string roomNumber = selectedRow.Cells["roomNumber"].Value.ToString();
                DateTime checkIn = Convert.ToDateTime(selectedRow.Cells["checkIn"].Value);
                DateTime checkOut = Convert.ToDateTime(selectedRow.Cells["checkOut"].Value);
                string cost = selectedRow.Cells["cost"].Value.ToString();

                // Создаем запрос для получения остальных данных по roomNumber
                string query = "SELECT * FROM Guests WHERE roomNumber = @roomNumber";
                SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);
                adapter.SelectCommand.Parameters.AddWithValue("@roomNumber", roomNumber);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];

                    // Получаем данные из таблицы
                    string surname = row["surname"].ToString();
                    string name = row["name"].ToString();
                    string middleName = row["middleName"].ToString();
                    string sex = row["sex"].ToString();
                    string document = row["document"].ToString();
                    string serie = row["serie"].ToString();
                    string number = row["number"].ToString();
                    string whoGave = row["whoGave"].ToString();
                    DateTime date = Convert.ToDateTime(row["date"]);

                    // Создаем экземпляр формы Reg с передачей всех данных
                    Reg regForm = new Reg(surname, name, middleName, sex, document, serie, number, whoGave,
                                          date, roomNumber, checkIn, checkOut, cost);

                    // Отображаем форму Reg
                    regForm.Show();
                }
                else
                {
                    MessageBox.Show("Данные для выбранного номера не найдены.");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку из таблицы для продолжения.");
            }
        }

        private SqlConnection sqlConnection = null;
        private void RoomStatus_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();

            // Загрузка данных из таблицы Guests в dataGridView1
            SqlDataAdapter guestsDataAdapter = new SqlDataAdapter("SELECT roomNumber, checkIn, checkOut, cost FROM Guests", sqlConnection);
            DataSet guestsDataSet = new DataSet();
            guestsDataAdapter.Fill(guestsDataSet);
            dataGridView1.DataSource = guestsDataSet.Tables[0];
            dataGridView1.ReadOnly = true;
        }
    }
}
