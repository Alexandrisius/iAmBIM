using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Document = Autodesk.Revit.DB.Document;

namespace ShareProject
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

            RevitDoc.SyncWithoutRelinquishing(doc);

            FileInfo filePath = new FileInfo(doc.PathName);

            Document newDoc = app.NewProjectDocument(UnitSystem.Metric);

            ModelPath mp_temp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.DirectoryName + @"\" + "file_temp.rvt");

            newDoc.SaveAs(mp_temp, new SaveAsOptions{OverwriteExistingFile = true});

            FileInfo newFilePath = new FileInfo(newDoc.PathName);

            uiapp.OpenAndActivateDocument(newDoc.PathName);

            doc.Close(false);
            
            ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.FullName);

            OpenOptions openoptions = new OpenOptions();
            openoptions.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
            openoptions.Audit = true;
            openoptions.AllowOpeningLocalByWrongUser = true;
            uiapp.OpenAndActivateDocument(mp,openoptions,true);

            newDoc.Close(false);

            newFilePath.Delete();

           










            //SaveAsOptions options = new SaveAsOptions();
            //options.OverwriteExistingFile = true;
            //WorksharingSaveAsOptions save = new WorksharingSaveAsOptions();
            //save.SaveAsCentral = true;
            //ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(mpath + "\\" + fileName);
            //openedDoc.SaveAs(modelPathout, options);



            return Result.Succeeded;
        }
    }
}
