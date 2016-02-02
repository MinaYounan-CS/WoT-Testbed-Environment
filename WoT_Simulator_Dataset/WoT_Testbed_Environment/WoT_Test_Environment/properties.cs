using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace WoT_Test_Environment
{
    public class properties
    {
        public properties(int Capacity)
        {
            myValue = new string[Capacity];
            MyType = new Type[Capacity];
        }

        [System.ComponentModel.Category("Data"),
         System.ComponentModel.Description("The Type of parameter ")]
        public Type Type
        {
            get 
            { 
                return MyType[index]; 
            }

            set 
            { 
                MyType[index] = value;
            }
        }

        [System.ComponentModel.Category("Data"),
         System.ComponentModel.Description(" Enter value but whit attention Type parameter")]
        public string Value
        {
            get
            {
                return myValue[index];
            }
            set
            {
                myValue[index] = value;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int Index
        {
            set { index = value; }
            get { return index; }
        }

        [System.ComponentModel.Browsable(false)]
        public string[] MyValue
        {
            get { return myValue; }
        }

        [System.ComponentModel.Browsable(false)]
        public Type[] TypeParameter
        {
            get { return MyType; }
            set { MyType = value; }
        }



        private static string[] myValue;
        private static int index;
        private static Type[] MyType;
        
    }
}
