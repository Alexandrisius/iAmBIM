using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignElements
{
    internal class ConnectorCalculator
    {
        public static Connector GetConnectorByPoint(Element elem, XYZ point)
        {
            double length = double.MaxValue;
            Connector connector = null;

            if (elem is MEPCurve elemPipe)
            {
                ConnectorSet list = elemPipe.ConnectorManager.Connectors;
                foreach (Connector item in list)
                {
                    if (item.Origin.DistanceTo(point) < length)
                    {
                        length = item.Origin.DistanceTo(point);

                        connector = item;
                    }
                }

            }

            if (elem is FamilyInstance family)
            {
                ConnectorSet list = family.MEPModel.ConnectorManager.Connectors;
                foreach (Connector item in list)
                {

                    if (item.Origin.DistanceTo(point) < length)
                    {
                        length = item.Origin.DistanceTo(point);
                        connector = item;
                    }
                }
            }

            return connector;
        }

        public static Line GetNormalAxis(Connector connector1, Connector connector2)
        {
            List<XYZ> point = new List<XYZ>
                                 {
                                     connector1.Origin,
                                     connector1.Origin + connector1.CoordinateSystem.BasisZ,
                                     connector2.Origin + connector2.CoordinateSystem.BasisZ
                                 };
            if (point[1].IsAlmostEqualTo(point[2])) // Если два элемента имеют одинаковое направление векторов Z
            {
                point[2] = connector2.Origin + connector2.CoordinateSystem.BasisX; // вспомогательная точка, между X и Z всегда 90 град.
            }
            try
            {
                Plane plane = Plane.CreateByThreePoints(point[0], point[1], point[2]); // плоскость по трём точкам
                XYZ axis = connector2.Origin + plane.Normal; // Перемещение нормали в точку
                return Line.CreateBound(connector2.Origin, axis); // создание линии для оси вращения
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
