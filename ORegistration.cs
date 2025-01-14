﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;
using System.IO;
namespace VRMS___Management__12_01_21_
{
    public partial class ORegistration : Form
    {
        public ORegistration()
        {
            InitializeComponent();
        }

        //DATABASE
        OdbcConnection con = new OdbcConnection("dsn=capstone");

        //FORM LOAD
        private void ORegistration_Load(object sender, EventArgs e)
        {
            
            display();
        }

        //DISPLAY REGISTERED OWNERS DATA
        public void display()
        {
            try
            {
                OdbcCommand cmd = new OdbcCommand("SELECT owner_id as 'PROPRIETARY ID', school_id as 'SCHOOL ID', lname as 'LAST NAME', fname as 'FIRST NAME', mname as 'M.I.', suf as 'SUFFIX', type as 'OWNER TYPE' FROM registered_owners;", con);
                OdbcDataAdapter adptr = new OdbcDataAdapter(cmd);
                DataSet ds = new DataSet();
                adptr.Fill(ds, "Empty");
                dgvRP.DataSource = ds.Tables[0];
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
            pbOwner.Image = null;
            label2.Text = "";
            label4.Text = "";
            label6.Text = "";
            label8.Text = "";

        }

        //SEARCH
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            OdbcConnection cons = new OdbcConnection("dsn=capstone");
            try
            {
                OdbcCommand commands = new OdbcCommand("SELECT owner_id, school_id, fullname, type FROM registered_owners WHERE owner_id LIKE '%" + txtSearch.Text + "%' OR school_id LIKE '%" + txtSearch.Text + "%' OR fullname LIKE '%" + txtSearch.Text + "%'", cons);
                OdbcDataAdapter adptrr = new OdbcDataAdapter(commands);
                DataTable dt = new DataTable();
                adptrr.Fill(dt);
                dgvRP.DataSource = dt;
                con.Close();

                dgvRP.Columns[0].HeaderText = "OWNER ID";
                dgvRP.Columns[1].HeaderText = "SCHOOL ID";
                dgvRP.Columns[3].HeaderText = "OWNER TYPE";
            }catch(Exception ex)
            {
                cons.Close();
            }
            pics();
            fetch();
        }

        public void fetch()
        {
            try
            {
                OdbcCommand cmd = new OdbcCommand("SELECT owner_id,school_id,type,fname,mname,lname,suf	FROM registered_owners WHERE owner_id='"+txtSearch.Text+"'",con);
                OdbcDataAdapter adptr = new OdbcDataAdapter(cmd);
                DataTable dt = new DataTable();
                adptr.Fill(dt);
                label2.Text = dt.Rows[0][0].ToString();
                label4.Text = dt.Rows[0][5].ToString() + ", " + dt.Rows[0][3].ToString() + " " + dt.Rows[0][4].ToString()+".";
                label6.Text = dt.Rows[0][1].ToString();
                label8.Text = dt.Rows[0][2].ToString();
                con.Close();
            }catch(Exception ex){
                if (txtSearch.TextLength > 7)
                {
                    MessageBox.Show("No data found.");
                }
                con.Close();
            }
        }

