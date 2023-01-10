﻿// This file is part of OpenPasswordFilter.
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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace OPFService
{
    class OPFDictionary
    {
        List<string> matchlist;
        List<string> contlist;

        public OPFDictionary(string pathmatch, string pathcont)
        {
            string line;
            StreamReader infilematch = new StreamReader(pathmatch);
            matchlist = new List<string>();
            int a = 1;
            while ((line = infilematch.ReadLine()) != null)
            {
                try
                {
                    matchlist.Add(line.ToLower());
                    a += 1;
                }
                catch
                {
                    Helpers.Log("Died trying to ingest line number " + a.ToString() + " of opfmatch.txt.", EventLogEntryType.Information, 20);
                }
            }
            infilematch.Close();
            StreamReader infilecont = new StreamReader(pathcont);
            contlist = new List<string>();
            a = 1;
            while ((line = infilecont.ReadLine()) != null)
            {
                try
                {
                    contlist.Add(line.ToLower());
                    a += 1;
                }
                catch
                {
                    Helpers.Log("Died trying to ingest line number " + a.ToString() + " of opfcont.txt.", EventLogEntryType.Information, 20);
                }
            }

        }


        public Boolean contains(string word)
        {
            foreach (string badstr in contlist)
                if (word.ToLower().Contains(badstr))
                {
                    Helpers.Log("Password attempt contains poison string " + badstr + ", case insensitive.", EventLogEntryType.Information, 20);
                    return true;
                }
            if (matchlist.Contains(word))
            {
                Helpers.Log("Password attempt matched a string in the bad password list", EventLogEntryType.Information, 20);
                return true;
            }
            else
            {
                Helpers.Log("Password passed custom filter.", EventLogEntryType.Information, 20);
                return false;
            }
        }
    }
}
