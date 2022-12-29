using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ShareProject
{
    internal class FailuresClass : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failList = new List<FailureMessageAccessor>();
            failList = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor failure in failList)
            {
                failuresAccessor.DeleteWarning(failure);
            }
            return FailureProcessingResult.Continue;
        }
    }
}
