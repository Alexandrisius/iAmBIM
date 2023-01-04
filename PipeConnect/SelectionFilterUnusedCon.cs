using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace PipeConnect
{
    public class SelectionFilterUnusedCon : ISelectionFilter
    {
        private readonly Connector con;

        public SelectionFilterUnusedCon(Connector _con)
        {
            con = _con;
        }

        public bool AllowElement(Element elem)
        {
            if (SelectionUnussedConnectors.GetUnussedConnectors(elem) & con == null)
            {
                return true;
            }

            else if (SelectionUnussedConnectors.GetUnussedConnectors(elem) & con?.Owner.Id != elem.Id)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

}
