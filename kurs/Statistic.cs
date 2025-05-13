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
using System.Windows.Forms.DataVisualization.Charting;

namespace kurs
{
    public partial class Statistic : Form
    {
        public Statistic()
        {
            InitializeComponent();
        }
        private SqlConnection sqlConnection = null;
        private void Statistic_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Получаем выбранные значения из датапикеров и комбобокса
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            // Получаем данные из базы данных
            Dictionary<string, int> bookingsData = GetBookingsData(startDate, endDate);

            // Построение графика
            PlotGraph(bookingsData);

        }

        private Dictionary<string, int> GetBookingsData(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, int> bookingsData = new Dictionary<string, int>();

            string query = "SELECT roomNumber, COUNT(*) AS BookingCount FROM [check] " +
                "WHERE checkIn >= @StartDate AND checkOut <= @EndDate " +
                "GROUP BY roomNumber";


            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Преобразовываем номер комнаты в категорию
                        string category = GetCategoryFromRoomNumber(Convert.ToInt32(reader["roomNumber"]));
                        int bookingCount = Convert.ToInt32(reader["BookingCount"]);

                        // Добавляем в словарь
                        if (!bookingsData.ContainsKey(category))
                        {
                            bookingsData.Add(category, bookingCount);
                        }
                        else
                        {
                            bookingsData[category] += bookingCount;
                        }
                    }
                }
            }

            return bookingsData;
        }

        private string GetCategoryFromRoomNumber(int roomNumber)
        {
            if (roomNumber >= 1 && roomNumber <= 10)
            {
                return "Стандарт";
            }
            else if (roomNumber >= 11 && roomNumber <= 20)
            {
                return "Стандарт с балконом";
            }
            else if (roomNumber >= 21 && roomNumber <= 30)
            {
                return "Студия";
            }
            else if (roomNumber >= 31 && roomNumber <= 40)
            {
                return "Полулюкс";
            }
            else if (roomNumber >= 41 && roomNumber <= 50)
            {
                return "Люкс";
            }
            else
            {
                return "Неизвестная категория";
            }
        }

        private void PlotGraph(Dictionary<string, int> bookingsData)
        {
            chart1.Series.Clear();
            Series series = new Series("Бронирования по категориям");
            series.ChartType = SeriesChartType.Column;

            series["PointWidth"] = "0.9"; // Увеличиваем ширину столбцов


            foreach (var kvp in bookingsData)
            {
                series.Points.AddXY(kvp.Key, kvp.Value);
            }

            chart1.Series.Add(series);
            // Настройка диапазона значений оси X
            chart1.ChartAreas[0].AxisX.Maximum = bookingsData.Count + 1; // +1 для увеличения длины оси
            chart1.ChartAreas[0].AxisX.Minimum = 0;
        }
    }
}
