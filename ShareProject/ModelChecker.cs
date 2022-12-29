using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ShareProject
{
    internal class ModelChecker
    {
        public static bool CheckNameModel(Document doc)
        {
            FileInfo fileName = new FileInfo(doc.PathName);
            string name = fileName.Name;
            string[] strCont =name.Split('-');
            return strCont[0].Length == 4 &&
                   strCont[1].Length == 3 &&
                   strCont[2].Length == 3 &&
                   strCont[3].Length == 5 &&
                   strCont[4].Length == 3 &&
                   strCont[5].Length == 4 &&
                   strCont[6] == "M3" &&
                   strCont[7].Contains("S0");
        }
    }
}
