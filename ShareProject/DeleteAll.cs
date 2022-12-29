using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace ShareProject
{
    internal class DeleteAll
    {
        public static void DeleteForSharing(Document doc)
        {
            var ungroupAllGroup = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().Select(v => v.UngroupMembers()).ToList();
            var ungroupAllAssebly = new FilteredElementCollector(doc).OfClass(typeof(AssemblyInstance)).Cast<AssemblyInstance>().Select(v => v.Disassemble()).ToList();

            var allGroup = new FilteredElementCollector(doc).OfClass(typeof(GroupType)).Cast<GroupType>()
                .Select(v => v.Id).ToList();

            var allSpace = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MEPSpaces).Cast<Space>().Select(v => v.Id).ToList();

            var allViews = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>()
                .Where(v => !v.Name.Contains("TASK_") && !v.Name.Contains("Navisworks") && !v.Name.Contains("CORD_")).Select(v => v.Id).ToList();
           
            var allLinks = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).Select(v => v.Id).ToList();
            
            var allImage = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RasterImages).Select(v => v.Id).ToList();
            
            var allImport = new FilteredElementCollector(doc).OfClass(typeof(ImportInstance)).Cast<ImportInstance>().Select(v => v.Id).ToList();

            var allReinforc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_FabricReinforcement).Select(v => v.Id).ToList();
            var allRebar = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar).Select(v => v.Id).ToList();

            var superList = allViews.Concat(allLinks).Concat(allImport).Concat(allReinforc)
                .Concat(allRebar).Concat(allImage).Concat(allGroup).Concat(allSpace);
            
            foreach (var item in superList)
            {
                try
                {
                    doc.Delete(item);
                }
                catch (Exception)
                {
                    
                    continue;
                }

            }
        }
        internal static List<ElementId> GetPurgeableElements(Document doc, List<PerformanceAdviserRuleId> performanceAdviserRuleIds)
        {
            List<FailureMessage> failureMessages = PerformanceAdviser.GetPerformanceAdviser().ExecuteRules(doc, performanceAdviserRuleIds).ToList();
            if (failureMessages.Count > 0)
            {
                List<ElementId> purgeableElementIds = failureMessages[0].GetFailingElements().ToList();
                return purgeableElementIds;
            }
            return null;
        }

        public static void Purge(Document doc)
        {
            //The internal GUID of the Performance Adviser Rule 
            const string PurgeGuid = "e8c63650-70b7-435a-9010-ec97660c1bda";

            List<PerformanceAdviserRuleId> performanceAdviserRuleIds = new List<PerformanceAdviserRuleId>();

            //Iterating through all PerformanceAdviser rules looking to find that which matches PURGE_GUID
            foreach (var item in PerformanceAdviser.GetPerformanceAdviser().GetAllRuleIds())
            {
                string str = PerformanceAdviser.GetPerformanceAdviser().GetRuleName(item);

                if (item.Guid.ToString() == PurgeGuid)
                {
                    performanceAdviserRuleIds.Add(item);
                    break;
                }
            }

            //Attempting to recover all purgeable elements and delete them from the document
            List<ElementId> purgeableElementIds = GetPurgeableElements(doc, performanceAdviserRuleIds);
            if (purgeableElementIds != null)
            {
                doc.Delete(purgeableElementIds);
                List<ElementId> purgeableElementIds_2 = GetPurgeableElements(doc, performanceAdviserRuleIds);
                if (purgeableElementIds_2 != null)
                {
                    doc.Delete(purgeableElementIds_2);
                    List<ElementId> purgeableElementIds_3 = GetPurgeableElements(doc, performanceAdviserRuleIds);
                    if (purgeableElementIds_3 != null)
                    {
                        doc.Delete(purgeableElementIds_3);
                        List<ElementId> purgeableElementIds_4 = GetPurgeableElements(doc, performanceAdviserRuleIds);
                        if (purgeableElementIds_4 != null)
                        {
                            doc.Delete(purgeableElementIds_4);
                        }
                    }
                }
            }






        }
    }
}
