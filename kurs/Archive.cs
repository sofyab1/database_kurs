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
    public partial class Guests : Form
    {
        public Guests()
        {
            InitializeComponent();
        }

        private SqlConnection sqlConnection = null;
        private void Guests_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();

            // Определите запрос для извлечения нужных столбцов
            string query = "SELECT surname, roomNumber, damage, totalcost, checkIn, checkOut FROM [check]";

            // Создайте SqlCommand для выполнения запроса
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                // Создайте DataTable для хранения результата запроса
                DataTable dataTable = new DataTable();

                // Выполните запрос и заполните DataTable
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }

                // Привяжите DataTable к DataGridView
                dataGridView1.DataSource = dataTable;
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
