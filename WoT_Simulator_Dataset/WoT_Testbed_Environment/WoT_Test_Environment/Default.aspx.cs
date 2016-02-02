using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;

namespace WoT_Test_Environment
{
    public partial class _Default : System.Web.UI.Page
    {
        static string path_root = "C:\\WoT Testbed\\";
        private string filename = path_root + "WoT_Structure.xls";//"D:\\Master Files\\Codes _2014\\Generated_WoT_Dataset\\
        private string Title1 = "Building";
        private string Title2 = "Device";

        protected void Page_Load(object sender, EventArgs e)
        {
            string SQL = " select id,title,root_id,type,description FROM [" + Title1 + "] WHERE type='room' and root_id>=1 order by root_id,id";
            DataSet room = new DataSet();
            using (System.Data.OleDb.OleDbConnection ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + filename + "';Extended Properties=Excel 8.0;"))
            {
                ExcelCon.Open();

                using (System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(SQL, ExcelCon))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(room);
                    }
                }

                ExcelCon.Close();
            }
            
            GridView1.DataSource = room;
            GridView1.DataBind(); 
             
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {
                    HyperLink hlink = (HyperLink)e.Row.Cells[0].Controls[0];
                    hlink.NavigateUrl = "~/Room_Devices.aspx?Id=" + hlink.Text;
                }
                catch { }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MyCommand")
            {
                int row = int.Parse(e.CommandArgument.ToString());
                var cellText = GridView1.Rows[row].Cells[1].Text.Trim();
               // Label1.Text = cellText.ToString();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}
