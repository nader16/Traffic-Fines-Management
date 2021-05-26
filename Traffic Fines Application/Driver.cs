using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.Shared;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Traffic_Fines_Application
{
    public partial class Driver : Form
    {
        public Driver()
        {
            InitializeComponent();
        }
        DriverFine dr;
        private void ret_btn_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            this.Close();
            dr.Close();
            main.Show();
        }
        private void Driver_Load(object sender, EventArgs e)
        {
            dr = new DriverFine();
        }
        private void src_btn_Click(object sender, EventArgs e)
        {
            string ordb = "Data source=orcl;User Id=scott; Password=tiger;";
            OracleConnection conn;
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand c = new OracleCommand();
            c.Connection = conn;
            c.CommandText = "select *   from Vehicles where Driver_LN =:ln and Plate_Number =:platen";
            c.CommandType = CommandType.Text;
            c.Parameters.Add("ln", Convert.ToInt32(textBox1.Text));
            c.Parameters.Add("platen", textBox2.Text);
            OracleDataReader dr1 = c.ExecuteReader();
            if (dr1.Read())
            {
                OracleCommand c1 = new OracleCommand();
                c1.Connection = conn;
                c1.CommandText = "select  * from FineReport where vpn =:vpn";
                c1.CommandType = CommandType.Text;
                c1.Parameters.Add("vpn", textBox2.Text);
                OracleDataReader dr2 = c1.ExecuteReader();
                if (dr2.Read())
                {
                    dr.SetParameterValue(0, textBox1.Text);
                    dr.SetParameterValue(1, textBox2.Text);
                    crystalReportViewer1.ReportSource = dr;
                }
                else { MessageBox.Show("This Vehicle Has No Assigned Fines"); }
            }
            else { MessageBox.Show("Invalid License Number or Plate Number"); }
        }
    }
}
