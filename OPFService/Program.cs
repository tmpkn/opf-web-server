// This file is part of OpenPasswordFilter.
// 
// OpenPasswordFilter is free software; you can redistribute it and / or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// OpenPasswordFilter is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OpenPasswordFilter; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111 - 1307  USA
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;

namespace OPFService
{
    class OPFService : ServiceBase
    {
        Thread worker;
        public OPFService()
        {
        }
        private void InitializeComponent() => this.ServiceName = "OPF";
        static void Main(string[] args)
        {
            OPFService service = new OPFService();
            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            if (!EventLog.SourceExists(constants.eventLogSource))
                EventLog.CreateEventSource(constants.eventLogSource, "Application");
            OPFDictionary d = new OPFDictionary(AppDomain.CurrentDomain.BaseDirectory + "\\opfmatch.txt", AppDomain.CurrentDomain.BaseDirectory + "opfcont.txt");
            NetworkService svc = new NetworkService(d);
            worker = new Thread(() => svc.main());
            worker.Start();
        }
        protected override void OnShutdown()
        {
            base.OnShutdown();
            worker.Abort();
        }
    }
}
