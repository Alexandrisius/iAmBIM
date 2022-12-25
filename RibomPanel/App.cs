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
            Autodesk.Revit.UI.RibbonPanel ribbonPanel_CDE = CDERibbonPanel(a);

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

        public Autodesk.Revit.UI.RibbonPanel CDERibbonPanel(UIControlledApplication a)
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
