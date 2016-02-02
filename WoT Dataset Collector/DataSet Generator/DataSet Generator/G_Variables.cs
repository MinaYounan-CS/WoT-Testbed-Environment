using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSet_Generator
{
    class G_Variables
    {
       public static string Log_Path = "";
       public static string Log_Title = "";
       public static string[] Dev_types;
       public static int No_GWs; // #GW stored in DB
       public static int No_GWs_Availble = 0;               // #GW available                initiated at load_gateways function
       public static int[] Gateways_ID;                     // GWs IDs available            initiated at load_gateways function
       public static string[] Gateways_Title;               // set of GW's titles available initiated at load_gateways function
    }
}
