using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;

namespace RotateElements
{
    public class Filter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance family)
            {
                ConnectorManager list = family.MEPModel.ConnectorManager;

                foreach (Connector item in list.Connectors)
                {
                    if (item.Domain == Domain.DomainPiping)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    

}
