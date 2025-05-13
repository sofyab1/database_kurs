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
    public partial class Rooms : Form
    {
        public Rooms()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RoomsSettings roomsSettings = new RoomsSettings();
            roomsSettings.ShowDialog();

            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Rooms", sqlConnection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
        }

        private SqlConnection sqlConnection = null;
        private void Rooms_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();

            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Rooms", sqlConnection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.ReadOnly = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Проверяем, что останется хотя бы одна строка после удаления
                if (dataGridView1.SelectedRows.Count == dataGridView1.Rows.Count)
                {
                    MessageBox.Show("Не удалось удалить все строки. В таблице должна остаться хотя бы одна строка.");
                    return; // Выходим из метода, чтобы предотвратить удаление всех строк
                }

                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранное?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        int selectedRoomNumber = Convert.ToInt32(row.Cells["roomNumber"].Value);

                        string deleteRoomQuery = "DELETE FROM [Rooms] WHERE roomNumber = @roomNumber";
                        string deleteReservedRoomsQuery = "DELETE FROM [reservedRooms] WHERE roomNumber = @roomNumber";

                        using (SqlCommand deleteRoomCommand = new SqlCommand(deleteRoomQuery, sqlConnection))
                        using (SqlCommand deleteReservedRoomsCommand = new SqlCommand(deleteReservedRoomsQuery, sqlConnection))
                        {
                            deleteRoomCommand.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);
                            deleteReservedRoomsCommand.Parameters.AddWithValue("@roomNumber", selectedRoomNumber);

                            SqlTransaction transaction = null;

                            try
                            {
                                transaction = sqlConnection.BeginTransaction();

                                // Удаление номера из таблицы Rooms
                                deleteRoomCommand.Transaction = transaction;
                                deleteRoomCommand.ExecuteNonQuery();

                                // Удаление номера из таблицы reservedRooms
                                deleteReservedRoomsCommand.Transaction = transaction;
                                deleteReservedRoomsCommand.ExecuteNonQuery();

                                transaction.Commit();

                                // Удаление строки из DataGridView
                                (dataGridView1.DataSource as DataTable).Rows.RemoveAt(row.Index);
                            }
                            catch (Exception ex)
                            {
                                transaction?.Rollback();
                                MessageBox.Show("Ошибка при удалении номера: " + ex.Message);
                            }
                        }
                    }
                    MessageBox.Show("Выбранное успешно удалено.");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
            }
        }


        private void DeleteRoomFromReservedRooms(int roomId)
        {
            string deleteQuery = "DELETE FROM reservedRooms WHERE roomNumber = @roomId";

            using (SqlCommand command = new SqlCommand(deleteQuery, sqlConnection))
            {
                command.Parameters.AddWithValue("@roomId", roomId);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Номер успешно удален из таблицы 'Забронированные номера'.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении номера из таблицы 'Забронированные номера': " + ex.Message);
                }
            }
        }
    }
}
