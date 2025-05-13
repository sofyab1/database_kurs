using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace kurs
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString);
            sqlConnection.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Booking booking = new Booking();
            booking.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MoveIn moveIn = new MoveIn();
            moveIn.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MoveOut moveOut = new MoveOut();
            moveOut.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RoomStatus status = new RoomStatus();
            status.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Rooms rooms = new Rooms();
            rooms.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Statistic statistic = new Statistic();
            statistic.ShowDialog();
        }
    }
}
