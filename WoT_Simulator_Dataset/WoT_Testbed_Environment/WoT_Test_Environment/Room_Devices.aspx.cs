using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;

namespace WoT_Test_Environment
{
    public partial class Room_Devices : System.Web.UI.Page
    {
        static string path_root = "C:\\WoT Testbed\\";
        private string filename = path_root + "WoT_Structure.xls";//"D:\\Master Files\\Codes _2014\\Generated_WoT_Dataset\\
       // private string filename = "D:\\Master Files\\Codes _2014\\Generated_WoT_Dataset\\WoT_Structure.xls";
        private string Title1 = "Building";
        private string Title2 = "Device";

        protected void Page_Load(object sender, EventArgs e)
        {
            string SQL = " select * FROM [" + Title2 + "] WHERE building_id=@Id  order by id";
            DataSet room = new DataSet();
            using (System.Data.OleDb.OleDbConnection ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + filename + "';Extended Properties=Excel 8.0;"))
            {
                ExcelCon.Open();

                using (System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(SQL, ExcelCon))
                {
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@Id", Request.QueryString["Id"]);
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
    }
}