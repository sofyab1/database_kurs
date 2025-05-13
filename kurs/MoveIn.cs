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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace kurs
{
    public partial class MoveIn : Form
    {

        public MoveIn()
        {
            InitializeComponent();
        }
        private DateTime moveIn;
        private DateTime moveOut;
  

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли хотя бы одна строка из таблицы
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку из таблицы.");
                return; // Возвращаем управление без дальнейших действий
            }

            // Получаем выбранную строку из dataGridView1
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            // Получаем значения roomNumber и cost из выбранной строки
            string roomNumber = selectedRow.Cells["roomNumber"].Value.ToString();
            int costPerNight = Convert.ToInt32(selectedRow.Cells["cost"].Value); // Стоимость за ночь

            // Получаем значения дат заезда и выезда из DateTimePicker
            DateTime moveIn = dateTimePicker1.Value;
            DateTime moveOut = dateTimePicker2.Value;

            // Проверяем, если дата выезда меньше или равна дате заезда
            if (moveOut <= moveIn)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.");
                return; // Возвращаем управление без дальнейших действий
            }

            // Вычисляем количество дней между датами заезда и выезда
            int numberOfDays = (int)(moveOut - moveIn).TotalDays;

            // Вычисляем общую стоимость проживания
            int cost = costPerNight * numberOfDays;

            // Передаем значения в конструктор формы Registration при создании экземпляра
            Registration registration = new Registration(roomNumber, cost.ToString(), moveIn, moveOut);
            DialogResult dialogResult = registration.ShowDialog();

            // Проверяем результат диалога
            if (dialogResult == DialogResult.OK)
            {
                // ваш код для удаления строки из DataGridView1
                dataGridView1.Rows.Remove(selectedRow);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите номер из таблицы для продолжения.");
                return;
            }

            // Получаем выбранную строку из dataGridView1
            DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

            // Получаем значения roomNumber, checkIn, checkOut и surname из выбранной строки
            string roomNumber = selectedRow.Cells["roomNumber"].Value.ToString();
            string checkIn = selectedRow.Cells["checkIn"].Value.ToString();
            string checkOut = selectedRow.Cells["checkOut"].Value.ToString();
            string surname = selectedRow.Cells["surname"].Value.ToString();
            string cost = selectedRow.Cells["cost"].Value.ToString();
            string serie = selectedRow.Cells["serie"].Value.ToString();
            string number = selectedRow.Cells["number"].Value.ToString();

            // Передаем значения в конструктор формы Registration при создании экземпляра
            Registration registration = new Registration(roomNumber, checkIn, checkOut, surname, cost, serie, number);
            DialogResult dialogResult = registration.ShowDialog();

            // Проверяем результат диалога
            if (dialogResult == DialogResult.OK)
            {
                // ваш код для удаления строки из DataGridView1
                dataGridView2.Rows.Remove(selectedRow);
            }
            // Очищаем содержимое textBox1
            textBox1.Text = "";
        }

        private SqlConnection sqlConnection = null;
        private void MoveIn_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();

            // Загрузка данных из таблицы Rooms в dataGridView1
            SqlDataAdapter roomsDataAdapter = new SqlDataAdapter("SELECT roomNumber, category, cost FROM Rooms", sqlConnection);
            DataTable roomsDataTable = new DataTable();
            roomsDataAdapter.Fill(roomsDataTable);
            dataGridView1.DataSource = roomsDataTable;
            dataGridView1.ReadOnly = true;

            // Загрузка данных из таблицы reservedRooms в dataGridView2
            SqlDataAdapter reservedRoomsDataAdapter = new SqlDataAdapter("SELECT * FROM reservedRooms", sqlConnection);
            DataTable reservedRoomsDataTable = new DataTable();
            reservedRoomsDataAdapter.Fill(reservedRoomsDataTable);
            dataGridView2.DataSource = reservedRoomsDataTable;
            dataGridView2.ReadOnly = true;

            // Заполнение комбобокса категориями номеров
            FillCategoryComboBox();
        }

        private void FillCategoryComboBox()
        {
            // Получение уникальных категорий из таблицы Rooms
            var categories = ((dataGridView1.DataSource as DataTable)?.AsEnumerable()
                .Select(row => row.Field<string>("category"))).Distinct().ToList();

            // Заполнение комбобокса категориями
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(categories.ToArray());
        }

        private void UpdateDataGridView1()
        {
            string selectedCategory = comboBox1.SelectedItem as string;
            if (selectedCategory == null) return;


            DateTime checkInDate = dateTimePicker1.Value.Date;
            DateTime checkOutDate = dateTimePicker2.Value.Date;

            // Выполняем запрос к базе данных для получения доступных номеров
            string query = @"SELECT roomNumber, category, cost 
             FROM Rooms 
             WHERE category = @category";

            SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);
            adapter.SelectCommand.Parameters.AddWithValue("@category", selectedCategory);

            DataTable availableRoomsDataTable = new DataTable();
            adapter.Fill(availableRoomsDataTable);

            // Фильтруем доступные номера, проверяя их доступность в обеих таблицах
            for (int i = availableRoomsDataTable.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = availableRoomsDataTable.Rows[i];
                int roomNumber = Convert.ToInt32(row["roomNumber"]);

                if (!IsRoomAvailable(roomNumber, checkInDate, checkOutDate))
                {
                    availableRoomsDataTable.Rows.RemoveAt(i); // Удаляем недоступный номер
                }
            }

            dataGridView1.DataSource = availableRoomsDataTable;
        }
        private bool IsRoomAvailable(int roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            // Проверяем забронирован ли номер в таблице reservedRooms
            string reservedRoomsQuery = "SELECT COUNT(*) FROM reservedRooms WHERE @checkInDate < checkOut AND @checkOutDate > checkIn AND roomNumber = @roomNumber";
            using (SqlCommand command = new SqlCommand(reservedRoomsQuery, sqlConnection))
            {
                command.Parameters.AddWithValue("@checkInDate", checkInDate);
                command.Parameters.AddWithValue("@checkOutDate", checkOutDate);
                command.Parameters.AddWithValue("@roomNumber", roomNumber);

                int reservedRoomsCount = (int)command.ExecuteScalar();
                if (reservedRoomsCount > 0)
                {
                    return false; // Номер забронирован
                }
            }

            // Проверяем забронирован ли номер в таблице Guests
            string guestsQuery = "SELECT COUNT(*) FROM Guests WHERE @checkInDate < checkOut AND @checkOutDate > checkIn AND roomNumber = @roomNumber";
            using (SqlCommand command = new SqlCommand(guestsQuery, sqlConnection))
            {
                command.Parameters.AddWithValue("@checkInDate", checkInDate);
                command.Parameters.AddWithValue("@checkOutDate", checkOutDate);
                command.Parameters.AddWithValue("@roomNumber", roomNumber);

                int guestsCount = (int)command.ExecuteScalar();
                if (guestsCount > 0)
                {
                    return false; // Номер забронирован
                }
            }

            return true; // Номер доступен
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                DataTable dataTable = (dataGridView2.DataSource as DataTable);
                // Фильтруем только по столбцу с названием "surname" и только по началу слова
                filter = $"surname LIKE '{textBox1.Text}%'";
            }

             (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = filter;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGridView1();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridView1();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            // Проверяем, если дата выезда меньше или равна дате заезда
            if (dateTimePicker2.Value <= dateTimePicker1.Value)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.");
                dateTimePicker2.Value = dateTimePicker1.Value.AddDays(1); // Возвращаем дату выезда к дате заезда + 1 день
            }
            UpdateDataGridView1();
        }

    }

}
