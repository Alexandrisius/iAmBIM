using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ShareProject
{
    internal class ModelChecker
    {
        static readonly string stringConnectionDB_NameConvention = Properties.Resources.ConnectionString;
        private static readonly SqlConnection DBconnection = new SqlConnection(stringConnectionDB_NameConvention);


        public static bool CheckNameModel(Document doc, out string error)
        {
            FileInfo fileName = new FileInfo(doc.PathName);
            string name = fileName.Name;
            string[] strCont = name.Split('-');
            bool IsDiscipline = false;

            DBconnection.Open();

            if (DBconnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    string query = $"SELECT Discipline FROM BIM_NC_Discipline";
                    SqlCommand command = new SqlCommand(query, DBconnection);

                    SqlDataReader reader = command.ExecuteReader();

                    reader.Read();

                    while (reader.Read())
                    {
                        if (reader[0].ToString() != strCont[5]) continue;
                        IsDiscipline = true;
                        break;
                    }
                    if (IsDiscipline == false)
                    {
                        error = "В корпоративной базе данных не найден информационный конейнер отвечающий за принадлежность дисциплины проекта." +
                            "Пожалуйста, проверьте корректность кода вашего раздела.";
                        return false;
                    }
                }
                catch
                {
                    DBconnection.Close();
                    error = "Нет доступа к базе данных компании!";
                    return false;
                }
            }
            if (DBconnection.State == System.Data.ConnectionState.Open) DBconnection.Close();

            bool result = strCont[0].Length == 4 &&
                   strCont[1] == "PPR" &&
                   strCont[2].Length == 3 &&
                   strCont[3].Length == 5 &&
                   strCont[4].Length == 3 &&
                   IsDiscipline &&
                   strCont[6] == "M3" &&
                   strCont[7].Contains("S0");

            if (!result)
            {
                error = "Именование модели не соответсвует CDE стандарту организации. " +
                    "Пожалуйста, исправьте и попробуйте снова.";
            }
            else { error = ""; }

            return result;
        }
    }
}
