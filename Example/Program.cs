using DO.VIVICARE.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<string>();
            FileInfo assemblyFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
            foreach (var file in Directory.GetFiles(assemblyFile.Directory.FullName + "\\Documents"))
            {
                FileInfo f = new FileInfo(file);

                if (f.Exists && f.Name.StartsWith("DO.VIVICARE"))
                {
                    var a = Assembly.LoadFile(f.FullName);
                    var obj = (from type in a.GetExportedTypes()
                               where type.Name.Equals("Report16")
                               select (BaseDocument)a.CreateInstance(type.FullName)).FirstOrDefault();
                    var ua = (DocumentReferenceAttribute)obj.GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                    if (ua != null)
                    {
                        list.Add($"{ua.Name}|{ua.FileName}|{ua.Description}");
                    }
                }
            }

            foreach(var n in list)
            {
                Console.WriteLine(n);
            }
            Console.Read();
        }
    }
}
