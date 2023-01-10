using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPFService
{
    public static class Helpers
    {
        public static void Log(string msg, EventLogEntryType type, int eventId)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = constants.eventLogSource;
                eventLog.WriteEntry(msg, type, 20);
            }
        }
    }
}
