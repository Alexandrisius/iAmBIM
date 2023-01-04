using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeConnect
{
    public class TurnByClick
    {
        public static void TurnRight(Document doc, Connector con1, double angle)
        {
            Line lineZ = Line.CreateBound(con1.Origin, con1.Origin + con1.CoordinateSystem.BasisZ);

            if (angle != 0)
            {
                using (var tx = new Transaction(doc))
                {
                    tx.Start("TurnRight");

                    ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ, angle * Math.PI / 180);

                    tx.Commit();
                }
            }
        }
        public static void TurnLeft(Document doc, Connector con1, double angle)
        {
            Line lineZ = Line.CreateBound(con1.Origin, con1.Origin + con1.CoordinateSystem.BasisZ);

            if (angle != 0)
            {
                using (var tx = new Transaction(doc))
                {
                    tx.Start("TurnLeft");

                    ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ, -angle * Math.PI / 180);

                    tx.Commit();
                }
            }
        }

        public static void TurnAroundAxis(Document doc, Connector con1, Connector con2, out Connector usedConHere, List<Connector> usedConThere)
        {
            ICollection<Connector> freeCon = ElementConnectorIterator.GetFreeConnectors(con1);
            usedConHere = null;
            int count = freeCon.Count;
            
            if (usedConThere.Count == count)
            {
                usedConThere.Clear();
            }
            if (!usedConThere.Contains(con1))
            {
                usedConThere.Add(con1);
            }

            foreach (Connector mainCon in freeCon)
            {
                if (!usedConThere.Contains(mainCon) & mainCon.Origin.DistanceTo(con2.Origin)>0.1)
                {
                    usedConHere = mainCon;

                    using (var tx = new Transaction(doc))
                    {
                        tx.Start("ChangeConnect");

                        XYZ newPoint = con2.Origin - mainCon.Origin;//находим вектор по двум точкам!!!

                        ElementTransformUtils.MoveElement(doc, mainCon.Owner.Id, newPoint);

                        doc.Regenerate();

                        Line line = PlanePointCalculator.GetNormalAxis(mainCon, con2);// получаем ось вращения по двум коннекторам!!!

                        if (line != null)
                        {
                            XYZ vec1 = mainCon.CoordinateSystem.BasisZ;//!!!

                            XYZ vec2 = con2.CoordinateSystem.BasisZ;
                            double angle2 = vec1.AngleTo(vec2) + Math.PI;
                            ElementTransformUtils.RotateElement(doc, mainCon.Owner.Id, line, angle2);
                        }
                        doc.Regenerate();

                        if (mainCon.CoordinateSystem.BasisY.AngleTo(XYZ.BasisZ) != 0)
                        {
                            XYZ vec1 = mainCon.CoordinateSystem.BasisZ;//!!!
                            XYZ vec2 = mainCon.CoordinateSystem.BasisZ;

                            Line lineZ = Line.CreateBound(mainCon.Origin, mainCon.Origin + mainCon.CoordinateSystem.BasisZ);//!!!

                            double angle3 = mainCon.CoordinateSystem.BasisY.AngleTo(con2.CoordinateSystem.BasisY);// угол между векторами двух коннекторов!!!

                            if (angle3 != 0)
                            {
                                ElementTransformUtils.RotateElement(doc, mainCon.Owner.Id, lineZ, angle3);
                            }

                            double angle4 = mainCon.CoordinateSystem.BasisY.AngleTo(con2.CoordinateSystem.BasisY);

                            if (angle4 != 0)
                            {
                                ElementTransformUtils.RotateElement(doc, mainCon.Owner.Id, lineZ, -angle3 * 2);//если угол не стал 0 то поворот против часовой
                            }
                        }
                        tx.Commit();
                        break;
                    }
                }
            }
        }
    }
}
