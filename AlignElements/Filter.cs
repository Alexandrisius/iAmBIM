using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignElements
{
    public class FilterWithPipe : ISelectionFilter
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


            if (elem is Pipe curve)
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
