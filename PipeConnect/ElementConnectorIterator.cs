using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace PipeConnect
{
    public class ElementConnectorIterator
    {
        /// <summary>
        /// Метод который возвращает id инженерной цепочки элементов.
        /// </summary>
        /// <param name="element"> Начальный элемент цепи </param>
        /// <param name="size"> Размер списка элементов</param>
        /// <returns> Возвращает список id элементов </returns>
        public static ICollection<ElementId> IteratorElements(Element element, int size)
        {
            ICollection<ElementId> listElements = new List<ElementId>();

            ConnectorSet connectorSet = GetConnectorSet(element);

            if (connectorSet != null)
            {
                ConnectorSetIterator csi = connectorSet.ForwardIterator();

                while (csi.MoveNext() && listElements.Count < size)
                {
                    if (csi.Current is Connector connector)
                    {
                        if (!listElements.Contains(connector.Owner.Id))
                        {
                            listElements.Add(connector.Owner.Id);
                        }
                        ConnectorSet conSet = connector.AllRefs;

                        foreach (Connector elemCon in conSet)
                        {
                            if (!listElements.Contains(elemCon.Owner.Id) && GetConnectorSet(elemCon.Owner) != null)
                            {
                                csi = GetConnectorSet(elemCon.Owner).ForwardIterator();
                            }
                        }
                    }
                }
            }

            return listElements;
        }

        public static ConnectorSet GetConnectorSet(Element element)
        {
            if (element is FamilyInstance family)
            {
                ConnectorSet connectorSet = family.MEPModel.ConnectorManager.Connectors;
                return connectorSet;
            }

            if (element is MEPCurve curve)
            {
                ConnectorSet connectorSet = curve.ConnectorManager.Connectors;
                return connectorSet;
            }

            return null;
        }

        public static ICollection<Connector> GetFreeConnectors(Connector connector)
        {
            ICollection<Connector>freeCon = new List<Connector>();

            ConnectorSet connectorSet = GetConnectorSet(connector.Owner);
            foreach (Connector con in connectorSet)
            {
                if (!con.IsConnected)
                {
                    freeCon.Add(con);
                }
            }

            return freeCon;
        }

    }
}
