using Autodesk.Revit.DB;

namespace RotateElements
{
    internal class ConnectorCalculator
    {
        public static Line GetAxisByTwoElements(Element elemForRotate, XYZ clickPoint, out Connector refConnector)
        {
            double length = double.MaxValue;
            refConnector = null;
            Connector connector = null;

            if (elemForRotate is FamilyInstance family)
            {
                ConnectorSet list = family.MEPModel.ConnectorManager.Connectors;
                foreach (Connector item in list)
                {

                    if (item.Origin.DistanceTo(clickPoint) < length)
                    {
                        length = item.Origin.DistanceTo(clickPoint);
                        connector = item;
                        ConnectorSet conSet = connector.AllRefs;
                        foreach (Connector refcon in conSet)
                        {
                            refConnector = refcon;
                        }
                    }
                }
            }

            return Line.CreateBound(connector?.Origin, connector?.Origin + connector?.CoordinateSystem.BasisZ);
        }
    }
}
