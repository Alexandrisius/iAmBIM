#region Namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using UIFramework;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
#endregion

namespace iAmBIM
{
    class App : IExternalApplication
    {
        private const string _tab = "iAmBIM";

        private readonly string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public Result OnStartup(UIControlledApplication a)
        {
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_CDE = CDE_RibbonPanelAndTabPanel(a);
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_PS = PipeSys_RibbonPanel(a);
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_ARCH = ARCH_RibbonPanel(a);

            #region CreateButtonShareProject
            Image LargeImageConnect = Properties.Resources.ShareProject_32x32;
            ImageSource largeImageConnectSaSource = GetImageSource(LargeImageConnect);
            Image imageConnect = Properties.Resources.ShareProject_16x16;
            ImageSource imageConnectSource = GetImageSource(imageConnect);

            PushButtonData PBD_SP = new PushButtonData("Share Project", "Share\nProject", this.path + "\\ShareProject.dll", "ShareProject.StartPlugin")
            {
                ToolTip = "Размещение данного проекта в зоне Shared.",
                LongDescription = "Данный плагин очищает модель, проверяет на соответствие CDE-стандарту и размещает данный проект в зоне Shared. ",
                Image = imageConnectSource,
                LargeImage = largeImageConnectSaSource
            };

            PushButton button_Connect_1 = ribbonPanel_CDE.AddItem(PBD_SP) as PushButton;

            button_Connect_1.Enabled = true;

            #endregion

            #region CreateButtonScheduleByAssembly
            Image LargeImageScheduleByAssembly = Properties.Resources.ScheduleAssembly_32x32;
            ImageSource largeImageConSourceScheduleByAssembly = GetImageSource(LargeImageScheduleByAssembly);
            Image imageScheduleByAssembly = Properties.Resources.ScheduleAssembly_16x16;
            ImageSource imageScheduleByAssemblySource = GetImageSource(imageScheduleByAssembly);

            PushButtonData PBD_SFA = new PushButtonData("Schedule Assembly", "Schedule\nAssembly", this.path + "\\ScheduleByAssembly.dll", "ScheduleByAssembly.StartPlugin")
            {
                ToolTip = "Создание экспликаций по сборкам.",
                LongDescription = "Данный плагин создаёт экспликации по монтажным участкам ТХ. Участки системы должны быть объединены в сборки " +
                "и в именовании содержать слово 'Трубопровод', так же в проекте должен содержаться шаблон вида для спецификаций 'Экспликация_ТХ'" +
                " с зафиксированными полями и настроенными заголовками. ",
                Image = imageScheduleByAssemblySource,
                LargeImage = largeImageConSourceScheduleByAssembly
            };

            PushButton button_Connect_SFA = ribbonPanel_PS.AddItem(PBD_SFA) as PushButton;

            button_Connect_SFA.Enabled = true;

            #endregion

            #region CreateButtonFinishingSchedule
            Image LI_FinishingSchedule = Properties.Resources.FinishingSchedule_32x32;
            ImageSource LIS_FinishingSchedule = GetImageSource(LI_FinishingSchedule);
            Image I_FinishingSchedule = Properties.Resources.FinishingSchedule16x16;
            ImageSource IS_FinishingSchedule = GetImageSource(I_FinishingSchedule);

            PushButtonData PBD_FS = new PushButtonData("Finishing Schedule", "Finishing\nSchedule", this.path + "\\FinishingSchedule.dll", "FinishingSchedule.StartPlugin")
            {
                ToolTip = " ",
                LongDescription = " ",
                Image = IS_FinishingSchedule,
                LargeImage = LIS_FinishingSchedule
            };

            PushButton button_Connect_FS = ribbonPanel_ARCH.AddItem(PBD_FS) as PushButton;

            button_Connect_FS.Enabled = true;

            #endregion

            #region CreateButtonPipeConnect
            Image LargeImageConnect_PC = Properties.Resources.PipeCon_32х32;
            ImageSource largeImageConnectSaSource_PC = GetImageSource(LargeImageConnect_PC);
            Image imageConnect_PC = Properties.Resources.PipeCon_16х16;
            ImageSource imageConnectSource_PC = GetImageSource(imageConnect_PC);

            PushButtonData PBD_1 = new PushButtonData("Pipe Connect", "Pipe\nConnect", this.path + "\\PipeConnect.dll", "PipeConnect.StartPlugin")
            {
                ToolTip = "Присоединение трубопроводных элементов.",
                LongDescription = "Присоединяет коннекторы компонентов трубопроводных систем. Базовым элементом является компонент который пользователь выбрал в первую очередь. ",
                Image = imageConnectSource_PC,
                LargeImage = largeImageConnectSaSource_PC
            };

            PushButton button_Connect_PC = ribbonPanel_PS.AddItem(PBD_1) as PushButton;

            button_Connect_PC.Enabled = true;

            #endregion

            #region CreateButtonRotateElements
            Image LargeImageConnect_RE = Properties.Resources.RotatEl_32x32;
            ImageSource largeImageConnectSaSource_RE = GetImageSource(LargeImageConnect_RE);
            Image imageConnect_RE = Properties.Resources.RotatEl_16x16;
            ImageSource imageConnectSource_RE = GetImageSource(imageConnect_RE);

            PushButtonData PBD_RE = new PushButtonData("Rotate Elements", "Rotate\nElements", this.path + "\\RotateElements.dll", "RotateElements.StartPlugin")
            {
                ToolTip = "Вращает элементы вокруг оси выбранного коннектора.",
                LongDescription = "Вращает элементы вокруг оси выбранного коннектора со всеми присоединёнными к нему элементами. ",
                Image = imageConnectSource_RE,
                LargeImage = largeImageConnectSaSource_RE
            };

            PushButton button_Connect_RE = ribbonPanel_PS.AddItem(PBD_RE) as PushButton;
            button_Connect_RE.Enabled = true;
            #endregion

            #region CreateButtonAlignElements
            Image LargeImageConnect_AE = Properties.Resources.AlignEl_32x32;
            ImageSource largeImageConnectSaSource_AE = GetImageSource(LargeImageConnect_AE);
            Image imageConnect_AE = Properties.Resources.AlignEl_16x16;
            ImageSource imageConnectSource_AE = GetImageSource(imageConnect_AE);

            PushButtonData PBD_AE = new PushButtonData("Align Elements", "Align\nElements", this.path + "\\AlignElements.dll", "AlignElements.StartPlugin")
            {
                ToolTip = "Выравнивает элементы по оси выбранного коннектора.",
                LongDescription = "Выравнивает элементы по оси выбранного коннектора со всеми присоединёнными к нему элементами. ",
                Image = imageConnectSource_AE,
                LargeImage = largeImageConnectSaSource_AE
            };

            PushButton button_Connect_AE = ribbonPanel_PS.AddItem(PBD_AE) as PushButton;
            button_Connect_AE.Enabled = true;
            #endregion

            return Result.Succeeded;
        }

