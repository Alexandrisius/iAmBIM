using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace AlignElements
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class StartPlugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;

            FilterWithPipe filterWithPipe = new FilterWithPipe();

            Element elem1 = null;
            Element elem2 = null;
            XYZ point_1 = null;
            XYZ point_2 = null;
            try
            {
                Reference select_1 = sel.PickObject(ObjectType.Element, filterWithPipe, "Выберите элемент который необходимо выровнять");
                elem1 = doc.GetElement(select_1);
                point_1 = select_1.GlobalPoint;
                if (elem1 == null)
                {
                    TaskDialog.Show("Ошибка", "Объект не найден");
                    return Result.Failed;
                }
            }
            catch (Exception)
            {
                TaskDialog.Show("Внимание", "Объект не найден");
                return Result.Failed;
            }
            if (elem1 != null)
            {
                try
                {
                    Reference select_2 = sel.PickObject(ObjectType.Element, filterWithPipe, "Выберите элемент который будет являться осью для выравнивания");
                    elem2 = doc.GetElement(select_2);
                    point_2 = select_2.GlobalPoint;
                    if (elem2 == null)
                    {
                        TaskDialog.Show("Ошибка", "Ось не найдена");
                        return Result.Failed;
                    }
                }
                catch (Exception)
                {
                    TaskDialog.Show("Внимание", "Ось не найдена");
                    return Result.Failed;
                }

            }
            if (elem1 != null && elem2 != null)
            {
                Connector con1 = ConnectorCalculator.GetConnectorByPoint(elem1, point_1);
                Connector con2 = ConnectorCalculator.GetConnectorByPoint(elem2, point_2);


                ICollection<ElementId> list = Iterator.GetElements(elem1, elem2);

                using (var tx = new Transaction(doc))
                {
                    tx.Start("AlignElements");
                    XYZ newPoint = con2.Origin - con1.Origin ;//находим вектор по двум точкам
                    ElementTransformUtils.MoveElements(doc, list, newPoint);
                    doc.Regenerate();

                    Line axis = ConnectorCalculator.GetNormalAxis(con1, con2);

                    if (axis != null)
                    {
                        XYZ vec1 = con1.CoordinateSystem.BasisZ;
                        XYZ vec2 = con2.CoordinateSystem.BasisZ;
                        double angle = vec1.AngleTo(vec2) + Math.PI;
                        ElementTransformUtils.RotateElements(doc, list, axis, angle);
                        doc.Regenerate();
                    }

                    XYZ point_corr = con2.CoordinateSystem.BasisZ;

                    ElementTransformUtils.MoveElements(doc, list, 0.25 * point_corr);

                    tx.Commit();

                    return Result.Succeeded;

                }
            }
            return Result.Failed;
        }
    }
}

