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
    public partial class FineType : Form
    {
        string ordb = "Data source=orcl;User Id=scott; Password=tiger;";
        OracleConnection conn;
        List<int> fid_list = new List<int>();
        public FineType()
        {
            InitializeComponent();
        }

        private void ret_btn_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            this.Close();
            main.Show();
        }

        private void insrt_btn_Click(object sender, EventArgs e)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            if (!fid_list.Contains(Convert.ToInt16(fid_cmb.Text)))
            {
                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = conn;
                cmd1.CommandText = "insert into Fines values (:id,:amount,:type)";
                cmd1.Parameters.Add("id", Convert.ToInt16(fid_cmb.Text.ToString()));
                cmd1.Parameters.Add("amount", Convert.ToInt16(amt_txt.Text.ToString()));
                cmd1.Parameters.Add("type", fty_txt.Text);
                int r = cmd1.ExecuteNonQuery();
                if (r != -1)
                {
                    fid_cmb.Items.Add(fid_cmb.Text);
                    fid_list.Add(Convert.ToInt16(fid_cmb.Text.ToString()));
                    MessageBox.Show("Fine Type added");
                }
            }
            else { MessageBox.Show("Invalid ID"); }
        }

        private void FineType_Load(object sender, EventArgs e)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Get_Fine_ID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("id", OracleDbType.RefCursor, ParameterDirection.Output);
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                fid_cmb.Items.Add(dr[0]);
                fid_list.Add(Convert.ToInt32(dr[0]));
            }
            dr.Close();
        }

        private void fid_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select Fine_Type   from Fines where FineID =:fid";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("fid", fid_cmb.Text.ToString());
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                fty_txt.Text = dr[0].ToString();
            }
            dr.Close();
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand cmd1 = new OracleCommand();
            cmd1.Connection = conn;
            cmd1.CommandText = "get_fines_Ammount";
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Add("fine_id", Convert.ToInt32(fid_cmb.Text.ToString()));
            cmd1.Parameters.Add("fin_Amount", OracleDbType.Decimal, ParameterDirection.Output);
            cmd1.ExecuteNonQuery();
            amt_txt.Text = Convert.ToString(cmd1.Parameters["fin_Amount"].Value.ToString());
        }

        private void uptBTN_Click(object sender, EventArgs e)
        {
            OracleCommand c = new OracleCommand();
            c.Connection = conn;
            c.CommandText = "update Fines set Amount=:amt where FineID =:id";
            c.Parameters.Add("amt", Convert.ToInt16(amt_txt.Text.ToString()));
            c.Parameters.Add("id", Convert.ToInt16(fid_cmb.SelectedItem.ToString()));
            int r = c.ExecuteNonQuery();
            if (r != -1)
            {
                MessageBox.Show("Fine modified");
            }
        }

        private void dlt_btn_Click(object sender, EventArgs e)
        {
            conn = new OracleConnection(ordb);
            conn.Open();
            OracleCommand c = new OracleCommand();
            c.Connection = conn;
            c.CommandText = "Delete_Fine_reports";
            c.CommandType = CommandType.StoredProcedure;
            c.Parameters.Add("amt", Convert.ToInt32(fid_cmb.SelectedItem.ToString()));
            int r = c.ExecuteNonQuery();
            if (r != 0)
            {
                MessageBox.Show("Fine Type Has Been Deleted");
            }
        }
    }
}
