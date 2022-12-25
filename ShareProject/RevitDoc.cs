using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareProject
{
    internal class RevitDoc
    {
        public static void SyncWithoutRelinquishing(Document doc)
        {
            TransactWithCentralOptions transOpts = new TransactWithCentralOptions();
            SynchLockCallback transCallBack = new SynchLockCallback();
            transOpts.SetLockCallback(transCallBack);

            SynchronizeWithCentralOptions syncOpts = new SynchronizeWithCentralOptions();
            RelinquishOptions relinquishOpts = new RelinquishOptions(false);
            syncOpts.SetRelinquishOptions(relinquishOpts);
            syncOpts.SaveLocalAfter = false;
            syncOpts.Comment = "Revit API - ShareProject";

            try
            {
                doc.SynchronizeWithCentral(transOpts, syncOpts);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Synchronize Failed", e.Message);
            }
        }
        class SynchLockCallback : ICentralLockedCallback
        {
            // If unable to lock central, give up rather than waiting
            public bool ShouldWaitForLockAvailability()
            {
                return false;
            }

        }
    }
}