        public void pics()
        {


            try
            {
                con.Open();
                OdbcCommand cmd = new OdbcCommand("select img from owner_pic where owner_id='" + txtSearch.Text + "'", con);
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[0]["img"]);
                    pbOwner.Image = new Bitmap(ms);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddOwner ao = new AddOwner();
            ao.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(txtSearch.Text==""){
                MessageBox.Show("PLease select a data, you want to update");
            }
            else
            {
                OUpdate ou = new OUpdate();
                ou.lblPID.Text = lblShowID.Text;
                ou.ShowDialog();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "")
            {
                MessageBox.Show("PLease select a data, you want to delete");
            }
            else {
    
              DialogResult ask = MessageBox.Show("Are you sure you want to delete the proprietary data", "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
              if (ask == DialogResult.Yes)
              {
                  try
                  {
                      OdbcCommand cmd = new OdbcCommand("SELECT count(owner_id) FROM registered_vehicles WHERE owner_id = '" + lblShowID.Text + "'", con);
                      OdbcDataAdapter adptr = new OdbcDataAdapter(cmd);
                      DataTable dt = new DataTable();
                      adptr.Fill(dt);
                      con.Close();
                      //count number of rows in registered vehicles
                      int i = 0;
                      i = Int32.Parse(dt.Rows[0][0].ToString());

                      //fetch data in registered owners
                      OdbcCommand cmd1 = new OdbcCommand("SELECT * FROM registered_owners WHERE owner_id = '" + lblShowID.Text + "'", con);
                      OdbcDataAdapter adptr1 = new OdbcDataAdapter(cmd1);
                      DataTable dt1 = new DataTable();
                      adptr1.Fill(dt1);
                      con.Close();

                      //insert data of owners in archived table
                      con.Open();
                      OdbcCommand cmd3 = new OdbcCommand();
                      cmd3 = con.CreateCommand();
                      cmd3.CommandText = "INSERT INTO Archived(Archived_Operator_Owner_ID,Archived_Operator_Sch_ID,Archived_Operator_type,Archived_Operator_fullname,Archived_Operator_Mid,Archived_Operator_Last,Archived_Operator_Suffix)VALUES(?,?,?,?,?,?,?)";
                      cmd3.Parameters.Add("@Archived_Operator_Owner_ID", OdbcType.VarChar).Value = dt1.Rows[0][1].ToString();
                      cmd3.Parameters.Add("@Archived_Operator_Sch_ID", OdbcType.VarChar).Value = dt1.Rows[0][2].ToString();
                      cmd3.Parameters.Add("@Archived_Operator_type", OdbcType.VarChar).Value = dt1.Rows[0][3].ToString();
                      cmd3.Parameters.Add("@Archived_Operator_fullname", OdbcType.VarChar).Value = dt1.Rows[0][4].ToString();
                      cmd3.Parameters.Add("@Archived_Operator_Mid", OdbcType.VarChar).Value = dt1.Rows[0][5].ToString();
                      cmd3.Parameters.Add("@Archived_Operator_Last", OdbcType.VarChar).Value = dt1.Rows[0][6].ToString();
                      cmd3.Parameters.Add("@Archived_Operator_Suffix", OdbcType.VarChar).Value = dt1.Rows[0][7].ToString();
                      //cmd3.Parameters.Add("@Archived_Operator_ID", OdbcType.VarChar).Value = dt1.Rows[0][0].ToString();
                      if (cmd3.ExecuteNonQuery() == 1)
                      {
                          MessageBox.Show("Proprietary Data was Deleted");
                        
                          
                      }
                      con.Close();


                      //insert multiplerows in v_archived
                      for (int o = 0; o < i; o++)
                      {
                          OdbcCommand cmd4 = new OdbcCommand("SELECT qrtext,type,plate_num,owner_id,enc FROM registered_vehicles WHERE owner_id = '" + lblShowID.Text + "'", con);
                          OdbcDataAdapter adptr2 = new OdbcDataAdapter(cmd4);
                          DataTable dt2 = new DataTable();
                          adptr2.Fill(dt2);
                          con.Close();

                          con.Open();
                          OdbcCommand cmd5 = new OdbcCommand();
                          cmd5 = con.CreateCommand();
                          cmd5.CommandText = "INSERT INTO v_archived(Archived_Vehicle_Qrtext,Archived_Vehicle_type,Archived_Vehicle_PlateNum,Archived_Vehicle_OwnerID,Archived_Vehicle_Enc)VALUES(?,?,?,?,?);";
                          cmd5.Parameters.Add("@Archived_Vehicle_Qrtext", OdbcType.VarChar).Value = dt2.Rows[0][0].ToString();
                          cmd5.Parameters.Add("@Archived_Vehicle_type", OdbcType.VarChar).Value = dt2.Rows[0][1].ToString();
                          cmd5.Parameters.Add("@Archived_Vehicle_PlateNum", OdbcType.VarChar).Value = dt2.Rows[0][2].ToString();
                          cmd5.Parameters.Add("@Archived_Vehicle_OwnerID", OdbcType.VarChar).Value = dt2.Rows[0][3].ToString();
                          cmd5.Parameters.Add("@Archived_Vehicle_Enc", OdbcType.VarChar).Value = dt2.Rows[0][4].ToString();
                          cmd5.ExecuteNonQuery();
                          con.Close();
                      }

                      //delete of data in registered owners
                      con.Open();
                      OdbcCommand cmd6 = new OdbcCommand();
                      cmd6 = con.CreateCommand();
                      cmd6.CommandText = "DELETE FROM registered_owners WHERE owner_id = '" + lblShowID.Text + "'";
                      cmd6.ExecuteNonQuery();
                      con.Close();

                      con.Open();
                      OdbcCommand cmd7 = new OdbcCommand();
                      cmd7 = con.CreateCommand();
                      cmd7.CommandText = "DELETE FROM registered_vehicles WHERE owner_id = '" + lblShowID.Text + "'";
                      cmd7.ExecuteNonQuery();
                      con.Close();

                      display();
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                      con.Close();
                  }

              }
              else
              {

              }
            }

        }

        
        private void dgvRegOwn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                String OWID;
                OWID = dgvRP.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                lblShowID.Text = OWID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lblShowID_Click(object sender, EventArgs e)
        {

        }

        private void bunifuCustomDataGrid1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                String OWID;
                OWID = dgvRP.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                lblShowID.Text = OWID;
                txtSearch.Text = OWID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bunifuCustomDataGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                String OWID;
                OWID = dgvRP.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                lblShowID.Text = OWID;
                txtSearch.Text = OWID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
