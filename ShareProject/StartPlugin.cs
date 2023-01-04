using System;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
            Document LocalDoc = uidoc.Document;

            if (ModelChecker.CheckNameModel(LocalDoc))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                UserControlShareProject ShareWindow = new UserControlShareProject();
                ShareWindow.Show();

                RevitDoc.SyncWithoutRelinquishing(LocalDoc); // синхронизируемся
                ShareWindow.addValueProgressBar(5);


                FileInfo fileLocalDoc = new FileInfo(LocalDoc.PathName); // получаем информацию о локальном файле

                ShareWindow.LabelShare.Content = "Создаём временный проект...";
                Document tempDoc = app.NewProjectDocument(UnitSystem.Metric); // создаём временный проект без шаблона
                ShareWindow.addValueProgressBar(10);

                ModelPath mp_temp = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileLocalDoc.DirectoryName + @"\" + "file_temp.rvt"); // создаём путь модели

                ShareWindow.LabelShare.Content = "Сохраняем временный проект...";
                tempDoc.SaveAs(mp_temp, new SaveAsOptions { OverwriteExistingFile = true }); // сохраняем временный проект по пути модели
                ShareWindow.addValueProgressBar(20);

                FileInfo tempFile = new FileInfo(tempDoc.PathName); // получаем информацию о временном файле

                ShareWindow.LabelShare.Content = "Активируем временный проект...";
                uiapp.OpenAndActivateDocument(tempDoc.PathName); // активируем вид, переключаемся на временный проект
                ShareWindow.addValueProgressBar(30);

                ModelPath MyCentralFile = LocalDoc.GetWorksharingCentralModelPath(); // получаем путь к файлу хранилища основного файла

                ShareWindow.LabelShare.Content = "Закрываем ваш локальный проект...";
                LocalDoc.Close(false); // закрываем наш локальный файл, мы можем это сделать только из-за того что переключили активный вид на временный проект
                ShareWindow.addValueProgressBar(35);

                ModelPath pathLocalFile = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileLocalDoc.FullName); // получаем путь модели локального файла

                OpenOptions openoptions = new OpenOptions(); // настройка опций открытия модели
                openoptions.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets; // опция отсоединения от ФХ с сохранением РН
                openoptions.Audit = true;//опция проведения аудита модели
                openoptions.AllowOpeningLocalByWrongUser = true; // опиция открытия файла в случае если его уже открыли 
                ShareWindow.LabelShare.Content = "Открываем ваш локальный проект с отсоединением от ФХ...";
                uiapp.OpenAndActivateDocument(MyCentralFile, openoptions, true);//заново открываем наш главный файл только уже с опциями
                ShareWindow.addValueProgressBar(45);

                Document detachSharedModel = uiapp.ActiveUIDocument.Document; // получаем документ с активного вида

                ShareWindow.LabelShare.Content = "Удаляем лишние элементы и виды...";
                using (Transaction tx = new Transaction(detachSharedModel)) // открываем транзакцию для удаления лишних элементов
                {
                    tx.Start("Delete elements");

                    FailureHandlingOptions failOpt = tx.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new FailuresClass());
                    tx.SetFailureHandlingOptions(failOpt);


                    DeleteAll.Purge(detachSharedModel);
                    DeleteAll.DeleteForSharing(detachSharedModel);
                    tx.Commit();
                }
                ShareWindow.addValueProgressBar(55);

                ShareWindow.LabelShare.Content = "Закрываем временный проект...";
                tempDoc.Close(false); // закрываем временный проект
                ShareWindow.addValueProgressBar(60);

                ShareWindow.LabelShare.Content = "Удаляем временный проект...";
                tempFile.Delete(); // удаляем временный проект
                ShareWindow.addValueProgressBar(65);

                SaveAsOptions options = new SaveAsOptions();
                options.OverwriteExistingFile = true;

                WorksharingSaveAsOptions save = new WorksharingSaveAsOptions();
                save.SaveAsCentral = true;
                options.SetWorksharingOptions(save);

                string modelPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(MyCentralFile);

                FileInfo openFile = new FileInfo(modelPath);

                if (openFile.DirectoryName != null)
                {
                    string stringNewPath = openFile.DirectoryName.Replace(@"\02_WIP\", @"\03_Shared\");

                    string newFileName = openFile.Name.Replace("-S0.rvt", "-S1.rvt");

                    ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(stringNewPath + @"\" + newFileName);

                    ShareWindow.LabelShare.Content = "Сохраняем модель в зону Shared...";
                    detachSharedModel.SaveAs(modelPathout, options);
                }

                ShareWindow.addValueProgressBar(75);

                OpenOptions newOpenoptions = new OpenOptions();
                newOpenoptions.DetachFromCentralOption = DetachFromCentralOption.DoNotDetach;
                newOpenoptions.Audit = false;
                newOpenoptions.AllowOpeningLocalByWrongUser = true;

                ShareWindow.LabelShare.Content = "Открываем ваш локальный файл...";
                uiapp.OpenAndActivateDocument(pathLocalFile, newOpenoptions, false);
                ShareWindow.addValueProgressBar(85);

                ShareWindow.LabelShare.Content = "Закрываем модель из зоны Shared...";
                detachSharedModel.Close(false);
                ShareWindow.addValueProgressBar(90);
                ShareWindow.LabelShare.Content = "Синхронизируем ваш локальный файл...";
                RevitDoc.SyncWithoutRelinquishing(uiapp.ActiveUIDocument.Document);
                ShareWindow.addValueProgressBar(100);

                ShareWindow.Close();
                stopwatch.Stop();
                double timer = Math.Round(stopwatch.ElapsedMilliseconds / 1000d, 0);

                string path = @"C:\ProgramData\Autodesk\Revit\Addins\2021\iAmBIM\MySaveTime.txt";
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                    StreamWriter write = File.CreateText(path);
                    write.Write(0);
                    write.Close();
                }
                StreamReader read = File.OpenText(path);
                double saveTime = Double.Parse(read.ReadToEnd());
                read.Close();
                double dif = Math.Round((300 - timer), 0);
                File.WriteAllText(path, (saveTime+dif).ToString());
                double saveTimeAll = Math.Round((saveTime + dif) / 3600,1);
                TaskDialog.Show("Share Project", "Проект успешно перемещён в зону Shared.\n" +
                    $"Затрачено времени: {timer} сек.\n" +
                    $"Сэкономлено часов за всё время: {saveTimeAll} час.", TaskDialogCommonButtons.Ok);

                return Result.Succeeded;
            }
            message = "Именование модели не соответсвует CDE стандарту организации. Пожалуйста, исправьте и попробуйте снова.";
            return Result.Failed;
        }
    }
}
