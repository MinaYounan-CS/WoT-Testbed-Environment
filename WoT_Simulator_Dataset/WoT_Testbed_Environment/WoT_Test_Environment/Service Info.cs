using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Services.Description;
using System.CodeDom;
using System.Xml.Serialization;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;

namespace WoT_Test_Environment
{
    public class Service_Info
    {
        public static Type service;
        public static MethodInfo[] methodInfo;
        public static Uri uri;
        public static string[] listurl;

        //  private MethodInfo[] methodInfo;
        public static ParameterInfo[] param;
        //  private Type service;
        public static Type[] paramTypes;
    //    public static properties myProperty;
        public static string MethodName = "";
        public static properties myProperty;

        public static void service_info_methods()
        {
            uri = new Uri("http://localhost:60377/Service1.asmx?wsdl"); 
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

                methodInfo = service.GetMethods();

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

            }

        }
    }
}