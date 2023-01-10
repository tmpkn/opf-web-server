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
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace OPFService
{
    class NetworkService
    {
        OPFDictionary dict;
        public NetworkService(OPFDictionary d) => dict = d;
        public void main()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            var local = new IPEndPoint(ip, 5999);
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Helpers.Log("OPFService Starting", EventLogEntryType.Information, 30);

            try
            {
                listener.Bind(local);
                listener.Listen(64);
                while (true)
                {
                    Socket client = listener.Accept();
                    new Thread(() => handle(client)).Start();
                }
            }
            catch (Exception e)
            {
                Helpers.Log($"OPFService Start Exception: {e.InnerException}", EventLogEntryType.Error, 30);
            }
        }

        public void handle(Socket client)
        {
            try
            {
                var netStream = new NetworkStream(client);
                var istream = new StreamReader(netStream);
                var ostream = new StreamWriter(netStream);
                var command = istream.ReadLine();
                var pwd = istream.ReadLine();
                var usr = istream.ReadLine();


                switch (command)
                {
                    case "validate":
                        ostream.WriteLine(IsValid(usr, pwd));
                        ostream.Flush();
                        break;
                    case "update":
                        Update(usr);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Helpers.Log($"OPFService Handle Socket Exception: {e}", EventLogEntryType.Error, 30);
            }
            client.Close();
        }
        private string IsValid(string usr, string pwd)
        {
            var dictLookup = dict.contains(pwd);
            var pwned = PwnedClient.IsPwned(pwd);
            Helpers.Log($"IsValid results User: {usr} dictLookup: {dictLookup} pwned: {pwned}", EventLogEntryType.Information, 30);
            if (dictLookup || pwned)
                return "false";
            else
                return "true";
        }
        private void Update(string usr)
        {
            try
            {
                var phone = AD.GetUserAttributeValue(usr, "telephonenumber");
                if (string.IsNullOrWhiteSpace(phone))
                    Helpers.Log($"OPFService Telephone Number not found for Logon: {usr} , Vendor passwords not reset.", EventLogEntryType.Error, 30);
                else
                {
                    Helpers.Log($"OPFService found Telephone Number for Logon: {usr}", EventLogEntryType.Information, 30);
                    PwnedClient.UpdateExternalProviderPasswordsAsync(usr, phone);
                }
            }
            catch (Exception e)
            {
                Helpers.Log($"OPFService Update Exception: {e.InnerException}", EventLogEntryType.Error, 30);
            }
        }
    }
}
