using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace WinUser
{
    ///<summary>Critical systeminteractions</summary>
    public sealed class WindownsAccountInteract
    {
        ///<summary>Gets all local user-names</summary>
        public static String[] GetSystemUserList()
        {
            List<String> List = new();

            ManagementObjectSearcher searcher = new(new SelectQuery("Win32_UserAccount"));

            Regex regex = new("(\",Name=\")");
            List.AddRange(from ManagementObject envVar in searcher.Get()
                          let temp = regex.Split(envVar.ToString())
                          select temp[2].Remove(temp[2].Length - 1));
            return List.ToArray();
        }

        ///<summary>Gets local Administrator group name</summary>
        public static String GetAdminGroupName()
        {
            return new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).Value.Split('\\')[1];
        }

        ///<summary>Gets the current UAC user</summary>
        public static String GetUACUser()
        {
            return WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        }
    }
}