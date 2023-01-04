using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace PipeConnect
{
    public class ConnectElement
    {
        public static void ConnectTo(Connector con1, Connector con2, Document doc)
        {
            if (Math.Round(con1.Origin.X,3) == Math.Round(con2.Origin.X,3) &&
                Math.Round(con1.Origin.Y, 3) == Math.Round(con2.Origin.Y, 3)&&
                Math.Round(con1.Origin.Z, 3) == Math.Round(con2.Origin.Z, 3))
            {
                using (var tx = new Transaction(doc))
                {
                    tx.Start("Connect");

                    con1.ConnectTo(con2);

                    tx.Commit();
                }
            }
            else
            {
                TaskDialog.Show("Внимание!", "Коннекторы не находятся в одной точке!");
            }
        }
    }
}
