using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace WoT_Dataset_WS
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        private string filename = "C:\\WoT Testbed\\Network_Time_All_2015-01-01-h0.XLS";
        //"C:\\WoT Testbed\\Network_Time_All_2014-12-14-h17.xls";
        // "D:\\Master Files\\Codes _2014\\Generated_WoT_Dataset\\Network_Time_All_2014-12-14-h17.xls";
        private string Title = "Network_Time_All";
        System.Data.OleDb.OleDbConnection ExcelCon;


        //---------------------------------------------------------<test>
        [WebMethod]
        public string test(string dev)
        {

            return dev + "-";
        }
//-------------------------------------------------------<Update History>
        [WebMethod]
        public bool update_history(string dev)
        {
            string sheet = (dev.Replace('/', '-')).Replace(':', '@');
            string src = "C:\\WoT Testbed\\" + sheet + ".xls";
            string headers = "[RecDateTime] datetime, [reading] int, [state] int";

            System.Data.OleDb.OleDbCommand cmd;
            System.Data.OleDb.OleDbConnection ExcelCon_dev = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + src + "';Extended Properties=Excel 8.0;");

            if (!File.Exists(src))
            {
                cmd = new System.Data.OleDb.OleDbCommand("create table [" + sheet + "] (" + headers + " ) ", ExcelCon_dev);
                ExcelCon_dev.Open();
                cmd.ExecuteNonQuery();
                ExcelCon_dev.Close();
            }
            try
            {
                // check history size
                ExcelCon_dev.Open();
                cmd = new System.Data.OleDb.OleDbCommand("Select count([reading]) from [" + sheet + "] ", ExcelCon_dev);
                int history_count = int.Parse(cmd.ExecuteScalar().ToString());

                DateTime d = new DateTime();
                d = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
                // delete old rows 
                string val = monitor(dev);
                if (history_count > 20)
                {
                    cmd = new OleDbCommand(" update  [" + sheet + "] set [RecDateTime]='" + d + "', [reading]=" + int.Parse(val) + " where [RecDateTime]=(select min([RecDateTime]) from [" + sheet + "]) ", ExcelCon_dev);

                }
                // insert new rows
                else
                {
                    cmd = new OleDbCommand(" insert into [" + sheet + "] ([RecDateTime], [reading]) values ('" + d + "','" + val + "')", ExcelCon_dev);
                }
                cmd.ExecuteNonQuery();
                ExcelCon_dev.Close();
            }
            catch { ExcelCon_dev.Close(); }


            return true;
        }

//---------------------------------------------------------<monitor>
        [WebMethod]
        public string monitor(string dev)
        {
            string val = "no";
            ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + filename + "';Extended Properties=Excel 8.0;");
            ExcelCon.Close();
            ExcelCon.Open();
            try
            {
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand();
                cmd.Connection = ExcelCon;
                string date1 = DateTime.Parse("01/01/2015 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                string date2 = DateTime.Parse("01/01/2016 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
               // string SQL = " select [" + dev + "] FROM [" + Title + "] WHERE [RecDateTime] =(SELECT min([RecDateTime]) FROM [" + Title + "] WHERE [RecDateTime] BETWEEN @p1 AND @p2)";
               
                string SQL = " select [" + dev + "] FROM [" + Title + "] WHERE [RecDateTime] =(SELECT min([RecDateTime]) FROM [" + Title + "] WHERE [RecDateTime] BETWEEN @p1 AND @p2)";
                cmd.CommandText = SQL;
                cmd.Parameters.Add("@p1", OleDbType.Date).Value = date1;
                cmd.Parameters.Add("@p2", OleDbType.Date).Value = date2;
               
                OleDbDataReader DR = cmd.ExecuteReader();
                DR.Read();
                val= DR.GetValue(0).ToString();
               // val = cmd.ExecuteScalar().ToString();
            }
            catch { }
            ExcelCon.Close();
            return val;
        }
//---------------------------------------------------------------<Set on>
        [WebMethod]
        public bool set_on(string dev)
        {
            try
            {
                string sets = "[" + dev + "]=1";//,[Switch_Toshiba1(7/9:40/2)]=0,[Switch_Toshiba2(8/10:42/2)]=0,[Switch_Toshiba3(9/11:44/2)]=0,[Switch_Toshiba4(10/12:46/2)]=0,[Switch_Toshiba5(16/13:48/2)]=0,[Fan(22/9:39/2)]=0,[Humidity_BLed(26/15:33/2)]=0,[Humidity_GLed(25/15:35/2)]=0,[Humidity_RLed(24/15:37/2)]=0,[Niazy Lamp1(11/9:41/2)]=0,[Niazy Lamp2(12/10:43/2)]=0,[Niazy Lamp3(13/11:45/2)]=0,[Niazy Lamp4(14/12:47/2)]=0,[Niazy Lamp5(15/13:49/2)]=0,[LDR(20/15:14/2)]=0,[DHT11(21/15:15/2)]=0";
             
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand();
                string SQL = null;
                ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + filename + "';Extended Properties=Excel 8.0;");
                ExcelCon.Open();
                cmd.Connection = ExcelCon;
                SQL = "Update [" + Title + "] set " + sets + " WHERE [RecDateTime] =(SELECT min([RecDateTime]) FROM [" + Title + "] WHERE [RecDateTime] BETWEEN @p1 AND @p2)";
                cmd.CommandText = SQL;
                string date1 = DateTime.Parse("01/01/2015 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                string date2 = DateTime.Parse("01/01/2016 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                cmd.Parameters.Add("@p1", OleDbType.Date).Value = date1;
                cmd.Parameters.Add("@p2", OleDbType.Date).Value = date2;

                cmd.CommandText = SQL;
                int x = cmd.ExecuteNonQuery();
                ExcelCon.Close();
                return true;
            }
            catch { return false; }
        }
        //------------------------------------------------------<set off>
        [WebMethod]
        public bool set_off(string dev)
        {
            try
            {
                string sets = "[" + dev + "]=0";
                string y0 = DateTime.Parse("14/12/2014 18:" + DateTime.Now.Minute + "").ToString("dd/MM/yyyy HH:mm"); 
                string y1 = DateTime.Parse("01/01/2015 " + DateTime.Now.ToLocalTime().Hour + ":" + DateTime.Now.ToLocalTime().Minute + "").ToString("dd/MM/yyyy HH:mm"); 
              
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand();
                string SQL = null;
                ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + filename + "';Extended Properties=Excel 8.0;");
                ExcelCon.Open();
                cmd.Connection = ExcelCon;
                SQL = "Update [" + Title + "] set " + sets + " WHERE [RecDateTime] =(SELECT min([RecDateTime]) FROM [" + Title + "] WHERE [RecDateTime] BETWEEN @p1 AND @p2)";
                cmd.CommandText = SQL;
                string date1 = DateTime.Parse("01/01/2015 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                string date2 = DateTime.Parse("01/01/2016 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                
                cmd.Parameters.Add("@p1", OleDbType.Date).Value = date1;
                cmd.Parameters.Add("@p2", OleDbType.Date).Value = date2;

                cmd.CommandText = SQL;
                int x = cmd.ExecuteNonQuery();
                ExcelCon.Close();
                return true;
            }
            catch { return false; }
        }
    }
}