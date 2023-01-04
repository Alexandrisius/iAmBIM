using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using PipeConnect;

namespace PipeConnect
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
            Connector conDetector = null;//временный коннектор чтобы нельзя было выбрать один и тотже элемент
            Connector con1 = PointOriginConnectors.GetConnector(sel, doc, conDetector);
            if (con1 == null)
            {
                
                TaskDialog.Show("Ошибка", "Первый объект не найден");
                return Result.Failed;
            }
            conDetector = con1;
            Connector con2 = PointOriginConnectors.GetConnector(sel, doc, conDetector);
            if (con2 == null)
            {
                TaskDialog.Show("Ошибка", "Второй объект не найден");
                return Result.Failed;
            }
            ICollection<ElementId> elementIds = ElementConnectorIterator.IteratorElements(con1.Owner, 10);

            int x;// идентификатор диаметра/радиуса
            Parameter param = null;
            if (CheckSizeConnector.GetParameterSizeConnector(doc, con1, out x) != null)
            {
                param = CheckSizeConnector.GetParameterSizeConnector(doc, con1, out x);
            }


            using (TransactionGroup transGroup = new TransactionGroup(doc))
            {
                transGroup.Start("Pipe Connect");

                using (var tx = new Transaction(doc))
                {
                    tx.Start("MoveElement");
                    if (x == 1)
                    {
                        param.Set(con2.Radius * 2); // изменение размера присоединяемого элемента
                    }
                    if (x == 0)
                    {
                        param.Set(con2.Radius); // изменение размера присоединяемого элемента
                    }
                    if (x == -1)
                    {
                        CheckSizeConnector.ChangeTypeBySizeConnector(doc, con1, con2);
                    }


                    doc.Regenerate();

                    XYZ newPoint = con2.Origin - con1.Origin;//находим вектор по двум точкам
                    ElementTransformUtils.MoveElements(doc, elementIds, newPoint);
                    tx.Commit();
                }

                Line line = PlanePointCalculator.GetNormalAxis(con1, con2);// получаем ось вращения по двум коннекторам


                using (var tx = new Transaction(doc))
                {
                    tx.Start("RotateElementXY");
                    if (line != null)
                    {
                        XYZ vec1 = con1.CoordinateSystem.BasisZ;
                        XYZ vec2 = con2.CoordinateSystem.BasisZ;
                        double angle2 = vec1.AngleTo(vec2) + Math.PI;
                        ElementTransformUtils.RotateElements(doc, elementIds, line, angle2);
                    }

                    
                    tx.Commit();
                }

                using (var tx = new Transaction(doc))
                {
                    tx.Start("RotateElementZ");
                    if (con1.CoordinateSystem.BasisY.AngleTo(XYZ.BasisZ) != 0)
                    {
                        //param.Set(con2.Radius);
                        XYZ vec1 = con1.CoordinateSystem.BasisZ;
                        XYZ vec2 = con2.CoordinateSystem.BasisZ;

                        Line lineZ = Line.CreateBound(con1.Origin, con1.Origin + con1.CoordinateSystem.BasisZ);

                        double angle3 = con1.CoordinateSystem.BasisY.AngleTo(con2.CoordinateSystem.BasisY);// угол между векторами двух коннекторов

                        if (angle3 != 0)
                        {
                            ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ, angle3);
                        }

                        double angle4 = con1.CoordinateSystem.BasisY.AngleTo(con2.CoordinateSystem.BasisY);

                        if (angle4 != 0)
                        {
                            ElementTransformUtils.RotateElement(doc, con1.Owner.Id, lineZ, -angle3 * 2);//если угол не стал 0 то поворот против часовой
                        }

                        
                    }
                    tx.Commit();
                }
                UserPipeAngleControl upc = new UserPipeAngleControl(doc, con1, con2, transGroup);

                upc.ShowDialog();

                if (transGroup.GetStatus() == TransactionStatus.Started)
                {
                    transGroup.Assimilate();
                }
                
                return Result.Succeeded;
            }

        }
    }
}

