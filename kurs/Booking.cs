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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace kurs
{
    public partial class Booking : Form
    {
        public Booking()
        {
            InitializeComponent();
        }

        private SqlConnection sqlConnection = null;
        private DataTable originalRoomsDataTable;

        private void Booking_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();

            // Загрузка данных из таблицы Rooms в dataGridView1
            SqlDataAdapter roomsDataAdapter = new SqlDataAdapter("SELECT roomNumber, category, cost FROM Rooms", sqlConnection);
            DataSet roomsDataSet = new DataSet();
            roomsDataAdapter.Fill(roomsDataSet);
            dataGridView1.DataSource = roomsDataSet.Tables[0];
            dataGridView1.ReadOnly = true;

            // Загрузка данных из таблицы reservedRooms в dataGridView2
            SqlDataAdapter reservedRoomsDataAdapter = new SqlDataAdapter("SELECT * FROM reservedRooms", sqlConnection);
            DataSet reservedRoomsDataSet = new DataSet();
            reservedRoomsDataAdapter.Fill(reservedRoomsDataSet);
            dataGridView2.DataSource = reservedRoomsDataSet.Tables[0];
            dataGridView2.ReadOnly = true;

            // Добавляем обработчики событий ValueChanged для dateTimePicker1 и dateTimePicker2
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
            dateTimePicker2.ValueChanged += dateTimePicker2_ValueChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;
            originalRoomsDataTable = roomsDataSet.Tables[0].Copy();
        }

        private int selectedRoomNumber;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                selectedRoomNumber = Convert.ToInt32(selectedRow.Cells["roomNumber"].Value);

                // Добавляем отладочный вывод, чтобы проверить значение selectedRoomNumber
                Console.WriteLine($"Selected Room Number: {selectedRoomNumber}");
            }
        }

        private double GetCostByCategory(string category)
        {
            switch (category)
            {
                case "Стандарт":
                    return 2900;
                case "Стандарт с балконом":
                    return 3500;
                case "Студия":
                    return 4000;
                case "Полулюкс":
                    return 5000;
                case "Люкс":
                    return 7000;
                default:
                    return 0; // Возвращайте значение по умолчанию, если категория не найдена
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получаем выбранный номер из dataGridView1
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                selectedRoomNumber = Convert.ToInt32(selectedRow.Cells["roomNumber"].Value);
            }
            else
            {
                selectedRoomNumber = 0; // Если номер не выбран, сбрасываем selectedRoomNumber в 0
            }

            // Проверяем, выбран ли номер комнаты
            if (selectedRoomNumber == 0)
            {
                MessageBox.Show("Выберите номер, который хотите забронировать.");
                return;
            }
            if (dateTimePicker2.Value.Date <= dateTimePicker1.Value.Date)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Введите фамилию для бронирования.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Введите серию паспорта.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Введите номер паспорта.");
                return;
            }

            // Проверяем, что в textBox2 введено ровно 4 символа
            if (textBox2.Text.Length != 4)
            {
                MessageBox.Show("Серия паспорта должна содержать 4 символа.");
                return;
            }

            // Проверяем, что в textBox3 введено ровно 6 символов
            if (textBox3.Text.Length != 6)
            {
                MessageBox.Show("Номер паспорта должен содержать 6 символов.");
                return;
            }

            string category = comboBox1.SelectedItem.ToString();
            double costPerNight = GetCostByCategory(category); // Получаем стоимость за одну ночь по выбранной категории
            int nights = (int)(dateTimePicker2.Value.Date - dateTimePicker1.Value.Date).TotalDays; // Количество ночей
            double totalCost = costPerNight * nights; // Общая стоимость

            string query = "INSERT INTO [reservedRooms] (checkIn, checkOut, surname, roomNumber, category, cost, serie, number) VALUES (@checkIn, @checkOut, @surname, @roomNumber, @category, @cost, @serie, @number)";
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@checkIn", dateTimePicker1.Value.Date);
                command.Parameters.AddWithValue("@checkOut", dateTimePicker2.Value.Date);
                command.Parameters.AddWithValue("@surname", textBox1.Text);
                command.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@cost", totalCost); // Общая стоимость вместо стоимости за одну ночь
                command.Parameters.AddWithValue("@serie", textBox2.Text);
                command.Parameters.AddWithValue("@number", textBox3.Text);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно внесены!");
                    UpdateReservedRoomsDataGridView();
                    comboBox1.SelectedItem = null;
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    dateTimePicker1.Value = DateTime.Now;
                    dateTimePicker2.Value = DateTime.Now;
                    ClearDataGridViewSelection(dataGridView1);
                    ClearDataGridViewSelection(dataGridView2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении данных в базу данных: {ex.Message}");
                }
            }

            // Очистка значений в DateTimePicker
           // dateTimePicker1.Value = DateTime.Now;
           // dateTimePicker2.Value = DateTime.Now;
        }

        // Метод для снятия выделения с DataGridView
        private void ClearDataGridViewSelection(DataGridView dataGridView)
        {
            dataGridView.ClearSelection();
        }

        // Метод для обновления DataGridView для отображения данных из таблицы reservedRooms
        private void UpdateReservedRoomsDataGridView()
        {
            // Создаем запрос для получения данных из таблицы reservedRooms
            string selectQuery = "SELECT checkIn, checkOut, surname, roomNumber, category, cost, serie, number FROM reservedRooms";
            // Создаем адаптер данных для таблицы reservedRooms
            SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, sqlConnection);
            // Создаем DataSet для хранения данных
            DataSet dataSet = new DataSet();
            try
            {
                // Заполняем DataSet данными из таблицы reservedRooms
                adapter.Fill(dataSet, "reservedRooms");
                // Связываем DataGridView с данными из таблицы reservedRooms
                dataGridView2.DataSource = dataSet.Tables["reservedRooms"];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных в DataGridView: {ex.Message}");
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Устанавливаем дату в dateTimePicker2 на следующий день после выбранной даты в dateTimePicker1
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(1);
            UpdateDataGridView1();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            // Проверяем, изменилось ли значение даты
            if (dateTimePicker2.Value != dateTimePicker2.Value.Date)
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

        private void UpdateDataGridView1()
        {
            if (comboBox1.SelectedItem != null)
            {
                string category = comboBox1.SelectedItem.ToString();
                DateTime checkInDate = dateTimePicker1.Value.Date;
                DateTime checkOutDate = dateTimePicker2.Value.Date;

                DataTable filteredDataTable = originalRoomsDataTable.Clone();

                foreach (DataRow row in originalRoomsDataTable.Rows)
                {
                    string roomCategory = row["category"].ToString();
                    int roomNumber = Convert.ToInt32(row["roomNumber"]);

                    // Выполняем проверку только если категория номера совпадает с выбранной категорией
                    if (roomCategory == category &&
                        !IsRoomBooked(checkInDate, checkOutDate, roomNumber) &&
                        !IsRoomBookedInGuests(checkInDate, checkOutDate, roomNumber))
                    {
                        filteredDataTable.ImportRow(row);
                    }
                }

                dataGridView1.DataSource = filteredDataTable;
            }
            else
            {
                // Если категория не выбрана, отобразить все доступные номера
                dataGridView1.DataSource = originalRoomsDataTable;
            }
        }

        // Метод для проверки, забронирован ли номер на указанные даты
        private bool IsRoomBooked(DateTime checkInDate, DateTime checkOutDate, int roomNumber)
        {
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                DateTime bookedCheckInDate = Convert.ToDateTime(row.Cells["checkIn"].Value);
                DateTime bookedCheckOutDate = Convert.ToDateTime(row.Cells["checkOut"].Value);
                int bookedRoomNumber = Convert.ToInt32(row.Cells["roomNumber"].Value);
                string bookedCategory = row.Cells["category"].Value.ToString();

                // Если номер забронирован на указанные даты, возвращаем true
                if (bookedRoomNumber == roomNumber && DatesOverlap(checkInDate, checkOutDate, bookedCheckInDate, bookedCheckOutDate))
                {
                    return true;
                }
            }
            // Если номер не забронирован на указанные даты, возвращаем false
            return false;
        }

        private bool IsRoomBookedInGuests(DateTime checkInDate, DateTime checkOutDate, int roomNumber)
        {
            string query = "SELECT COUNT(*) FROM Guests WHERE roomNumber = @roomNumber AND " +
                           "((checkIn <= @checkOut AND checkOut >= @checkIn) OR " +
                           "(checkIn >= @checkIn AND checkOut <= @checkOut))";

            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@roomNumber", roomNumber);
                command.Parameters.AddWithValue("@checkIn", checkInDate);
                command.Parameters.AddWithValue("@checkOut", checkOutDate);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private bool DatesOverlap(DateTime checkInDate, DateTime checkOutDate, DateTime bookedCheckInDate, DateTime bookedCheckOutDate)
        {
            return checkInDate <= bookedCheckOutDate && checkOutDate >= bookedCheckInDate;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //UpdateDataGridView1();
            // Проверяем, выбран ли какой-либо элемент в комбобоксе
            if (comboBox1.SelectedItem != null)
            {
                // Получаем выбранную категорию из ComboBox
                string selectedCategory = comboBox1.SelectedItem.ToString();

                // Определяем запрос для получения строк из таблицы Rooms по выбранной категории
                string query = "SELECT * FROM Rooms WHERE category = @Category";

                // Создаем команду SQL для выполнения запроса
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    // Передаем значение параметра категории
                    command.Parameters.AddWithValue("@Category", selectedCategory);

                    // Создаем DataTable для хранения результата запроса
                    DataTable dataTable = new DataTable();

                    // Выполняем запрос и заполняем DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }

                    // Привязываем DataTable к DataGridView для отображения результатов
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // Получаем ID выбранной строки
                int selectedRowID = (int)dataGridView2.SelectedRows[0].Cells["ID"].Value;

                // Формируем запрос DELETE для таблицы reservedRooms
                string query = $"DELETE FROM [reservedRooms] WHERE ID = @ID";

                // Создаем и настраиваем команду
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@ID", selectedRowID);

                try
                {
                    // Выполняем запрос DELETE
                    command.ExecuteNonQuery();

                    // Удаляем выбранную строку из источника данных
                    (dataGridView2.DataSource as DataTable).Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
                    MessageBox.Show("Строка успешно удалена из таблицы 'Забронированные номера'.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении строки из таблицы 'Забронированные номера': " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, что количество введенных цифр не превышает 4
            if (textBox2.Text.Length > 4)
            {
                // Обрезаем строку до 4 символов
                textBox2.Text = textBox2.Text.Substring(0, 4);
                // Перемещаем курсор в конец текстового поля
                textBox2.SelectionStart = textBox2.Text.Length;
            }
            else
            {
                // Проверяем каждый символ в текстовом поле
                foreach (char c in textBox2.Text)
                {
                    // Если символ не является цифрой, то удаляем его
                    if (!char.IsDigit(c))
                    {
                        // Удаляем символ из текстового поля
                        textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
                        // Перемещаем курсор в конец текстового поля
                        textBox2.SelectionStart = textBox2.Text.Length;
                        break; // Прерываем цикл, чтобы обработать только один символ
                    }
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Проверяем, что количество введенных цифр не превышает 6
            if (textBox3.Text.Length > 6)
            {
                // Обрезаем строку до 6 символов
                textBox3.Text = textBox3.Text.Substring(0, 6);
                // Перемещаем курсор в конец текстового поля
                textBox3.SelectionStart = textBox3.Text.Length;
            }
            else
            {
                // Проверяем каждый символ в текстовом поле
                foreach (char c in textBox3.Text)
                {
                    // Если символ не является цифрой, то удаляем его
                    if (!char.IsDigit(c))
                    {
                        // Удаляем символ из текстового поля
                        textBox3.Text = textBox3.Text.Remove(textBox3.Text.Length - 1);
                        // Перемещаем курсор в конец текстового поля
                        textBox3.SelectionStart = textBox3.Text.Length;
                        break; // Прерываем цикл, чтобы обработать только один символ
                    }
                }
            }
        }
    }
}
