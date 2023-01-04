using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace PipeConnect
{
    public class PointOriginConnectors
    {
        /// <summary>
        /// Метод который возвращает ближайший коннектор к месту клика мышкой.
        /// </summary>
        /// <param name="sel"></param>
        /// <param name="doc"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Connector GetConnector(Selection sel, Document doc, Connector connector)
        {
            double length = double.MaxValue;
            Reference reference = null;
            XYZ point = null;
            SelectionFilterUnusedCon filterUnusedCon = new SelectionFilterUnusedCon(connector);
            try
            {
                reference = sel.PickObject(ObjectType.Element, filterUnusedCon, "Выберите элемент для присоединения");
                point = reference.GlobalPoint;
            }
            catch
            {
            }

            if (reference != null)
            {
                using (Element elem = doc.GetElement(reference))
                {

                    if (elem is MEPCurve elemPipe)
                    {
                        ConnectorSet list = elemPipe.ConnectorManager.Connectors;
                        foreach (Connector item in list)
                        {
                            if (!item.IsConnected)
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

                    if (elem is FamilyInstance family)
                    {
                        ConnectorSet list = family.MEPModel.ConnectorManager.Connectors;
                        foreach (Connector item in list)
                        {
                            if (!item.IsConnected)
                            {
                                if (item.Origin.DistanceTo(point) < length)
                                {
                                    length = item.Origin.DistanceTo(point);
                                    connector = item;
                                }
                            }
                        }
                    }

                    return connector;
                }
            }
            return null;
        }
    }
}