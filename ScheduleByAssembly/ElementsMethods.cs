using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ScheduleByAssembly
{
    internal class ElementsMethods
    {
        public static List<ElementId> GetAllSubelements(Document doc, Element elem)
        {
            List<ElementId> allElements = new List<ElementId>();

            if (!(elem is FamilyInstance familyInstance)) return null;
            List<ElementId> subElememts = familyInstance.GetSubComponentIds().ToList();
            if (!subElememts.Any()) return allElements;
            foreach (ElementId item in subElememts)
            {
                allElements.Add(item);
                List<ElementId> list = GetAllSubelements(doc, doc.GetElement(item));
                allElements.AddRange(list);
            }

            return allElements;

        }
       
    }
}
