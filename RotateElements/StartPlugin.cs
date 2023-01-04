using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RotateElements
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class StartPlugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            if (uidoc == null)
            {
                TaskDialog.Show("No Active Document", "There's no active document in Revit.", TaskDialogCommonButtons.Ok);
                return Autodesk.Revit.UI.Result.Failed;
            }
            

            Filter filter = new Filter();

            Element elemForRotate;
            XYZ clickPoint;
            try
            {
                elemForRotate = doc.GetElement(sel.PickObject(ObjectType.Element, filter, "Выберите элемент который необходимо повернуть"));
            }
            catch (Exception)
            {
                //TaskDialog.Show("Внимание", "Объект для вращения не найден", TaskDialogCommonButtons.Ok);
                return Result.Failed;
            }

            try
            {
                Reference select_2 = sel.PickObject(ObjectType.PointOnElement,"Выберите ближайшую точку к оси вокруг которой необходимо совершать врщение");
                clickPoint = select_2.GlobalPoint;
                    
            }
            catch (Exception)
            {
                //TaskDialog.Show("Внимание", "Ось вращения не найдена", TaskDialogCommonButtons.Ok);
                return Result.Failed;
            }

            using (TransactionGroup transGroup = new TransactionGroup(doc))
            {
                transGroup.Start("RotateElements");

                UserRotateElementsControl wpf = new UserRotateElementsControl(doc, elemForRotate, clickPoint, transGroup);

                wpf.ShowDialog();

               return Result.Succeeded;
            }
        }
    }
}

