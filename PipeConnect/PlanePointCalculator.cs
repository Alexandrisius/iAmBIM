using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace PipeConnect
{
    class PlanePointCalculator
    {
        /// <summary>
        /// Метод который возвращает перпендикулярную ось к плоскости перекрещивания осей Z у коннекторов.
        /// </summary>
        /// <param name="connector1"></param>
        /// <param name="connector2"></param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
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
                XYZ axis = connector1.Origin + plane.Normal; // Перемещение нормали в точку
                return Line.CreateBound(connector1.Origin, axis); // создание линии для оси вращения
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
