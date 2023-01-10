//
// opf-web additions / tompaw@tompaw.net / 2023-01-01
// https://tompaw.net/opf-web/
//

using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace OPFService
{
    public static class AD
    {
        /// <summary>
        /// Ex: testa, telephonenumber : Returns the users Telephone Number.
        /// </summary>
        /// <param name="logon"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetUserAttributeValue(string logon, string attribute)
        {
            var ctx = new PrincipalContext(ContextType.Domain);
            UserPrincipal q = new UserPrincipal(ctx)
            {
                SamAccountName = logon
            };
            PrincipalSearcher s = new PrincipalSearcher(q);
            DirectorySearcher ds = (DirectorySearcher)s.GetUnderlyingSearcher();
            ds.PropertiesToLoad.Clear();
            ds.PropertiesToLoad.Add(attribute);

            foreach (SearchResult dsResult in ds.FindAll())
                return (String)dsResult.Properties[attribute][0];

            return "";
        }
    }
}
