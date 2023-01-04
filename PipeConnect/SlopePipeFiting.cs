using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace PipeConnect
{
    class SlopePipeFiting
    {
        public static void SlopePipe(Document doc, Connector con1, int typeSlope, double slope)
        {
            FamilyInstance fi = con1.Owner as FamilyInstance;
            if (fi != null)
            {
                MechanicalFitting mf = fi.MEPModel as MechanicalFitting;

                if (mf != null && mf.PartType == PartType.Elbow)
                {
                    Line lineZ = Line.CreateBound(con1.Origin, con1.Origin + con1.CoordinateSystem.BasisZ);

                    double angle_desi = Math.PI - Math.Atan(1000 / slope); //проектный угол
                    double angle_corr = 2 * angle_desi - Math.PI; // угол корректировки

                    Connector con2 = null;
                    foreach (Connector con_item in con1.ConnectorManager.Connectors)
                    {
                        if (con_item.Id != con1.Id)
                        {
                            con2 = con_item;
                        }
                    }


                    if (slope != 0 && mf != null)
                    {
                        using (var tx = new Transaction(doc))
                        {
                            int count = 0; //страховка для выхода из бесконечного цикла
                            tx.Start("SlopeFiting");
                            if (typeSlope == 0)
                            {
                                double angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);

                                while ((Math.Round(angle_fact, 5) - Math.Round(angle_desi, 5)) != 0 && count < 10)
                                {
                                    ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ,
                                        -Math.Abs(angle_fact - angle_desi));
                                    angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);
                                    count++;
                                }

                                ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ,
                                    angle_corr); // подкрутка на нужный угол

                            }

                            if (typeSlope == 1)
                            {
                                double angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);

                                while ((Math.Round(angle_fact, 5) - Math.Round(angle_desi, 5)) != 0 && count < 10)
                                {
                                    ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ,
                                        Math.Abs(angle_fact - angle_desi));
                                    angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);
                                    count++;
                                }

                                ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ,
                                    -angle_corr); // подкрутка на нужный угол
                            }

                            if (typeSlope == 2)
                            {
                                double angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);

                                while ((Math.Round(angle_fact, 5) - Math.Round(angle_desi, 5)) != 0 && count < 10)
                                {
                                    ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ,
                                        Math.Abs(angle_fact - angle_desi));
                                    angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);
                                    count++;
                                }
                            }

                            if (typeSlope == 3)
                            {
                                double angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);

                                while ((Math.Round(angle_fact, 5) - Math.Round(angle_desi, 5)) != 0 && count < 10)
                                {
                                    ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ,
                                        -Math.Abs(angle_fact - angle_desi));
                                    angle_fact = con2.CoordinateSystem.BasisZ.AngleTo(XYZ.BasisZ);
                                    count++;
                                }
                            }

                            if (count >= 10)
                            {
                                TaskDialog.Show("Error", "Не удалось повернуть на заданный угол. Слишком много итераций. " +
                                                         "Возможно данная функция не для вашего случая.");
                                tx.RollBack();

                            }
                            else
                            {
                                TaskDialog.Show("Error", "Не выбран тип уклона");
                                tx.RollBack();
                            }


                            tx.Commit();
                        }


                    }
                }
                
            }
            else
            {
                TaskDialog.Show("Error", "Данный тип не является \"отводом\"");
            }
        }
    }
}
