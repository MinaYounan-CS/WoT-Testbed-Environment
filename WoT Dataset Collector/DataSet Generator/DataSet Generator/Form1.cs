using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace DataSet_Generator
{
    public partial class Form1 : Form
    {
        private double totallatency = 0.0;
        private DateTime LastReceiveTime=System.DateTime.Now;
        private SerialPort serialPort = new SerialPort();
        private string cur_message = "";
        private string cur_response = "";
        private string cur_GW_ID = "";

        private int count_msg=0;
        private string board = "#";
        private bool pause = false; int pauses = 0;

        private int count_sensor_records=0,count_file_rows=0;
        private string DS_File_Headers = "";
        private string Cur_Type="",Cur_Ds_File = "", Cur_File_Sheet = "";
        private int columns = 0;
        private bool start_record = false;
        private string rec_state = "network";//rec_state = One Gateway like "arduino uno" or "network";
        private string[] headers;
        // Threads
        Thread t;
        ManualResetEvent runThread = new ManualResetEvent(false);

        // Delegates
        private delegate void DelegateAddToList(string msg);
        private DelegateAddToList m_DelegateAddToList;
        private delegate void DelegateStopPerfmormClick();
        private DelegateStopPerfmormClick m_DelegateStop;

        private static SqlConnection con2db = new SqlConnection();
       
        #region 
        //private int[] childs;
        //private string[] parts;
        //private string[] types;
        #endregion 

        public Form1()
        {
            InitializeComponent();
            txt_dt_start.Text = txt_dt_end.Text = System.DateTime.Now.ToString();
            DGV_Statistics[0, 0].Value = "0";
            DGV_Statistics[1, 0].Value = "0";
            DGV_Statistics[2, 0].Value = "0";
            // com ports
            string[] allSerialPorts = SerialPort.GetPortNames();
            foreach(string x in allSerialPorts)
                Com_Port.Items.Add(x);
            try
            {  Com_Port.SelectedIndex = 0; }
            catch { }
            com_format.SelectedIndex = 0;

            G_Variables.Log_Title = "SensorReadings.xls";

            TreeNode n = new TreeNode("House");
            con2db.ConnectionString = Properties.Settings.Default.ConStr;
            treeView1.Nodes.Add(n);
            G_Operations.PopulateTreeView(1, treeView1.Nodes[0],con2db);

            G_Operations.get_type_list(con2db);
          //  foreach (string x in G_Variables.Dev_types)
           // {com_locate_type.Items.Add(x);}//com_monitor_type.Items.Add(x);}
           // com_locate_type.SelectedIndex = 0;

            G_Operations.LoadCompWithCondition(com_gateway,"Type","Id"," Select Type,Id from Gateway ",con2db);
            com_gateway.SelectedIndex = 0;

            G_Operations.LoadCompWithCondition(com_locate_type, "Type", "Type", " Select distinct type from device order by type", con2db);
            com_locate_type.SelectedIndex = 0;

            G_Operations.LoadCompWithCondition(com_monitor_type, "Type", "gateway_id", " Select distinct type,gateway_id from device where gateway_id=" + com_gateway.SelectedValue + "", con2db);
            com_monitor_type.SelectedIndex = 0;

            G_Operations.load_gateways(DGV_Gateways_Available, con2db);
            G_Operations.load_devices(DGV_Dev_List, con2db,"");
        }
        // draw tree nodes 

        #region 

        //private void PopulateTreeView(int directoryValue, TreeNode parentNode)
        //{
        //    con2db.Close();
        //    con2db.Open();

        //    SqlCommand cmd = new SqlCommand( "select distinct id,title,type from building where root_id="+ directoryValue + "", con2db);
        //    SqlDataAdapter dt = new SqlDataAdapter(cmd);
        //    DataSet st = new DataSet();
        //    dt.Fill(st);
        //    int count = st.Tables[0].Rows.Count;
        //    DataRowCollection dtc = st.Tables[0].Rows;
        //    childs = new int[count];
        //    parts = new string[count];
        //    types = new string[count];
        //    int counter = 0;
        //    foreach (DataRow dr in dtc)
        //    {
        //        childs[counter] = int.Parse(dr[0].ToString());
        //        parts[counter] = dr[1].ToString();
        //        types[counter] = dr[2].ToString(); counter++;
        //    }
        //    try
        //    {
        //        if (childs.Length != 0)
        //        {
                   
        //            foreach (int directory in childs)
        //            {
        //                string mina =G_Operations.gettitle (directory,con2db);
        //                TreeNode myNode = new TreeNode(mina);
        //                parentNode.Nodes.Add(myNode);
        //                if (G_Operations.getBtype(directory,con2db) == "room")
        //                    PopulateTreeView_dev(directory, myNode);
                      
        //                PopulateTreeView(directory, myNode);
                        
        //            }
        //        }
        //    }
        //    catch (UnauthorizedAccessException) { }//parentNode.Nodes.Add("Access denied"); }
        //    //con2db.Close();
        //}
        //private void PopulateTreeView_dev(int directoryValue, TreeNode parentNode)
        //{
        //   // con2db.Close();
        //   // con2db.Open();

        //    SqlCommand ccmd = new SqlCommand("select distinct id,title from device where root_id=" + directoryValue + "", con2db);
        //    SqlDataAdapter dtt = new SqlDataAdapter(ccmd);
        //    DataSet sst = new DataSet();
        //    dtt.Fill(sst);
        //    int count = sst.Tables[0].Rows.Count;
        //    DataRowCollection dtcc = sst.Tables[0].Rows;
        //    childs = new int[count];
        //    parts = new string[count];
        
        //    int counter = 0;
        //    foreach (DataRow dr in dtcc)
        //    {
        //        childs[counter] = int.Parse(dr[0].ToString());
        //        parts[counter] = dr[1].ToString(); counter++;
        //    }
        //    try
        //    {
        //        if (childs.Length != 0)
        //        {
                   
        //            foreach (int directory in childs)
        //            {
        //                string mina =G_Operations.getDtitle(directory,con2db) ;
        //                TreeNode myNode = new TreeNode(mina);
        //                parentNode.Nodes.Add(myNode); 
        //            }
        //        }
        //    }
        //    catch (UnauthorizedAccessException) { }//parentNode.Nodes.Add("Access denied"); }
        //    //con2db.Close();
        //}
        //private string gettitle(int ind)
        //{

        //    SqlCommand cmd = new SqlCommand("select  title  from building where id=" + ind + " ", con2db);
        //    string part = cmd.ExecuteScalar().ToString();
        //    return part;

        //}
        //private string getDtitle(int ind)
        //{

        //    SqlCommand cmd = new SqlCommand("select  title  from device where id=" + ind + " ", con2db);
        //    string part = cmd.ExecuteScalar().ToString();
        //    return part;

        //}
        //private string getBtype(int ind)
        //{

        //    SqlCommand cmd = new SqlCommand("select  type  from building where id=" + ind + " ", con2db);
        //    string part = cmd.ExecuteScalar().ToString();
        //    return part;

        //}

        # endregion  //

        private void GraphsButton_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_DelegateAddToList = new DelegateAddToList(AddToList);
            m_DelegateStop = new DelegateStopPerfmormClick(close_serialport);
            t = new Thread(ReceiveThread);
            t.Start();
        }
      
        private void AddToList(string msg)
        {
            int n;
            DateTime st = System.DateTime.Now;
            if (Bt_Record.Text == "Stop Rec.")
            {
                DGV_Statistics[3, 0].Value = (st.Ticks - LastReceiveTime.Ticks) / 1000000.0;   // DGV_Statistics[3,0].Value = st.Subtract(LastReceiveTime);
                if (int.Parse(txt_time.Text) <= 0) txt_time.Text = "1000";
                textBox1.Text =Math.Truncate((((st.Ticks - LastReceiveTime.Ticks) / 10000.0) - double.Parse(txt_time.Text))).ToString();
                textBox2.Text =Math.Truncate((double.Parse(textBox2.Text) + double.Parse(textBox1.Text))).ToString();
            }
            LastReceiveTime = st; //last receive time

            if (msg.IndexOf("GW_ID=")>-1) // discovering gateways
            { 
               int ind = msg.IndexOf("=");
               int indx = (msg.IndexOf("\r")-ind-1);
                
               cur_GW_ID += msg.Substring(ind+1,indx);
               bool add_gw = false;
               int y=int.Parse(msg.Substring(ind+1)); 
               foreach(int i in G_Variables.Gateways_ID)
                   if (y == G_Variables.Gateways_ID[i]) add_gw = true;
               if (!add_gw) //if it is new one so it will be added
               { G_Variables.Gateways_ID[G_Variables.No_GWs_Availble++] = int.Parse(msg.Substring(ind + 1)); txt_time.Text = (1000 * G_Variables.No_GWs_Availble)+""; }
               for (int i = 0; i < (DGV_Gateways_Available.Rows.Count - 1); i++)
                   if (int.Parse(DGV_Gateways_Available.Rows[i].Cells[0].Value.ToString()) == y)
                       DGV_Gateways_Available.Rows[i].Cells[4].Value = Properties.Resources.on;
               
            }
            else if((msg.Contains("Connected"))||(msg.Contains("Disconnected")))
            {
                n = msg_listbox.Items.Add(msg);
                msg_listbox.SelectedIndex = n;
                msg_listbox.ClearSelected();
            }
            else
            {
                System.Data.OleDb.OleDbConnection MyConnection = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + Cur_Ds_File + "';Extended Properties='Excel 8.0;HDR=Yes'");
                n = Lb_Sensors_Data.Items.Add(msg);
                Lb_Sensors_Data.SelectedIndex = n;  Lb_Sensors_Data.ClearSelected();

                if (Bt_Record.Text == "Stop Rec.")
                {
                    count_sensor_records++;
                    DGV_Statistics[0, 0].Value = count_sensor_records + ""; lab_SensorRecords.Text = "Recieved =" + count_sensor_records + " msg.(s)";
                }
                DateTime d = new DateTime();
                d = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
                
                if ((start_record) && (msg.Contains("@")))// && (rec_state != "Network"))
                {
                    bool missing_headers_order = false;
                    try
                    {
                        int ind_under = (msg.IndexOf('_') - 1);
                        string eds = msg.Substring(2, ind_under - 1);
                        int gw = int.Parse(eds);

                        string s = msg.Substring(msg.LastIndexOf(":") + 1);
                        int x = s.LastIndexOf(',');
                        s = s.Substring(0, s.Length - 2);

                        s = s.Replace("@", "");
                        string gw_headers = "";

                        for (int i = 0; i < G_Variables.No_GWs_Availble; i++)
                            if (G_Variables.Gateways_ID[i] == gw)
                            {
                                if (Cur_Type == "Network")
                                    gw_headers = headers[i];
                                else
                                    gw_headers = headers[0];
                            }
                        x = gw_headers.LastIndexOf(',');
                        gw_headers = gw_headers.Substring(0, gw_headers.Length - 1);
                        var hlist = gw_headers.Split(',');
                        var vlist = s.Split(',');
                        // MyConnection = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + Cur_Ds_File + "';Extended Properties='Excel 8.0;HDR=Yes'");
                        MyConnection.Open();
                        System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand();

                        // to update
                        string sets = ""; string sql = "";
                        int NumOfUpdates = 0;
                        if ((Cur_Type == "Network") && (G_Variables.No_GWs_Availble > 1))
                        {
                            for (int i = 0; i < hlist.Length - 1; i++)
                            {
                                try { sets += "[" + hlist[i] + "]=" + vlist[i] + ","; }
                                catch { missing_headers_order = true; }
                            }
                            sets += "[" + hlist[hlist.Length - 1] + "]=" + vlist[vlist.Length - 1];

                            sql = " update [" + Cur_File_Sheet + "] set " + sets + " where [RecDateTime] like '" + d + "' ";
                            cmd.CommandText = sql;
                            cmd.Connection = MyConnection;
                            NumOfUpdates = cmd.ExecuteNonQuery();
                        }
                        if (NumOfUpdates == 0)
                        {
                            sets = "";
                            for (int i = 0; i < hlist.Length - 1; i++)
                                sets += "[" + hlist[i] + "],";
                            sets += "[" + hlist[hlist.Length - 1] + "]";
                            sql = "insert into [" + Cur_File_Sheet + "] ([RecDateTime]," + sets + ") values ('" + d + "'," + s + ")";

                            cmd.CommandText = sql;
                            cmd.Connection = MyConnection;

                            cmd.ExecuteNonQuery();
                            Lab_FileRows.Text = "Recorded=" + (++count_file_rows) + " row(s)";
                            DGV_Statistics[1, 0].Value = count_file_rows + " row(s)";

                            //DGV_Statistics[2, 0].Value = System.DateTime.Now.Ticks - st.Ticks / 10000000.0;
                          //  DGV_Statistics[4, 0].Value =""+ DateTime.Parse(DGV_Statistics[2, 0].Value.ToString()).Add( DateTime.Parse(DGV_Statistics[3, 0].Value.ToString());
                        }
                        MyConnection.Close();
                    }
                    catch { if (missing_headers_order) MessageBox.Show("Please Close Coordinator then press discover followed by reset rules and file creation, Press Yes for append message to continue"); }
                    DGV_Statistics[2, 0].Value = (System.DateTime.Now.Ticks - st.Ticks) / 1000000.0; //recording time
                    textBox3.Text = Math.Truncate(((System.DateTime.Now.Ticks - st.Ticks)/10000.0)).ToString();
                    textBox4.Text = Math.Truncate((double.Parse(textBox4.Text) + double.Parse(textBox3.Text))).ToString();
                    textBox5.Text = Math.Truncate((double.Parse(textBox2.Text) + double.Parse(textBox4.Text))).ToString();
                    textBox6.Text = (1-(double.Parse(textBox5.Text) / (int.Parse(DGV_Statistics[0, 0].Value.ToString()) * int.Parse(txt_time.Text)))).ToString();


                }
                else if ((start_record) && (msg.Contains("="))) //recieve arbitrary msgs according to changes from all devices at all available gateways
                {
                    //try
                    //{
                        int ind_under = (msg.IndexOf('_') - 1);
                        string eds = msg.Substring(2, ind_under - 1);
                        int gw = int.Parse(eds);

                        string s = msg.Substring(msg.LastIndexOf(":") + 1);
                        int x = s.LastIndexOf(',');
                        s = s.Substring(0, s.Length - 2);

                        string gw_headers_insert = "";
                        string gw_inser_values = "";
                        string gw_headers_update = "";
                        var vlist = s.Split(',');
                        for (int i = 0; i < vlist.Length - 1; i++)
                        {
                            var pin_value = vlist[i].Split('=');
                            string sqlstr = "SELECT  title, id, root_id FROM  dbo.device  WHERE  (gateway_id = " + gw + ") AND (pin_id = " + pin_value[0] + ")";
                            con2db.Close();
                            con2db.Open();
                            SqlCommand cmdd = new SqlCommand(sqlstr, con2db);
                            SqlDataReader r = cmdd.ExecuteReader();
                            r.Read();
                            gw_headers_insert += "[" + r.GetString(0) + "(" + r.GetInt32(1) + "/" + r.GetInt32(2) + ":" + pin_value[0] + "/" + gw + ")]";
                            gw_inser_values += pin_value[1];
                            gw_headers_update += gw_headers_insert + "=" + pin_value[1];
                            gw_headers_insert += ",";
                            gw_inser_values += ",";
                            gw_headers_update += ",";
                        }
                        var lpin_value = vlist[vlist.Length - 1].Split('=');
                        string lsqlstr = "SELECT  title, id, root_id FROM  dbo.device  WHERE  (gateway_id = " + gw + ") AND (pin_id = " + lpin_value[0] + ")";
                        con2db.Close();
                        con2db.Open();
                        SqlCommand lcmd = new SqlCommand(lsqlstr, con2db);
                        SqlDataReader lr = lcmd.ExecuteReader();
                        lr.Read();
                        gw_headers_insert += "[" + lr.GetString(0) + "(" + lr.GetInt32(1) + "/" + lr.GetInt32(2) + ":" + lpin_value[0] + "/" + gw + ")]";
                        gw_inser_values += lpin_value[1];
                        gw_headers_update += gw_headers_insert + "=" + lpin_value[1];

                        int NumOfUpdates = 0;
                        MyConnection.Close();
                        MyConnection.Open();
                        System.Data.OleDb.OleDbCommand execlcmd = new System.Data.OleDb.OleDbCommand();
                        string excelstr = " update [" + Cur_File_Sheet + "] set " + gw_headers_update + " where [RecDateTime] like '" + d + "' ";
                        //cmd =(sql, MyConnection); 
                        execlcmd.CommandText = excelstr;
                        execlcmd.Connection = MyConnection;
                        try
                        {
                            NumOfUpdates = execlcmd.ExecuteNonQuery();
                        }
                        catch { }
                        if (NumOfUpdates == 0)
                        {
                            excelstr = "insert into [" + Cur_File_Sheet + "] ([RecDateTime]," + gw_headers_insert + ") values ('" + d + "'," + gw_inser_values + ")";
                            execlcmd.CommandText = excelstr;
                            execlcmd.Connection = MyConnection;
                            execlcmd.ExecuteNonQuery();
                           Lab_FileRows.Text = "Recorded=" + (++count_file_rows) + " row(s)";
                           DGV_Statistics[1, 0].Value = count_file_rows + " row(s)";
                        }
                        MyConnection.Close();
                    //}
                    //catch { }
                } //end else if changes
                else { }
            }
            //DGV_Statistics[2, 0].Value = (System.DateTime.Now.Ticks - st.Ticks) / 1000000.0; //recording time
            if (Bt_Record.Text == "Stop Rec.")
            {
                double tl = double.Parse(DGV_Statistics[2, 0].Value.ToString()) + double.Parse(DGV_Statistics[3, 0].Value.ToString());
                totallatency += tl;
                // MessageBox.Show(""+(totallatency / (1+count_msg)));
                DGV_Statistics[4, 0].Value = totallatency;
                // DGV_Statistics[5, 0].Value = totallatency / (1 + count_sensor_records);

                DGV_Statistics[5, 0].Value =100-( 100*( (totallatency / (1 + count_sensor_records))/int.Parse(txt_time.Text)));
                list_SynTA.Items.Add(DGV_Statistics[5, 0].Value);
                list_SynTA.SelectedIndex=list_SynTA.Items.Count-1;
            }

        }

        private void close_serialport()
        {
            try
            {
                if (serialPort.IsOpen == true)
                    serialPort.Close();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            runThread.Reset();
            int n = msg_listbox.Items.Add("Connection closed.");
            msg_listbox.SelectedIndex = n;
            msg_listbox.ClearSelected();
        }
        private void ReceiveThread()
        {
            while (true)
            {
                runThread.WaitOne(Timeout.Infinite);
                while (true)
                {
                    try
                    {
                        string msg = serialPort.ReadLine();  // receive data 
                        this.Invoke(this.m_DelegateAddToList, new Object[] { "R: " + msg });
                    }
                    catch
                    {
                        try
                        { this.Invoke(this.m_DelegateStop, new Object[] { });  }
                        catch { }
                        runThread.Reset();
                        break;
                    }
                }
            }
        }

        private void OpenCloseComPort_button_Click(object sender, EventArgs e)
        {
            if (Bt_Port.Text == "Open")
            {
                try
                {
                    serialPort.PortName = Com_Port.SelectedItem.ToString();
                    serialPort.BaudRate = int.Parse(Com_Rate.SelectedItem.ToString());
                    serialPort.NewLine = "\n";

                    try
                    {
                        serialPort.Open();
                        Bt_Port.Text = "Close";
                        serialPort.WriteLine("Open");
                        cur_message = "Open";
                        groupBox2.Enabled = true;
                        pictureBox1.Image = Properties.Resources.on;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int n = msg_listbox.Items.Add("Connection established...");
                    msg_listbox.SelectedIndex = n;
                    msg_listbox.ClearSelected();

                    runThread.Set();
                }
                catch { MessageBox.Show("Please, Select Serial Port to establish connection"); }
            }
            else
            {
                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine("#Close"); cur_message = "#Close";
                    for (int i = 0; i < (DGV_Gateways_Available.Rows.Count - 1); i++)
                        DGV_Gateways_Available.Rows[i].Cells[4].Value = Properties.Resources.Stop;
                }
                else
                { try { serialPort.Open(); serialPort.WriteLine("#Close"); } catch { MessageBox.Show("Coordinator May be disconnected before","Error",MessageBoxButtons.OK,MessageBoxIcon.Error); } }
                for (int i = 0; i < G_Variables.No_GWs_Availble; i++)
                    G_Variables.Gateways_ID[i] = 0;
                G_Variables.No_GWs_Availble = 0;
                serialPort.Close();
                Bt_Port.Text = "Open";
                pictureBox1.Image = Properties.Resources.Stop;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            serialPort.WriteLine("Close"); cur_message = "Close";  serialPort.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Bt_Port.Text == "Close")
                OpenCloseComPort_button_Click(sender, e);
            else
                serialPort.Close();
            t.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                G_Variables.Log_Path = folderBrowserDialog1.SelectedPath.ToString();
                txt_path.Text = G_Variables.Log_Path;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            txt_time.Enabled = true;
            Ch_Depenencies.Checked = false;  Ch_Depenencies.Enabled = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            txt_time.Enabled = false ;
            Ch_Depenencies.Checked = true; //Ch_Depenencies.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            com_gateway.Enabled = false;// com_monitor_type.Enabled = false;
            if(Rb_DeviceType.Checked)
                G_Operations.LoadCompWithCondition(com_monitor_type, "type", "type", " Select distinct type from device ", con2db);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            com_gateway.Enabled = true; //com_monitor_type.Enabled = false;
            
            if (Rb_DeviceType.Checked)
                try { G_Operations.LoadCompWithCondition(com_monitor_type, "type", "gateway_id", " Select distinct type,gateway_id from device where gateway_id=" + com_gateway.SelectedValue + "", con2db); }
                catch { }
            else radioButton1_CheckedChanged_1( sender,  e);

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (Rb_DeviceType.Checked)
            {
                Ch_additionalSensors.Enabled = true;
                Ch_additionalSensors.Checked = false;
                com_monitor_type.Enabled = true; //com_gateway.Enabled = false;
            }
            load_combobox_gateways();
        }
        private void load_combobox_gateways()
        {
            com_gateway.DataSource = null;
            com_gateway.Items.Clear();
            com_gateway.DataSource = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("value");

            for (int i = 0; i < G_Variables.No_GWs_Availble; i++)
                for (int j = 0; j < DGV_Gateways_Available.Rows.Count; j++)
                    try
                    {
                        if (int.Parse(DGV_Gateways_Available[0, j].Value.ToString()) == G_Variables.Gateways_ID[i])
                            dt.Rows.Add(DGV_Gateways_Available[1, j].Value.ToString(), DGV_Gateways_Available[0, j].Value.ToString());
                    }
                    catch { }
            com_gateway.DataSource = dt;
            com_gateway.DisplayMember = "Name";
            com_gateway.ValueMember = "value";
        }
        private void button6_Click(object sender, EventArgs e)
        {
          G_Operations.load_by_type(com_locate_type.Text, DGV_Type_List, con2db);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (DateTime.Parse(txt_dt_start.Text) >= DateTime.Parse(txt_dt_end.Text)) // if (Dt_start_Time.Value >= Dt_End_Time.Value)
                MessageBox.Show("Please Select generation time period,Start Time < End Time", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                string file_name = ""; Lb_Rules.Items.Clear(); Lb_Headers.Items.Clear();
                if ((Rd_Time.Checked) && (int.Parse(txt_time.Text) <= 0)) // if time was selected and was set to zero or less
                    MessageBox.Show(" Set time to value >0, Default value is one 1000 m.second ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    if (Rb_Gateway.Checked)
                        try { board = com_gateway.SelectedValue.ToString() + "_" + com_gateway.Text + ":"; file_name = board.Substring(0, board.Length - 1); }
                        catch { MessageBox.Show("Discover Network then Select Specific Gateway if whole Network isn't needed", "info", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    else { board = "#"; file_name = "Network"; }
                    //------------------------------------------------------------------------------<Time to send information>
                    Lb_Rules.Items.Add(board + "Quick");
                    if (Rd_StatChanges.Checked) { Lb_Rules.Items.Add(board + "Feed_Changes_on"); file_name += "_Changes_"; }
                    else if (Rd_Statechange_off.Checked) { Lb_Rules.Items.Add(board + "Feed_Changes_off"); file_name += "_Time_"; }
                    else /* (Rd_Time.Checked)*/ { if (int.Parse(txt_time.Text) >= 0)  Lb_Rules.Items.Add(board + "Time=" + txt_time.Text); file_name += "_Time_"; }
                    //------------------------------------------------------------------------------<Device type>
                    if (Rb_DeviceType.Checked)
                    {
                        string s = "";
                        if (com_monitor_type.Text.IndexOf(' ') > 0) { s = com_monitor_type.Text.Substring(0, com_monitor_type.Text.IndexOf(' ')); }
                        else { s = com_monitor_type.Text; }
                        Lb_Rules.Items.Add(board + s);
                        file_name += com_monitor_type.Text;
                        if (Ch_additionalSensors.Checked)
                        {
                            string[] list = Get_Depdon_List(s);
                            for (int i = 0; i < list.GetLength(0); i++)
                                Lb_Rules.Items.Add(board + list[i]);
                        }
                    }
                    else
                    {
                        Lb_Rules.Items.Add(board + "All"); file_name += "All";
                        if (Ch_additionalSensors.Checked)
                        { Lb_Rules.Items.Add(board + "Feed_Temp_on"); Lb_Rules.Items.Add(board + "Feed_LDR_on"); }
                        else
                        { Lb_Rules.Items.Add(board + "Feed_Temp_off"); Lb_Rules.Items.Add(board + "Feed_LDR_off"); }
                    }
                    //------------------------------------------------------------------------------<Depenencies>
                    if (Ch_Depenencies.Checked) { Lb_Rules.Items.Add(board + "Temp_on"); Lb_Rules.Items.Add(board + "LDR_on"); }
                    else { Lb_Rules.Items.Add(board + "Temp_off"); Lb_Rules.Items.Add(board + "LDR_off"); }
                    //------------------------------------------------------------------------------<Dataset_on>
                    Lb_Rules.Items.Add(board + "Normal"); Lb_Rules.Items.Add(board + "Dataset_on");
                    Lb_Rules.SelectedIndex = Lb_Rules.Items.Count - 1; Lb_Rules.ClearSelected();
                    G_Variables.Log_Title = file_name;
                    set_headers_rec_order();

                    Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Rules: Selected"); Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Headers: Selected");
                } //end else block which mean if correct path selected then we will initialize rules and headers
            }
        }
        private string[] Get_Depdon_List(string type)
        {
            string[] list;
            string str = "(SELECT DISTINCT device_1.type  " +
                                "       FROM            dbo.device INNER JOIN " +
                                "                                    dbo.Dependencies ON dbo.device.id = dbo.Dependencies.device INNER JOIN " +
                                "                                     dbo.device AS device_1 ON dbo.Dependencies.dependon_device = device_1.id " +
                                "       WHERE        dbo.device.type = '" + type + "')";
            con2db.Close();
            con2db.Open();
            SqlCommand cmd = new SqlCommand(str, con2db);
            SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
            DataSet DTt = new DataSet();
            adaptor.Fill(DTt);
            DataRowCollection drgoods = DTt.Tables[0].Rows;
            list = new string[DTt.Tables[0].Rows.Count]; int c = 0;
            foreach (DataRow dtdep in drgoods)
            {
                list[c++] = dtdep[0].ToString();
            }
            con2db.Close();
            return list;
        }
        private int set_header_single_gw(int gw_id,int stp)
        {
            string condition = "";
            int count_headers = 0;
            if (Rb_All_Types.Checked)
                condition = "( Select title, id, root_id, pin_id from device where gateway_id=" + gw_id + " )";
            else //if ((Rb_DeviceType.Checked) && (!Ch_additionalSensors.Checked))
                condition = "( Select distinct title, id , root_id , pin_id ,type  from device where gateway_id=" + gw_id + "  and type='" + com_monitor_type.Text + "' )";


            if ((Rb_DeviceType.Checked) && (Ch_additionalSensors.Checked))
            {
                condition += " union (SELECT DISTINCT device_1.title, device_1.id , device_1.root_id, device_1.pin_id ,device_1.type  " +
                            "       FROM            dbo.device INNER JOIN " +
                            "                                    dbo.Dependencies ON dbo.device.id = dbo.Dependencies.device INNER JOIN " +
                            "                                     dbo.device AS device_1 ON dbo.Dependencies.dependon_device = device_1.id " +
                            "       WHERE        (device_1.gateway_id =" + gw_id + ") AND (dbo.device.type = '" + com_monitor_type.Text + "'))";

            }
            condition += " order by type,pin_id ";
            con2db.Close();
            con2db.Open();
            SqlCommand cmd = new SqlCommand(condition, con2db);
            SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
            DataSet DTt = new DataSet();
            adaptor.Fill(DTt);
            DataRowCollection drgoods = DTt.Tables[0].Rows;
            foreach (DataRow dtdep in drgoods)
            {
                string s = dtdep[0].ToString() + "(" + dtdep[1].ToString() + "/" + dtdep[2].ToString() + ":" + dtdep[3].ToString() + "/" + gw_id + ")";
                Lb_Headers.Items.Add(s); headers[stp] += (s + ",");
                count_headers++;
            }
            con2db.Close();
            return count_headers;
        }
        private void set_headers_rec_order()
        {
            int count_headers = 0;
            Lb_Headers.Items.Add("RecDateTime"); count_headers++;

            if (Rb_Whole_Network.Checked)
            {
                headers = new string[G_Variables.No_GWs_Availble];
                for (int i = 0; i < G_Variables.No_GWs_Availble; i++)
                    count_headers += set_header_single_gw(G_Variables.Gateways_ID[i], i);
            }
            else
            {
                headers = new string[1];
                if (G_Variables.No_GWs_Availble > 0)
                    count_headers += set_header_single_gw(int.Parse(com_gateway.SelectedValue.ToString()), 0);
            }
            
            if (Lb_Headers.Items.Count == 1)
            {
                Lb_Headers.Items.Clear(); count_headers = 0;
                MessageBox.Show("Ensure that Network was discovered and Devices have been connected to their gateways!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            lab_headers.Text = "=" + count_headers;
            for (int i = 1; i < Lb_Headers.Items.Count; i++) 
                DGV_Statistics_SThs.Rows.Add(Lb_Headers.Items[i].ToString(),"","");
        }
        ///-------------------------------------------------
        #region

        //private void set_headers_rec_order()
        //{
        //    int count_headers = 0; string condition = "";
        //    Lb_Headers.Items.Add("RecDateTime"); count_headers++;
        //    headers = new string[G_Variables.No_GWs_Availble];
        //    for (int i = 0; i < G_Variables.No_GWs_Availble; i++)
        //    {
        //        condition = "";
        //        if (Rb_All_Types.Checked)
        //            condition = "( Select title, id, root_id, pin_id from device where gateway_id=" + G_Variables.Gateways_ID[i] + " )";
        //        else //if ((Rb_DeviceType.Checked) && (!Ch_additionalSensors.Checked))
        //            condition = "( Select distinct title, id , root_id , pin_id ,type  from device where gateway_id=" + G_Variables.Gateways_ID[i] + "  and type='" + com_monitor_type.Text + "' )";


        //        if ((Rb_DeviceType.Checked) && (Ch_additionalSensors.Checked))
        //        {
        //            condition += " union (SELECT DISTINCT device_1.title, device_1.id , device_1.root_id, device_1.pin_id ,device_1.type  " +
        //                        "       FROM            dbo.device INNER JOIN " +
        //                        "                                    dbo.Dependencies ON dbo.device.id = dbo.Dependencies.device INNER JOIN " +
        //                        "                                     dbo.device AS device_1 ON dbo.Dependencies.dependon_device = device_1.id " +
        //                        "       WHERE        (device_1.gateway_id =" + G_Variables.Gateways_ID[i] + ") AND (dbo.device.type = '" + com_monitor_type.Text + "'))";

        //        }
        //        condition += " order by type,pin_id ";
        //        con2db.Close();
        //        con2db.Open();
        //        SqlCommand cmd = new SqlCommand(condition, con2db);
        //        SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
        //        DataSet DTt = new DataSet();
        //        adaptor.Fill(DTt);
        //        DataRowCollection drgoods = DTt.Tables[0].Rows;
        //        foreach (DataRow dtdep in drgoods)
        //        {
        //            string s = dtdep[0].ToString() + "(" + dtdep[1].ToString() + "/" + dtdep[2].ToString() + ":" + dtdep[3].ToString() + "/" + G_Variables.Gateways_ID[i] + ")";
        //            Lb_Headers.Items.Add(s); headers[i] += (s + ",");
        //            count_headers++;
        //        }

        //    }

        //    con2db.Close();
        //    if (Lb_Headers.Items.Count == 1)
        //    {
        //        Lb_Headers.Items.Clear(); count_headers = 0;
        //        MessageBox.Show("Ensure that Network was discovered and There were Devices, that already have been connected on them !", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    lab_headers.Text = "=" + count_headers;
        //}
        ///------------------------------------------------
        //private void set_headers()
        //{
        //   // bool add_sensor=false;
        //    int count_headers = 0;
        //    string condition = " Select id, title, root_id,gateway_id,pin_id from device ";
        //    if ((Rb_Whole_Network.Checked) && (Rb_All_Types.Checked))
        //    { condition += ""; rec_state = "Network"; }
        //    else if ((Rb_Whole_Network.Checked) && (Rb_DeviceType.Checked))
        //    { condition += " where type='" + com_monitor_type.Text + "'"; rec_state = "Network"; }
        //    else if ((Rb_Gateway.Checked) && (Rb_All_Types.Checked))
        //    { condition += " where gateway_id= " + com_gateway.SelectedValue + ""; rec_state = com_gateway.SelectedValue.ToString(); }
        //    else
        //    {
        //        condition += " where gateway_id= " + com_gateway.SelectedValue + " and  type='" + com_monitor_type.Text + "'"; rec_state = com_gateway.SelectedValue.ToString();
        //        // if((Ch_additionalSensors.Checked) && (!Rb_All_Types.Checked))
        //        //  add_sensor = true;
        //    }
        //    condition += " order by  type, id ,gateway_id";

        //    con2db.Close();
        //    con2db.Open();
        //    SqlCommand cmd = new SqlCommand(condition, con2db);
        //    SqlDataAdapter adaptor = new SqlDataAdapter(cmd);
        //    DataSet DTt = new DataSet();
        //    adaptor.Fill(DTt);
        //    DataRowCollection drgoods = DTt.Tables[0].Rows;
        //    count_headers = DTt.Tables[0].Rows.Count;// +1; //for time we add 1
        //    //lab_headers.Text = "=" + count_headers;
        //    Lb_Headers.Items.Add("RecDateTime");
        //    foreach (DataRow dtdep in drgoods)
        //    {
        //        string s = dtdep[1].ToString() + "(" + dtdep[0].ToString() + "/" + dtdep[2].ToString() + ":" + dtdep[4].ToString() + "/" + dtdep[3].ToString() + ")";
        //       Lb_Headers.Items.Add(s);
        //    }

        //    if ((Rb_Whole_Network.Checked)&&(Rb_DeviceType.Checked) && (Ch_additionalSensors.Checked))
        //    {
        //        condition = " SELECT DISTINCT device_1.id, device_1.title, device_1.root_id, device_1.gateway_id, dbo.device.type" +
        //                    " FROM            dbo.device INNER JOIN" +
        //                    "                          dbo.Dependencies ON dbo.device.id = dbo.Dependencies.device INNER JOIN" +
        //                    "                          dbo.device AS device_1 ON dbo.Dependencies.dependon_device = device_1.id" +
        //                    " WHERE           (dbo.device.type = '" + com_monitor_type.Text + "')";
        //    }
        //    else if ((Rb_Gateway.Checked) && (Rb_DeviceType.Checked) && (Ch_additionalSensors.Checked))
        //    {
        //        condition = " SELECT DISTINCT device_1.id, device_1.title, device_1.root_id, device_1.gateway_id, dbo.device.type" +
        //                    " FROM            dbo.device INNER JOIN" +
        //                    "                          dbo.Dependencies ON dbo.device.id = dbo.Dependencies.device INNER JOIN" +
        //                    "                          dbo.device AS device_1 ON dbo.Dependencies.dependon_device = device_1.id" +
        //                    " WHERE        (dbo.device.gateway_id = "+int.Parse(com_gateway.SelectedValue.ToString())+") AND (dbo.device.type = '"+com_monitor_type.Text+"')";

        //    }
        //    if ((Rb_DeviceType.Checked) && (Ch_additionalSensors.Checked))
        //    {
        //        cmd = new SqlCommand(condition, con2db);
        //        adaptor = new SqlDataAdapter(cmd);
        //        DTt = new DataSet();
        //        adaptor.Fill(DTt);
        //        drgoods = DTt.Tables[0].Rows;
        //        count_headers+=DTt.Tables[0].Rows.Count;
        //       // lab_headers.Text = "=" + count_headers;

        //        foreach (DataRow dtdep in drgoods)
        //        {
        //            string s = dtdep[1].ToString() + "(" + dtdep[0].ToString() + "/" + dtdep[2].ToString() + ":" + dtdep[3].ToString() + ")";
        //            Lb_Headers.Items.Add(s);
        //        }
        //    }
        //    con2db.Close();
        //    if (Lb_Headers.Items.Count == 1)
        //        Lb_Headers.Items.Clear();
        //    else
        //        lab_headers.Text = "=" + (++count_headers);
        //}
        #endregion
        ///-------------------------------------------------
        private void create_datasetfile()
        {
            System.Data.OleDb.OleDbConnection MyConnection;
         
            string s=""; 
            string t = "_"+DateTime.Now.Year+"-"+DateTime.Now.Month+"-"+DateTime.Now.Day+"-h" + DateTime.Now.ToLocalTime().Hour.ToString();
            txt_FileName.Text = G_Variables.Log_Title + t;
            s = G_Variables.Log_Path + "\\" + G_Variables.Log_Title + t;
            bool continue_work = true; bool append = false;
            if(File.Exists(s + ".XLS") == true)
            {
                DialogResult dr = MessageBox.Show(" File was Exist at that path with the same title, because another file was created within the same hour with the same criteria \n Do you want to continue process replacing existing file with that new file ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                { File.Delete(s + ".XLS"); continue_work = true; }
                else
                {
                    dr = MessageBox.Show(" Do You want to continue using the same file to append data on it?", "info.", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.No)
                    {
                        dr = MessageBox.Show(" Do You want to continue using an alternative title which will be (prev. titel plus minutes)?", "info.", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dr == DialogResult.OK)
                        {
                            s += "-m" + DateTime.Now.ToLocalTime().Minute.ToString();
                            txt_FileName.Text = G_Variables.Log_Title + t + "-m" + DateTime.Now.ToLocalTime().Minute.ToString(); ;
                            continue_work = true;
                        }
                        else
                            continue_work = false;
                    }
                    else append = true;
                    
                }
            }
            if(continue_work) 
            {
                try
                {
                    columns = Lb_Headers.Items.Count;
                    if (Lb_Rules.Items[1].ToString().Contains('#'))
                        Cur_Type = "Network";
                    else
                        Cur_Type = "Gateway";
                    Cur_Ds_File = s;
                    Cur_File_Sheet = G_Variables.Log_Title;
                    string header = "";
                    MyConnection = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + Cur_Ds_File + "';Extended Properties='Excel 8.0;HDR=Yes'");
                    MyConnection.Open();
                    if( Lb_Headers.Items.Count>0)
                        header += "[" + Lb_Headers.Items[0] + "] datetime,";
                    for (int i = 1; i < Lb_Headers.Items.Count - 1; i++)
                        header += "[" + Lb_Headers.Items[i] + "] int,";
                    header += "[" + Lb_Headers.Items[Lb_Headers.Items.Count - 1] + "] int";
                    if (!append)
                    {
                        System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand("create table [" + Cur_File_Sheet + "] (" + header + " ) ", MyConnection);
                        cmd.ExecuteNonQuery();
                    }
                    MyConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Bt_DiscNetwork_Click(object sender, EventArgs e)
        {
            Rb_Whole_Network.Checked = true;
            if (serialPort.IsOpen)
            { serialPort.Write("#Who"); cur_message = "#Who"; }
            else
                MessageBox.Show("Please Connect Port at First","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string[] allSerialPorts = SerialPort.GetPortNames();
            Com_Port.Items.Clear();
            foreach (string x in allSerialPorts)
                Com_Port.Items.Add(x);
            try
            { Com_Port.SelectedIndex = 0; }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Ch_Whole_Network.Checked)
            { G_Operations.load_devices(DGV_Dev_List, con2db, ""); }
            else
            {
                int columnIndex = DGV_Gateways_Available.CurrentCell.ColumnIndex;
                int rowIndex = DGV_Gateways_Available.CurrentCell.RowIndex;
                this.DGV_Gateways_Available.CurrentCell = this.DGV_Gateways_Available[0, rowIndex];
                string cond = " where gateway_id= " + DGV_Gateways_Available.CurrentCell.Value.ToString();
                G_Operations.load_devices(DGV_Dev_List, con2db, cond);
            }
            checkBox3_CheckedChanged(sender, e);
        }

        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Ch_Whole_Network.Checked = false;
                int columnIndex = DGV_Gateways_Available.CurrentCell.ColumnIndex;
                int rowIndex = DGV_Gateways_Available.CurrentCell.RowIndex;
                this.DGV_Gateways_Available.CurrentCell = this.DGV_Gateways_Available[0, rowIndex];
                string cond = " where gateway_id= " + DGV_Gateways_Available.CurrentCell.Value.ToString();
                G_Operations.load_devices(DGV_Dev_List, con2db, cond);
                Ch_Devices_List.Checked = false;
                Ch_Devices_List.Checked = true;
            }
            catch { }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Ch_Device_Type.Checked)
            {
                Ch_Devices_List.Checked = false;
                Rb_DeviceType.Checked = true;
                Rb_Whole_Network.Checked = true;
                G_Operations.LoadCompWithCondition(com_monitor_type, "type", "type", " Select distinct type from device ", con2db);
               // com_monitor_type.SelectedItem = com_locate_type.SelectedItem;
                
            }
        }

        private void com_locate_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Ch_Device_Type.Checked)
            {
                Rb_DeviceType.Checked = true;
                string sc = com_locate_type.Text;
                for (int i = 0; i < com_monitor_type.Items.Count; i++)
                {
                    com_monitor_type.SelectedIndex = i;
                    string ss = com_monitor_type.Text;
                    if (ss == sc)
                        break;
                }
               // if (com_monitor_type.Items.IndexOf(com_monitor_type.Items) == com_monitor_type.Items.Count - 1)
                    
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (Ch_Devices_List.Checked)
            {
                if (Ch_Whole_Network.Checked)
                { Rb_Whole_Network.Checked = true;  }
                else
                {
                    Rb_Gateway.Checked = true;
                    int columnIndex = DGV_Gateways_Available.CurrentCell.ColumnIndex;
                    int rowIndex = DGV_Gateways_Available.CurrentCell.RowIndex;
                    this.DGV_Gateways_Available.CurrentCell = this.DGV_Gateways_Available[0, rowIndex];
                    com_gateway.SelectedValue = this.DGV_Gateways_Available.CurrentCell.Value;
                }
                Rb_All_Types.Checked = true;
                Ch_Device_Type.Checked = false;
            }
            else
            {
                Rb_Whole_Network.Checked = true;
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //File_type = G_Variables.Log_Title; 
            start_record = false;
            if (Bt_Start_Monitoring.Text == "Start")
            {
                if (serialPort.IsOpen)
                {
                    Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Rules.: Sent(NW Domain)");
                    Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("DataCol.: Start");
                    Bt_Start_Monitoring.Text = "Pause";
                    timer1.Start();
                }
                else { MessageBox.Show("Open Coordinator Serial Port at First","Information",MessageBoxButtons.OK,MessageBoxIcon.Information); }
            }
            else
            {
                try
                {
                    Bt_Start_Monitoring.Text = "Start";
                    serialPort.Write(board + "Dataset_off");
                    if (Bt_Record.Text == "Stop Rec.")
                    {  start_record = false; Bt_Record.Text = "Start Rec."; Bt_Record.BackgroundImage = Properties.Resources.Rec_Run; }
                    Lb_Processes.Items.Add("DataCol.: Pause");
                }
                catch { }
            }
            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            try { Lb_Rules.Items.RemoveAt(Lb_Rules.SelectedIndex); Lb_Rules.SelectedIndex = Lb_Rules.Items.Count - 1; }
            catch { }
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (Rb_All_Types.Checked)
            { com_monitor_type.Enabled = false; Ch_additionalSensors.Checked = true; Ch_additionalSensors.Enabled = false; }
            else
            { com_monitor_type.Enabled = true; Ch_additionalSensors.Checked = false; Ch_additionalSensors.Enabled = true; }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            try { 
                Lb_Rules.Items.Clear(); Lb_Headers.Items.Clear(); lab_headers.Text = " =0";
                Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Rules: Deleted"); 
                Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Headers: Deleted");
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Lb_Rules.Items.Count > count_msg)
                {
                    Lb_Rules.SelectedIndex = count_msg;
                    serialPort.Write(Lb_Rules.SelectedItem.ToString());
                    count_msg++;
                }
                else
                {
                    Lb_Processes.Items.Add("Rules are sent");
                    timer1.Stop();
                    count_msg = 0;
                }
                if (Lb_Rules.Items.Count == count_msg - 1)
                    Lb_Sensors_Data.Items.Add("--------------------------------------------------- ");  
            }
            catch { MessageBox.Show("Sending Command was Corruptted Due to Closing Port Process"); timer1.Stop(); }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Lb_Sensors_Data.Items.Clear(); count_sensor_records = 0; lab_SensorRecords.Text = "Received =0 msg"; Lab_FileRows.Text = " Recorded=0 Row";
            DGV_Statistics[0, 0].Value = "0";
            DGV_Statistics[1, 0].Value = "0";
            DGV_Statistics[2, 0].Value = "0%";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if (txt_path.Text == "")
                MessageBox.Show(" Select Correct path to Store Generated Dataset ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if((G_Variables.Log_Title=="")||(Lb_Rules.Items.Count<3))
                MessageBox.Show(" Set Rules at first ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                G_Variables.Log_Path = txt_path.Text;
                create_datasetfile();
                create(txt_path.Text + "\\Syn_" + txt_FileName.Text + ".txt");
                Bt_Record.Enabled = true;
                Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Ds File: Created");
                Lb_Processes.SelectedIndex = Lb_Processes.Items.Add("Syn File: Created");
            }
        }
      //  private bool timer_interval_work = false;
        private void create(string path)
        {
            if (!File.Exists(path))
                File.Create(path);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (Bt_Record.Text == "Start Rec.") 
            { 
                start_record = true; Bt_Record.Text = "Stop Rec."; Bt_Record.BackgroundImage = Properties.Resources.Rec_Stop; timer2.Start();
                DGV_Statistics[6, 0].Value = DateTime.Parse(txt_dt_end.Text).Subtract(DateTime.Parse(txt_dt_start.Text));
               // DGV_Statistics[6, 0].Value = Dt_End_Time.Value.Subtract(Dt_start_Time.Value);
               // double x = Dt_End_Time.Value.Subtract(Dt_start_Time.Value).TotalSeconds * 1000 * G_Variables.No_GWs_Availble / int.Parse(txt_time.Text);
                double x = DateTime.Parse(txt_dt_end.Text).Subtract(DateTime.Parse(txt_dt_start.Text)).TotalSeconds * 1000 * G_Variables.No_GWs_Availble / int.Parse(txt_time.Text);
                x = Math.Round(x, 0) + G_Variables.No_GWs_Availble;
                DGV_Statistics[7, 0].Value = x+ "";
            }
            else
            { 
                start_record = false; Bt_Record.Text = "Start Rec."; Bt_Record.BackgroundImage = Properties.Resources.Rec_Run; timer2.Stop();
                    

                StreamWriter Sw = new StreamWriter("D:\\test.txt");
                for (int i = 0; i < list_SynTA.Items.Count; i++)
                    Sw.WriteLine(list_SynTA.Items[i].ToString());
               
                Sw.Dispose(); 
                Sw.Close();
            }
        }

       

        private void com_gateway_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (com_gateway.Items.Count > 0)
            {
                if (com_monitor_type.Items.Count > 0)
                { com_monitor_type.DataSource = null; com_monitor_type.Items.Clear(); }
                int id = 0;
                try { id = int.Parse(com_gateway.SelectedValue.ToString()); }
                catch { }
                G_Operations.LoadCompWithCondition(com_monitor_type, "type", "gateway_id", " Select distinct type,gateway_id from device where gateway_id=" + id + "", con2db);
                if (com_monitor_type.Items.Count > 0)
                    com_monitor_type.SelectedIndex = 0;
            }
        }
        private void com_gateway_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                com_gateway_SelectedIndexChanged(sender, e);
            }
            catch { }
        }

        private void Rb_Gateway_Click(object sender, EventArgs e)
        {
            radioButton1_CheckedChanged(sender, e);
        }

        private void Rb_DeviceType_Click(object sender, EventArgs e)
        {
            G_Operations.LoadCompWithCondition(com_monitor_type, "Type", "type", " Select distinct type from device order by type", con2db);
            com_monitor_type.SelectedIndex = 0;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try { txt_time.Text = (50000 / G_Variables.No_GWs_Availble) + ""; }
            catch { txt_time.Text = "0"; }
        }

        private void Rd_Statechange_off_CheckedChanged(object sender, EventArgs e)
        {
            txt_time.Enabled = false;
            Ch_Depenencies.Checked = false; Ch_Depenencies.Enabled = true;
        }

        private void Bt_Dev_Formats_Click(object sender, EventArgs e)
        {
            if (rb_excel.Checked)
            {
                G_Operations.write_hierarchicals_excel(txt_path.Text, con2db);
                if (ch_SeparatFiles.Checked)
                {
                    int[] devlist = G_Operations.get_devic_id_list(con2db);
                    if (com_format.Text == "microformat")
                    {
                        for (int i = 0; i < devlist.Length; i++)
                            G_Operations.write_microformate(con2db, devlist[i], txt_path.Text, "");
                    }
                    else if (com_format.Text == "microdata")
                    {
                        for (int i = 0; i < devlist.Length; i++)
                            ;
                    }
                    else
                    {
                        for (int i = 0; i < devlist.Length; i++)
                            ;
                    }
                }
               
            }
            else
            {
                if (com_format.Text == "microformat")
                {
                    G_Operations.write_WoT_microformat(con2db, txt_path.Text);
                }
                else if (com_format.Text == "microdata")
                {
                }
                else if (com_format.Text == "RDF")
                {
                }
                else if (com_format.Text == "all formats-sep.files")
                {

                }
                else  //shuffle formats-sep.files
                {
                }
            }
        }

        private void txt_FileName_MouseHover(object sender, EventArgs e)
        {
        //    txt_FileName.Tag = txt_FileName.Text;
           toolTip1.SetToolTip(txt_FileName, txt_FileName.Text);
        }

        private void rb_sel_format_CheckedChanged(object sender, EventArgs e)
        {
            ch_SeparatFiles.Checked = false;
            ch_SeparatFiles.Enabled = false;
        }

        private void rb_excel_CheckedChanged(object sender, EventArgs e)
        {
            ch_SeparatFiles.Enabled = true;
        }
      

        private void timer2_Tick(object sender, EventArgs e)
        {
            //if (Dt_start_Time.Value.ToLongTimeString() == Dt_End_Time.Value.ToLongTimeString())
            //{ timer2.Stop(); button4_Click(sender, e); button1_Click_1(sender, e); }
            //else
            //{
            //    Dt_start_Time.Value = Dt_start_Time.Value.AddSeconds(1);
            //}
            if (txt_dt_start.Text==txt_dt_end.Text)
            { timer2.Stop(); button4_Click(sender, e); button1_Click_1(sender, e); }
            else
            {
                txt_dt_start.Text = (DateTime.Parse(txt_dt_start.Text).AddSeconds(1)).ToString();
            }
        }

        private void Bt_About_Click(object sender, EventArgs e)
        {
            about frm = new about();
            frm.ShowDialog();
        }

           //--------------------------
    }
}
