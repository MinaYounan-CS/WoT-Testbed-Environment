using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace DataSet_Generator
{
    class G_Operations
    {
        public static int[] childs;
        public static string[] parts;
        public static string[] types;

        // create excel sheet for WoT hierarchical structure
        public static void write_hierarchicals_excel(string dir, SqlConnection con2db)
        {
            string path=dir + "\\WoT";
            string filename = "WoT_Structure.xls";
            string sheet1="Building", sheet2="Device";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (File.Exists(path +"\\"+ filename))
            {
                File.Delete(path +"\\"+ filename);
            }
            string header1 = "[id]int,[title]string,[root_id]int,[type]string,[description]string,[image_url]string";
            string header2 = "[id]int,[building_id]int,[title]string,[type]string,[model]string,[serial]string,[manufacturer]string,[description]string,[gateway_id]int,[pin_id]int,[ws_address]string,[img_on_url]string,[img_off_url]string";

            System.Data.OleDb.OleDbConnection ExcelCon;
            ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + path + "\\" + filename + "';Extended Properties='Excel 8.0;HDR=Yes'");
            ExcelCon.Open();
            System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand("create table [" + sheet1 + "] (" + header1 + " ) ", ExcelCon);
            cmd.ExecuteNonQuery();
                                           cmd = new System.Data.OleDb.OleDbCommand("create table [" + sheet2 + "] (" + header2 + " ) ", ExcelCon);
            cmd.ExecuteNonQuery();
            ExcelCon.Close();
            string sql1 = "Select id, title, root_id, type, description from building";
            string sql2 = "Select id, root_id, title, type, model, serial, manufacturer, description, gateway_id, pin_id,ws_address from device";

            con2db.Close();
            con2db.Open();
            ExcelCon.Close();
            ExcelCon.Open();

            SqlCommand sqlcmd = new SqlCommand(sql1, con2db);
            SqlDataAdapter dt = new SqlDataAdapter(sqlcmd);
            DataSet st = new DataSet();
            dt.Fill(st);
            DataRowCollection dtc = st.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                try { int.Parse(dr[2] + ""); } catch{dr[2] = 0;}
                //string txt = dr[4].ToString(); 
                for (int i = 0; i < 5; i++)
                {
                    if (dr[i].ToString().Contains("'")) dr[i] = dr[i].ToString().Replace("'s", "''s");
                }
                cmd.CommandText = " insert into [" + sheet1 + "] ([id],[title],[root_id],[type],[description]) values (" + dr[0] + ",'"+dr[1].ToString()+"',"+dr[2]+",'"+dr[3].ToString()+"','"+dr[4]+"')"; 
                cmd.Connection = ExcelCon;
                cmd.ExecuteNonQuery();
            }
            con2db.Close();
            con2db.Open();
            ExcelCon.Close();
            ExcelCon.Open();

            sqlcmd = new SqlCommand(sql2, con2db);
            dt = new SqlDataAdapter(sqlcmd);
            st = new DataSet();
            dt.Fill(st);
            dtc = st.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                cmd.CommandText = " insert into [" + sheet2 + "] ([id],[building_id],[title],[type],[model],[serial],[manufacturer],[description],[gateway_id],[pin_id],[ws_address]) values (" + dr[0] + ","+dr[1]+",'"+dr[2]+"','"+dr[3]+"','"+dr[4]+"','"+dr[5]+"','"+dr[6]+"','"+dr[7]+"',"+dr[8]+","+dr[9]+",'"+dr[10]+"')"; cmd.Connection = ExcelCon;
                cmd.ExecuteNonQuery();
            }
            con2db.Close();
            ExcelCon.Close();
        }
        // write whole WoT in microformat 
        public static void write_WoT_microformat(SqlConnection con2db, string path)
        {
            if (!Directory.Exists(path + "\\WoT"))
                Directory.CreateDirectory(path + "\\WoT");
            string filename = "WoT_microformat.txt";
            var lines = write_recursively_wot_microformat(1, con2db, "","","").Split('|');
            File.WriteAllLines(path + "\\WoT" + "\\" + filename, lines);
        }
        public static string write_recursively_wot_microformat(int root, SqlConnection con2db, string spaces, string strstring, string endstring)
        {
            string finalwrite = "";
            spaces += " \t";
            con2db.Close();
            con2db.Open();// id, title, root_id, type, description
            SqlCommand cmd = new SqlCommand(" select id, title, type, description from building where root_id=" + root + "", con2db);
            SqlDataAdapter dt = new SqlDataAdapter(cmd);
            DataSet st = new DataSet();
            dt.Fill(st);
            int count = st.Tables[0].Rows.Count;
            DataRowCollection dtc = st.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                finalwrite +=   spaces + strstring+
                                spaces + "<div class =\"hproduct \">|" +
                                spaces + "   <span class =\" fn\">" + dr[1].ToString() + "</span>|" +
                                spaces + "   <span class =\" identifier\">,|" +
                                spaces + "       <span class =\" type \">" + dr[2].ToString() + "</span> unique identifier|" +
                                spaces + "       <span class =\" value \">" + dr[0].ToString() + "</span>|" +
                                spaces + "   </span>|" +
                                spaces + "   <span class =\" description \">" + dr[3].ToString() + "</span>|" +
                                spaces + "   <span class =\" Photo \"> XXX </span>|" +
                                spaces + "     <a href = XXX  class =\" URL\"> More information about that Object/STh.</a>|" +
                                spaces + "   <ul>|" ;

                if (dr[2].ToString() == "room")
                    finalwrite += write_recursively_dev_microformat(int.Parse(dr[0].ToString()), con2db, spaces);
                else
                    finalwrite += write_recursively_wot_microformat(int.Parse(dr[0].ToString()), con2db, spaces, "<li>|", "</li>|");
                 
                finalwrite += spaces + "   </ul>|" +
                              spaces + "</div>|"+
                              spaces + endstring;
            }
            return finalwrite;
        }
        public static string write_recursively_dev_microformat(int root, SqlConnection con2db, string spaces)
        {
            string finalwrite = "";
            spaces += " \t";
            con2db.Close();
            con2db.Open();// id, title, root_id, type, description
            SqlCommand cmd = new SqlCommand(" select id,root_id,title,type,model,serial,manufacturer,description,gateway_id,pin_id,ws_address from device where root_id=" + root + "", con2db);
            SqlDataAdapter dt = new SqlDataAdapter(cmd);
            DataSet st = new DataSet();
            dt.Fill(st);
            int count = st.Tables[0].Rows.Count;
            DataRowCollection dtc = st.Tables[0].Rows;
            foreach (DataRow dr in dtc)
            {
                finalwrite += spaces + "<li>|" +
                              spaces + "   <span class =\" fn\">" + dr[0].ToString() + "_" + dr[2].ToString() + "</span>|" +
                              spaces + "   <span class =\" identifier\">,|" +
                              spaces + "       <span class =\" type \">" + dr[3].ToString() + "</span> unique identifier|" +
                              spaces + "       <span class =\" value \"> XXX </span>|" +
                              spaces + "   </span>|" +
                              spaces + "   <span class =\" category \">|" +
                              spaces + "       <a href =http :// www.webofthings . com  rel =\" tag\"> " + dr[3].ToString() + " </a>|" +
                              spaces + "   </span>|" +
                              spaces + "   <span class =\" brand \">" + dr[6].ToString() + "</span>|" +
                              spaces + "   <span class =\" description \">" + dr[7].ToString() + "</span>|" +
                              spaces + "   <span class =\" Photo \">" + dr[7].ToString() + "</span>|" +
                              spaces + "     <a href = XXX |" +
                              spaces + "     class =\" URL\"> More information about this device .</a>|" +
                              spaces + "</li>|";
            }
            return finalwrite;
        }
        // write device in microformat 
        public static void write_microformate(SqlConnection con2db, int dev_id,string path, string spaces)
        {
            if(!Directory.Exists(path + "\\WoT\\microformat")) 
                Directory.CreateDirectory(path + "\\WoT\\microformat");
            int cur_value = 0; string img_url = "";
            string[] dev_info = get_devic_info_list(con2db, dev_id);
            //id,building_id,title,type,model,serial,manufacturer,description,gateway_id,pin_id,ws_address
            //0     1          2    3    4      5        6            7          8          9        10
            //<div class="vcard">
            string []smformat = {spaces+"<div class =\"hproduct \">" ,
                                 spaces+"   <span class =\" fn\">" + dev_info[0] + "_" + dev_info[2] + "</span>" ,
                                 spaces+"   <span class =\" identifier\">," ,
                                 spaces+"       <span class =\" type \">" + dev_info[3] + "</span> unique identifier" ,
                                 spaces+"       <span class =\" value \">" + cur_value + "</span>" ,
                                 spaces+"   </span>" ,
                                 spaces+"   <span class =\" category \">" ,
                                 spaces+"       <a href =http :// www.webofthings . com  rel =\" tag\"> " + dev_info[3] + " </a>" ,
                                 spaces+"   </span>" ,
                                 spaces+"   <span class =\" brand \">" + dev_info[6] + "</span>" ,
                                 spaces+"   <span class =\" description \">" + dev_info[7] + "</span>" ,
                                 spaces+"   <span class =\" Photo \">" + dev_info[7] + "</span>" ,
                                 spaces+"     <a href =" + img_url +"  class =\" URL\"> More information about this device .</a>" ,
                                 spaces+"</div>"};

            File.WriteAllLines(path + "\\WoT\\microformat\\" + dev_info[0] + "_" + dev_info[1] + "_" + dev_info[2] + ".txt", smformat);
        }
        public static void write_microdata(string s, string cat, string sensor, string output, string method, string cur_value, string unit, string location, float longitude, float latitude, string img_src)
        {
            string[] smdata ={ "<html><div itemscope> this is " ,
                "<span itemprop=\"category\">" + cat + "</span> called" ,
                "<span itemprop=\"name\">" + sensor + "</span> that delivers the" ,
                "<span itemprop=\"output\">" + output + "</span> (on " +
                "<span itemprop=\"method\">" + method + "</span> requests):" ,
                "<span itemprop=\"output-value\">" + cur_value + "</span> " ,
                "<span itemprop=\"unit\">" + unit + "</span> </br></br>" ,
                "Sensor is located in " ,
                "<span itemscope itemprop=\"GeoDate\">" ,
                "<span itemprop=\"locality\">" + location + "</span> called" ,
                "<span itemprop=\"longitude\">" + longitude + "</span> called" ,
                "<span itemprop=\"latitude\">" + latitude + "</span> called" ,
                "</span></br></br>" ,
                "<img itemprop=\"picture\"> src=\"" + img_src + "\" widith=\"150\" />" ,
                "</div>" ,
                "</html>"};
        }
        public static void LoadCompWithCondition(ComboBox CBox, string display_column, string value_column, string sqlstring, SqlConnection con)
        {
            con.Close();
            con.Open();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = sqlstring;
            cmd.Connection = con;
            SqlDataAdapter adaptorr = new SqlDataAdapter(cmd);
            DataTable DTt = new DataTable();
            adaptorr.Fill(DTt);
           // CBox.Items.Clear();
            CBox.DataSource = DTt;
            CBox.DisplayMember = "" + display_column + "";
            CBox.ValueMember = "" + value_column + "";
            con.Close();
        }
        public static void load_devices( DataGridView DGV,SqlConnection con2db,string condition)
        {
            con2db.Close();
            con2db.Open();
            string sql_s="";
            sql_s="SELECT        dbo.device.id, dbo.device.title, dbo.device.type, dbo.building.title AS Location, dbo.Gateway.Type AS Gateway, dbo.device.pin_id, dbo.device.model, "+
                  "              dbo.device.serial, dbo.device.manufacturer, dbo.device.description"+
                  " FROM         dbo.device INNER JOIN"+
                  "              dbo.building ON dbo.device.root_id = dbo.building.id INNER JOIN"+
                  "              dbo.Gateway ON dbo.device.gateway_id = dbo.Gateway.Id";
            if(condition.Length>0)
                sql_s += condition;
            
            SqlDataAdapter DA = new SqlDataAdapter(sql_s, con2db);
            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            DGV.DataSource = dt;
            con2db.Close();
        }
        // get List of device's ids
        public static string[] get_devic_info_list(SqlConnection con2db,int dev_id)
        {
            con2db.Close();
            con2db.Open();
            string[] devlist;
            string sql_s = " SELECT  id,root_id,title,type,model,serial,manufacturer,description,gateway_id,pin_id,ws_address From device where id=" + dev_id + "";
            SqlCommand cmd = new SqlCommand(sql_s, con2db);
            SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
            DataSet DTt = new DataSet();
            adaptor.Fill(DTt);
            DataRowCollection drgoods = DTt.Tables[0].Rows;
            int count = DTt.Tables[0].Columns.Count;
            devlist = new string[count];
            
            DataRow dtdep = drgoods[0];
            for (int i=0;i<count;i++)
            {
                devlist[i] = dtdep[i].ToString(); 
            }
            con2db.Close();
            return devlist;
        }
        // get List of device's ids
        public static int[] get_devic_id_list( SqlConnection con2db)
        {
            con2db.Close();
            con2db.Open();
            int []devlist;
            string sql_s = " SELECT DISTINCT Id From device";
            SqlCommand cmd = new SqlCommand(sql_s, con2db);
            SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
            DataSet DTt = new DataSet();
            adaptor.Fill(DTt);
            DataRowCollection drgoods = DTt.Tables[0].Rows;
            int count = DTt.Tables[0].Rows.Count;
            devlist = new int[count];
            int i = 0;
            foreach (DataRow dtdep in drgoods)
            {
                devlist[i++] =int.Parse( dtdep[0].ToString());
            }
            con2db.Close();
            return devlist;
        }
        // get #GW and GW's IDs
        public static void load_gateways(DataGridView DGV, SqlConnection con2db)
        {
            con2db.Close();
            con2db.Open();
            string sql_s = " SELECT DISTINCT Id,Type,Dpins,Apins From Gateway";
            SqlCommand cmd = new SqlCommand(sql_s, con2db);
            SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
            DataSet DTt = new DataSet();
            adaptor.Fill(DTt);
            DataRowCollection drgoods = DTt.Tables[0].Rows;
            G_Variables.No_GWs = DTt.Tables[0].Rows.Count;
            G_Variables.Gateways_ID = new int[G_Variables.No_GWs];
            G_Variables.Gateways_Title = new string[G_Variables.No_GWs];
            foreach (DataRow dtdep in drgoods)
            {

                DGV.Rows.Add(dtdep[0].ToString(), dtdep[1].ToString(), dtdep[2].ToString(), dtdep[3].ToString());
            }
            con2db.Close();
        }
        public static void load_by_type(string type, DataGridView DGV, SqlConnection con2db)
        {
            con2db.Close();
            con2db.Open();
            string sql_s = " SELECT DISTINCT dbo.device.id, dbo.device.title, dbo.building.title AS Location, dbo.device.description" +
            " FROM            dbo.device INNER JOIN" +
            "             dbo.building ON dbo.device.root_id = dbo.building.id where dbo.device.type ='" + type + "'";

            SqlDataAdapter DA = new SqlDataAdapter(sql_s, con2db);
            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            DGV.DataSource = dt;
            con2db.Close();
        }
        public static void get_type_list(SqlConnection con2db)
        {
            con2db.Close();
            con2db.Open();
            SqlCommand cmd = new SqlCommand("select distinct type  from device ", con2db);
            SqlDataAdapter dt = new SqlDataAdapter(cmd);
            DataSet st = new DataSet();
            dt.Fill(st);
            int count = st.Tables[0].Rows.Count;
            DataRowCollection dtc = st.Tables[0].Rows;
            G_Variables.Dev_types = new string[count];
            int counter = 0;
            foreach (DataRow dr in dtc)
            { G_Variables.Dev_types[counter] = dr[0].ToString(); counter++; }
        }
        public static string gettitle(int ind, SqlConnection con2db)
        {
            SqlCommand cmd = new SqlCommand("select  title  from building where id=" + ind + " ", con2db);
            string part = cmd.ExecuteScalar().ToString();
            return part;
        }
        public static string getDtitle(int ind, SqlConnection con2db)
        {
            SqlCommand cmd = new SqlCommand("select  title  from device where id=" + ind + " ", con2db);
            string part = cmd.ExecuteScalar().ToString();
            return part;
        }
        public static string getBtype(int ind, SqlConnection con2db)
        {
            SqlCommand cmd = new SqlCommand("select  type  from building where id=" + ind + " ", con2db);
            string part = cmd.ExecuteScalar().ToString();
            return part;
        }
        public static void PopulateTreeView(int directoryValue, TreeNode parentNode,SqlConnection con2db)
        {
            con2db.Close();
            con2db.Open();
            SqlCommand cmd = new SqlCommand("select distinct id,title,type from building where root_id=" + directoryValue + "", con2db);
            SqlDataAdapter dt = new SqlDataAdapter(cmd);
            DataSet st = new DataSet();
            dt.Fill(st);
            int count = st.Tables[0].Rows.Count;
            DataRowCollection dtc = st.Tables[0].Rows;
            childs = new int[count];
            parts = new string[count];
            types = new string[count];
            int counter = 0;
            foreach (DataRow dr in dtc)
            {
                childs[counter] = int.Parse(dr[0].ToString());
                parts[counter] = dr[1].ToString();
                types[counter] = dr[2].ToString(); counter++;
            }
            try
            {
                if (childs.Length != 0)
                {

                    foreach (int directory in childs)
                    {
                        string mina = G_Operations.gettitle(directory, con2db);
                        TreeNode myNode = new TreeNode(mina);
                        parentNode.Nodes.Add(myNode);
                        if (G_Operations.getBtype(directory, con2db) == "room")
                            PopulateTreeView_dev(directory, myNode,con2db);

                        PopulateTreeView(directory, myNode,con2db);

                    }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
        public static void PopulateTreeView_dev(int directoryValue, TreeNode parentNode,SqlConnection con2db)
        {
            SqlCommand ccmd = new SqlCommand("select distinct id,title from device where root_id=" + directoryValue + "", con2db);
            SqlDataAdapter dtt = new SqlDataAdapter(ccmd);
            DataSet sst = new DataSet();
            dtt.Fill(sst);
            int count = sst.Tables[0].Rows.Count;
            DataRowCollection dtcc = sst.Tables[0].Rows;
            childs = new int[count];
            parts = new string[count];

            int counter = 0;
            foreach (DataRow dr in dtcc)
            {
                childs[counter] = int.Parse(dr[0].ToString());
                parts[counter] = dr[1].ToString(); counter++;
            }
            try
            {
                if (childs.Length != 0)
                {

                    foreach (int directory in childs)
                    {
                        string mina = G_Operations.getDtitle(directory, con2db);
                        TreeNode myNode = new TreeNode(mina);
                        parentNode.Nodes.Add(myNode);
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
