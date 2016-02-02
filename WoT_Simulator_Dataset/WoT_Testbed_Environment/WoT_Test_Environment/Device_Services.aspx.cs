using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.CodeDom;
using System.Web.Services.Description;
using System.IO;
using System.CodeDom.Compiler;
using System.Xml.Serialization;
using System.Net;
using System.Data;
using System.Data.OleDb;
using System.Web.UI.DataVisualization.Charting;

namespace WoT_Test_Environment
{
    public partial class Device_Services : System.Web.UI.Page
    {
        int x = 0;
        private Type service;
        private MethodInfo[] methodInfo;
        private Uri uri;
        private string[] listurl;

        //  private MethodInfo[] methodInfo;
        private ParameterInfo[] param;

        private Type[] paramTypes;
        private properties myProperty;
        private string MethodName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                get_device_details();
                // Service_Info.service_info_methods();
                DynamicInvocation();
                load_bullets();

            }
        }
        static string path_root = "C:\\WoT Testbed\\";
        private string filename = path_root + "WoT_Structure.xls";//"D:\\Master Files\\Codes _2014\\Generated_WoT_Dataset\\
        // private string filename = "D:\\Master Files\\Codes _2014\\Generated_WoT_Dataset\\WoT_Structure.xls";
        
        private string Title2 = "Device";

        private string filereadings = path_root + "Network_Time_All_2015-01-01-h0.XLS";
        private string Title3 = "Network_Time_All";
        private void get_device_details()
        {
            try
            {
                string SQL = " select [title],[id],[building_id],[pin_id],[gateway_id],[type],[model],[serial],[manufacturer],[description] FROM [" + Title2 + "] WHERE id=@Id ";
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
                        OleDbDataReader EDR = cmd.ExecuteReader();
                        EDR.Read();
                        string code = EDR.GetValue(0).ToString() + "(" + EDR.GetValue(1).ToString() + "/" + EDR.GetValue(2).ToString() + ":" + EDR.GetValue(3).ToString() + "/" + EDR.GetValue(4).ToString() + ")";
                        Session["Dev_ExcelTitle"] = code;
                    }

                    ExcelCon.Close();
                }
                GridView1.DataSource = room;
                GridView1.DataBind();
            }
            catch {  Response.Redirect("~/Default.aspx"); }
        }
        private void load_bullets()
        {
            uri = new Uri("http://localhost:60377/Service1.asmx?wsdl");
            BulletedList1.DisplayMode = BulletedListDisplayMode.HyperLink;
            ListItem li;
            string test = uri.ToString();//"http://localhost:60377/Service1.asmx";
            test = test.Substring(0, test.IndexOf('?'));

            for (int j = 0; j < listurl.Length; j++)
            {
                li = new ListItem();
                li.Text = listurl[j];// listurl[j];
                DropDownList1.Items.Add(listurl[j]);//listurl[j]);
                li.Value = test + "?op=" + li.Text;
                BulletedList1.Items.Add(li);

            }
            try { DropDownList1.SelectedIndex = 0; }
            catch { }
        }

        private void DynamicInvocation()
        {
            try
            {
                uri = new Uri(Session["Deviceurl"].ToString() + "?wsdl");

            }
            catch { uri = new Uri("http://localhost:60377/Service1.asmx?wsdl"); }
            WebRequest webRequest = WebRequest.Create(uri);
            System.IO.Stream requestStream = webRequest.GetResponse().GetResponseStream();
            // Get a WSDL file describing a service
            ServiceDescription sd = ServiceDescription.Read(requestStream);
            string sdName = sd.Services[0].Name;

            // Initialize a service description servImport
            ServiceDescriptionImporter servImport = new ServiceDescriptionImporter();
            servImport.AddServiceDescription(sd, String.Empty, String.Empty);
            servImport.ProtocolName = "Soap";
            servImport.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;

            CodeNamespace nameSpace = new CodeNamespace();
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(nameSpace);
            // Set Warnings
            ServiceDescriptionImportWarnings warnings = servImport.Import(nameSpace, codeCompileUnit);

            if (warnings == 0)
            {
                StringWriter stringWriter = new StringWriter(System.Globalization.CultureInfo.CurrentCulture);
                Microsoft.CSharp.CSharpCodeProvider prov = new Microsoft.CSharp.CSharpCodeProvider();
                prov.GenerateCodeFromNamespace(nameSpace, stringWriter, new CodeGeneratorOptions());

                string[] assemblyReferences = new string[2] { "System.Web.Services.dll", "System.Xml.dll" };
                CompilerParameters param = new CompilerParameters(assemblyReferences);
                param.GenerateExecutable = false;
                param.GenerateInMemory = true;
                param.TreatWarningsAsErrors = false;
                param.WarningLevel = 4;

                CompilerResults results = new CompilerResults(new TempFileCollection());
                results = prov.CompileAssemblyFromDom(param, codeCompileUnit);
                Assembly assembly = results.CompiledAssembly;
                service = assembly.GetType(sdName);
                Session["service"] = service;
                methodInfo = service.GetMethods();
                Session["methodInfo"] = methodInfo;
                int c = 0;
                foreach (MethodInfo t in methodInfo)
                {
                    if (t.Name == "Discover")
                        break;
                    c++;
                }
                listurl = new string[c]; c = 0;
                foreach (MethodInfo t in methodInfo)
                {
                    if (t.Name == "Discover")
                        break;
                    listurl[c++] = t.Name;
                }

                Session["listurl"] = listurl;
            }

        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            methodInfo = (MethodInfo[])Session["methodInfo"];
            string MethodName_ajax = "monitor";
            int indx = 0;
            //  param = (ParameterInfo[])Session["param"];
            //  ParameterInfo[] param_ajax = new ParameterInfo[param.Length];
            listurl = (string[])Session["listurl"];
            for (int i = 0; i < listurl.Length; i++)
                if (listurl[i] == MethodName_ajax)
                { indx = i; break; }
            ParameterInfo[] param_ajax = methodInfo[indx].GetParameters();
            properties myProperty_ajax = new properties(param_ajax.Length);// Service_Info.myProperty = new properties(Service_Info.param.Length);

            // Get the Parameters Type
            Type[] paramTypes_ajax = new Type[param_ajax.Length];

            for (int i = 0; i < paramTypes_ajax.Length; i++)
            {
                paramTypes_ajax[i] = param_ajax[i].ParameterType;
            }
            int ind = 0;
            foreach (ParameterInfo temp in param_ajax)
            {
                myProperty_ajax.TypeParameter[ind++] = temp.ParameterType;
            }
            myProperty_ajax.MyValue[0] = (string)Session["Dev_ExcelTitle"];//"Switch_Toshiba1(7/9:40/2)";
            object[] param1 = new object[param_ajax.Length];
            for (int i = 0; i < param_ajax.Length; i++)
            {
                param1[i] = Convert.ChangeType(myProperty_ajax.MyValue[i], myProperty_ajax.TypeParameter[i]);
            }
            service = (Type)Session["service"];
            foreach (MethodInfo t in methodInfo)
                if (t.Name == MethodName_ajax)
                {
                    //Invoke Method
                    Object obj = Activator.CreateInstance(service);
                    Object response = t.Invoke(obj, param1);
                    lb_Cur_State.Text = ("Current Reading Value = " + response.ToString());
                    break;
                }
            //---------------------------------------------------------------
            //  Random rnd = new Random();
            //  chtCategoriesProductCount.Series["Categories"].Points.AddXY(x,rnd.Next(5, 20));
            //  x++;
            BindChart();
        }
        protected void BindChart()
        {
            System.Data.OleDb.OleDbConnection ExcelCon = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + filereadings + "';Extended Properties=Excel 8.0;");
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ExcelCon.Open(); int m=0;int h=0;
            string date2 = DateTime.Parse("01/01/2015 " + DateTime.Now.Hour + ":" + (DateTime.Now.Minute+1) + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
           // string date2 = DateTime.Parse("01/01/2016 " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            if (DateTime.Now.Minute >= 20)
                m = DateTime.Now.Minute - 20;
            else
            { h = DateTime.Now.Hour - 1; m = 59 - (20 - DateTime.Now.Minute); }
            string date1 = DateTime.Parse("01/01/2015 " + h + ":" + m + ":" + DateTime.Now.Second + "").ToString("dd/MM/yyyy HH:mm:ss"); //DateTime.Now.ToString("dd/MM/yyyy HH:mm");
         
            string cmdstr = "select top 20 [RecDateTime], [" + (string)Session["Dev_ExcelTitle"] + "] from [" + Title3 + "] where [RecDateTime] BETWEEN @D1 AND @D2";
            OleDbCommand cmd = new OleDbCommand(cmdstr, ExcelCon);

            cmd.Parameters.Add("@D1", OleDbType.Date).Value = date1;
            cmd.Parameters.Add("@D2", OleDbType.Date).Value = date2;

            OleDbDataAdapter adp = new OleDbDataAdapter(cmd);

            adp.Fill(ds);
            dt = ds.Tables[0];

            string[] x = new string[dt.Rows.Count];
            int[] y = new int[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                x[i] = dt.Rows[i][0].ToString();
                y[i] = Convert.ToInt32(dt.Rows[i][1]);
            }
          
            ExcelCon.Close();
            for (int i = 0; i < x.Length; i++) 
                 x[i] =DateTime.Now.Day+"/"+DateTime.Now.Month+"/"+DateTime.Now.Year+" "+ x[i].Substring(x[i].IndexOf(' '), x[i].Length - x[i].IndexOf(' '));
            chtCategoriesProductCount.Series[0].Points.DataBindXY(x, y);
           // chtCategoriesProductCount.Series[0].ChartType = SeriesChartType.Column;
          //  chtCategoriesProductCount.ChartAreas["MainChartArea"].Area3DStyle.Enable3D = false;
         //   chtCategoriesProductCount.Legends[0].Enabled = true;

        }
        protected void Bt_Parameters_Click(object sender, EventArgs e)
        {
            MethodName = DropDownList1.Text;//    Service_Info.MethodName = DropDownList1.Text;
            Session["MethodName"] = MethodName;

            //----
            methodInfo = (MethodInfo[])Session["methodInfo"];
            //----

            param = methodInfo[DropDownList1.SelectedIndex].GetParameters(); //Service_Info.param = Service_Info.methodInfo[DropDownList1.SelectedIndex].GetParameters();
            Session["param"] = param;

            myProperty = new properties(param.Length);// Service_Info.myProperty = new properties(Service_Info.param.Length);

            // Get the Parameters Type
            paramTypes = new Type[param.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                paramTypes[i] = param[i].ParameterType;
            }
            Session["paramTypes"] = paramTypes;

            //  ListBox1.Items.Clear();
            DropDownList2.Items.Clear();
            TextBox2.Text = "";
            int ind = 0;
            foreach (ParameterInfo temp in param)
            {
                // ListBox1.Items.Add(temp.ParameterType.Name + "  " + temp.Name);
                DropDownList2.Items.Add(temp.ParameterType.Name + "  " + temp.Name);
                //ListBox2.Items.Add(temp.ParameterType.Name + "  " + temp.Name);
                myProperty.TypeParameter[ind++] = temp.ParameterType;
            }
            Session["myProperty"] = myProperty;
        }

        protected void bt_Param_Value_Click(object sender, EventArgs e)
        {
            try
            {
                ListBox1.Items.Add(DropDownList2.Text + "=" + TextBox2.Text);
                myProperty = (properties)Session["myProperty"];
                myProperty.MyValue[DropDownList2.SelectedIndex] = TextBox2.Text;
                Session["myProperty"] = myProperty;
                TextBox2.Text = "";
            }
            catch { }
        }

        protected void bt_Delete_Click(object sender, EventArgs e)
        {
            try { ListBox1.Items.RemoveAt(ListBox1.SelectedIndex); }
            catch { }
        }

        protected void bt_Invoke_Click(object sender, EventArgs e)
        {
            param = (ParameterInfo[])Session["param"];
            object[] param1 = new object[param.Length];
            myProperty = (properties)Session["myProperty"];

            for (int i = 0; i < param.Length; i++)
            {
                param1[i] = Convert.ChangeType(myProperty.MyValue[i], myProperty.TypeParameter[i]);
            }
            methodInfo = (MethodInfo[])Session["methodInfo"];
            MethodName = (string)Session["MethodName"];
            service = (Type)Session["service"];

            foreach (MethodInfo t in methodInfo)
                if (t.Name == MethodName)
                {
                    //Invoke Method
                    Object obj = Activator.CreateInstance(service);
                    Object response = t.Invoke(obj, param1);

                    TextBox3.Text = ("Result = " + response.ToString());

                    break;
                }
        }

    }
}