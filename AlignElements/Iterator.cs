using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignElements
{
     class Iterator
    {
        public static ICollection<ElementId> GetElements(Element elem_ForAlign, Element elem_Axis)
        {
            ICollection<ElementId> listElements = new List<ElementId>();

            ConnectorSet connectorSet = GetConnectorSet(elem_ForAlign);

            if (connectorSet != null)
            {
                ConnectorSetIterator csi = connectorSet.ForwardIterator();

                while (csi.MoveNext() )
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
                            if (!listElements.Contains(elemCon.Owner.Id) && GetConnectorSet(elemCon.Owner) != null 
                                && elemCon.Owner.Id != elem_Axis.Id)
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


    }
}
