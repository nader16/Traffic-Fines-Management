using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Traffic_Fines_Application
{
    public partial class Driver_Pay_Fine : Form
    {
        string ordb = "Data source=orcl;User Id=scott; Password=tiger;";
        OracleConnection conn;
        public Driver_Pay_Fine()
        {
            InitializeComponent();
        }

        private void RetBtn_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            this.Close();
            main.Show();
        }

        private void Driver_Pay_Fine_Load(object sender, EventArgs e)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select Licence_Num  from Driver";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ln_cmb.Items.Add(dr[0]);
            }
            dr.Close();
        }
        OracleDataAdapter adapter1;
        DataSet dataset;
        private void ln_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataset = new DataSet();
            string Cmdstr = @" select fp.FineID , f.Fine_Type , fp.CountTaken,fp.VPN ,fp.Court_ID,fp.Officer_Uname ,fp.IsPaid, v.V_Type ,
                                            fp.DueDate from FineReport fp,Vehicles v, Fines f where fp.VPN =v.PlATE_NUMBER
                                            and v.DRIVER_LN=:LN and fp.FineID= f.FineID";
            adapter1 = new OracleDataAdapter(Cmdstr, ordb);
            adapter1.SelectCommand.Parameters.Add("LN", Convert.ToInt32(ln_cmb.Text));
            adapter1.Fill(dataset);
            if (dataset.Tables[0].Rows.Count != 0) { dataGridView1.DataSource = dataset.Tables[0]; }
            else { dataGridView1.DataSource = ""; MessageBox.Show("Driver Has No Fines"); }
        }

        private void pay_btn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0 )
            {
                Int16 x = Convert.ToInt16(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells[3].Value.ToString());
                Int16 fineid = Convert.ToInt16(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells[0].Value.ToString());
                OracleCommand c1 = new OracleCommand();
                c1.Connection = conn;
                c1.CommandText = @"update driver set total_finesamount = total_finesamount - (select Amount from fines where fineid =:FID) * (select CountTaken from finereport where vpn = :vp and fineid =:fineId)
                                              where licence_num = :LN and 'N'=(select ispaid from finereport where vpn = :vp1 and fineid =:fd) ";
                c1.Parameters.Add("FID", fineid);
                c1.Parameters.Add("vp", x);
                c1.Parameters.Add("fineId", fineid);
                c1.Parameters.Add("LN", Convert.ToInt16(ln_cmb.SelectedItem.ToString()));
                c1.Parameters.Add("vp1", x);
                c1.Parameters.Add("fd", fineid);
                int r = c1.ExecuteNonQuery();
                if (r != -1)
                {
                    OracleCommand c = new OracleCommand();
                    c.Connection = conn;
                    c.CommandText = "update FineReport set isPaid='Y' , CountTaken = 0 where VPN =:vpn and FineID = :fid";
                    c.Parameters.Add("vpn", x);
                    c.Parameters.Add("fid", fineid);
                    int r1 = c.ExecuteNonQuery();
                    if (r1 != -1) { MessageBox.Show("Paid"); }
                }
            }
            else { MessageBox.Show("No Selected Fines"); }
        }
    }
}
