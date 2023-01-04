using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotateElements
{
    internal class Rotater
    {
        public static void TurnRight(Document doc, Line lineZ, double angle, Element elemForRotate,ICollection<ElementId> elemId,int type)
        {
           
            using (var tx = new Transaction(doc))
            {
                tx.Start("TurnRight");

                if (angle != 0 && type == 1)
                {
                    ElementTransformUtils.RotateElements(doc, elemId, lineZ, angle * Math.PI / 180);

                }
                if (angle != 0 && type == 2)
                {
                    ElementTransformUtils.RotateElement(doc, elemForRotate.Id, lineZ, angle * Math.PI / 180);

                }

                tx.Commit();
            }
            


        }
        public static void TurnLeft(Document doc, Line lineZ, double angle, Element elemForRotate, ICollection<ElementId> elemId,int type)
        {
            using (var tx = new Transaction(doc))
            {
                tx.Start("TurnLeft");

                if (angle != 0 && type == 1)
                {
                    ElementTransformUtils.RotateElements(doc, elemId, lineZ, -angle * Math.PI / 180);

                }
                if (angle != 0 && type == 2)
                {
                    ElementTransformUtils.RotateElement(doc, elemForRotate.Id, lineZ, -angle * Math.PI / 180);

                }

                tx.Commit();
            }


        }

    }
}

