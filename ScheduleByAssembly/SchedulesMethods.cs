using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleByAssembly
{
    class SchedulesMethods
    {
        public static void AddRegularFieldToSchedule(ViewSchedule schedule, string guid)
        {
            ScheduleDefinition definition = schedule.Definition;

            SchedulableField schedulableField = null;
            foreach (var x in schedule.Definition.GetSchedulableFields())
            {
                if (IsSharedParameterSchedulableField(schedule.Document, x.ParameterId, new Guid(guid)))
                {
                    schedulableField = x;
                    break;
                }
            }

            if (schedulableField != null)
            {
                definition.AddField(schedulableField);
            }
        }
        private static bool IsSharedParameterSchedulableField(Document document, ElementId parameterId, Guid sharedParameterId)
        {
            SharedParameterElement sharedParameterElement = document.GetElement(parameterId) as SharedParameterElement;

            return sharedParameterElement?.GuidValue == sharedParameterId;
        }
        public static ScheduleField FindField(ViewSchedule schedule, string guid)
        {
            ScheduleDefinition definition = schedule.Definition;

            SharedParameterElement sharParam = SharedParameterElement.Lookup(schedule.Document, Guid.Parse(guid));
            ElementId paramId = sharParam.Id;

            foreach (ScheduleFieldId fieldId in definition.GetFieldOrder())
            {
                ScheduleField foundField = definition.GetField(fieldId);
                if (foundField.ParameterId == paramId)
                {
                    return foundField;
                }
            }

            return null;
        }

    }
}
