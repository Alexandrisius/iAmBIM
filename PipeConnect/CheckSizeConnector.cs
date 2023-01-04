using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections;

namespace PipeConnect
{
    public class CheckSizeConnector
    {
        public static Parameter GetParameterSizeConnector (Document doc, Connector con, out int x)
        {
            if (con.Owner is FamilyInstance)
            {
                x = 0;
                MEPFamilyConnectorInfo famConnInfo = con.GetMEPConnectorInfo() as MEPFamilyConnectorInfo;

                ElementId paramId = famConnInfo.GetAssociateFamilyParameterId(new ElementId(BuiltInParameter.CONNECTOR_RADIUS));

                if (paramId.IntegerValue == -1)// если у семейства нет радиуса, то применяем диаметр
                {
                    paramId = famConnInfo.GetAssociateFamilyParameterId(new ElementId(BuiltInParameter.CONNECTOR_DIAMETER));
                    x = 1;
                }

                Parameter param = con.Owner.LookupParameter(doc.GetElement(paramId).Name);

                if (param == null)
                {
                    x = -1;
                    return null;

                }

                return param;

            }
            else if (con.Owner is Pipe)
            {
                Parameter param = con.Owner.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                x = 1;
                return param;
            }
            else
            {
                x = 0;
                return null;
            }

        }

        public static void ChangeTypeBySizeConnector(Document doc, Connector con1, Connector con2)
        {
            int y = 0;
            FamilySymbol familySym = doc.GetElement(con1.Owner.GetTypeId()) as FamilySymbol;
            Family family = familySym.Family;
            ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();

            MEPFamilyConnectorInfo famConnInfo = con1.GetMEPConnectorInfo() as MEPFamilyConnectorInfo;

            ElementId paramId = famConnInfo.GetAssociateFamilyParameterId(new ElementId(BuiltInParameter.CONNECTOR_RADIUS));

            if (paramId.IntegerValue == -1)// если у семейства нет радиуса, то применяем диаметр
            {
                y = 1;
                paramId = famConnInfo.GetAssociateFamilyParameterId(new ElementId(BuiltInParameter.CONNECTOR_DIAMETER));
            }
            int x = 0;
            foreach (var item in familySymbolIds)
            {
                
                Parameter param = doc.GetElement(item).LookupParameter(doc.GetElement(paramId).Name);
                double d = param.AsDouble();

                if ((d == con2.Radius && y == 0) || (d == con2.Radius * 2 && y == 1))
                {
                    x = 1;
                    con1.Owner.ChangeTypeId(item);
                }
                
            }

            if (x == 0)
            {
                TaskDialog.Show("Внимание!", "Типоразмера не существует! Выбран последний типоразмер!");
            }
        }


    }
}