        private BitmapSource GetImageSource(Image img)
        {
            BitmapImage largeImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                largeImage.BeginInit();
                largeImage.CacheOption = BitmapCacheOption.OnLoad;
                largeImage.UriSource = null;
                largeImage.StreamSource = ms;
                largeImage.EndInit();
            }
            return largeImage;
        }

        public Autodesk.Revit.UI.RibbonPanel CDE_RibbonPanelAndTabPanel(UIControlledApplication a)
        {
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_PS = null;
            try
            {
                a.CreateRibbonTab(_tab);
            }
            catch { }
            try
            {
                Autodesk.Revit.UI.RibbonPanel panel = a.CreateRibbonPanel(_tab, "CDE");
            }
            catch { }
            List<Autodesk.Revit.UI.RibbonPanel> panels = a.GetRibbonPanels(_tab);
            foreach (var p in panels)
            {
                if (p.Name == "CDE")
                {
                    ribbonPanel_PS = p;
                }

            }
            return ribbonPanel_PS;
        }

        public Autodesk.Revit.UI.RibbonPanel PipeSys_RibbonPanel(UIControlledApplication a)
        {
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_PS = null;
           
            try
            {
                Autodesk.Revit.UI.RibbonPanel panel = a.CreateRibbonPanel(_tab, "Pipe System");
            }
            catch { }
            List<Autodesk.Revit.UI.RibbonPanel> panels = a.GetRibbonPanels(_tab);
            foreach (var p in panels)
            {
                if (p.Name == "Pipe System")
                {
                    ribbonPanel_PS = p;
                }

            }
            return ribbonPanel_PS;
        }
       
        public Autodesk.Revit.UI.RibbonPanel ARCH_RibbonPanel(UIControlledApplication a)
        {
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_PS = null;
           
            try
            {
                Autodesk.Revit.UI.RibbonPanel panel = a.CreateRibbonPanel(_tab, "Architecture");
            }
            catch { }
            List<Autodesk.Revit.UI.RibbonPanel> panels = a.GetRibbonPanels(_tab);
            foreach (var p in panels)
            {
                if (p.Name == "Architecture")
                {
                    ribbonPanel_PS = p;
                }

            }
            return ribbonPanel_PS;
        }

        public BitmapImage GetBitmapImage(string imagePath)
        {
            if (File.Exists(imagePath))
                return new BitmapImage(new Uri(imagePath));
            else
                return null;
        }


        void SetRibbonItemToolTip(RibbonItem item, RibbonToolTip toolTip)
        {
            var ribbonItem = GetRibbonItem(item);
            if (ribbonItem == null)
                return;
            ribbonItem.ToolTip = toolTip;
        }


        public Autodesk.Windows.RibbonItem GetRibbonItem(RibbonItem item)
        {
            RibbonControl ribbonControl = RevitRibbonControl.RibbonControl;

            foreach (var tab in ribbonControl.Tabs)
            {
                foreach (var panel in tab.Panels)
                {
                    foreach (var ribbonItem in panel.Source.Items)
                    {
                        if (ribbonItem.AutomationName == item.Name)
                            return ribbonItem as Autodesk.Windows.RibbonItem;
                    }
                }
            }

            return null;
        }


        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
