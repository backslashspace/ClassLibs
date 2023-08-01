using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;

namespace RegistryTools
{
    ///<summary>Handles Registry access.</summary>
    public sealed class RegistryIO
    {
        ///<summary>Converts Human-Readable Registry path to <see langword="RegistryKey"/> type.</summary>
        ///<returns><see langword="null"/> when RegistryHive not found.</returns>
        private static RegistryKey PathToKey(String Path)
        {
            String DPath = Path.Split('\\')[0];
            Path = Regex.Match(Path, "(?<=\\\\).*").ToString();

            return DPath.ToUpper() switch
            {
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine.OpenSubKey(Path, true),
                "HKEY_CURRENT_USER" => Registry.CurrentUser.OpenSubKey(Path, true),
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot.OpenSubKey(Path, true),
                "HKEY_USERS" => Registry.Users.OpenSubKey(Path, true),
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig.OpenSubKey(Path, true),
                _ => throw new Exception("Invalid RegistryHive"),
            };
        }

        //######################################################

        ///<summary>Reads a value from the Registry.</summary>
        ///<returns>
        ///Returns data in form of specified <see langword="RegistryValueKind"/>.<br/>
        ///Returns <see langword="null"/> when value is not present.<br/>
        ///Returns '<c>-1</c>' when value has the wrong type and <see href="DeleteWrongType"/> is set to <see langword="false"/>.
        ///</returns>
        public static dynamic GetValue(String Path, String Value, RegistryValueKind ExpectedType, Boolean DeleteWrongType = false)
        {
            var Out = Registry.GetValue(Path, Value, null);

            if (Out == null)
            {
                return null;
            }

            switch (ExpectedType)
            {
                case RegistryValueKind.String:
                    if (Out is String)
                    {
                        return Out;
                    }
                    return Fallback();
                case RegistryValueKind.DWord:
                    if (Out is Int32)
                    {
                        return Out;
                    }

                    return Fallback();
                case RegistryValueKind.QWord:
                    if (Out is Int64)
                    {
                        return Out;
                    }

                    return Fallback(); ;
                case RegistryValueKind.MultiString:
                    if (Out is String[])
                    {
                        return Out;
                    }

                    return Fallback();
                case RegistryValueKind.Binary:
                    if (Out is Byte[] || Out is Int16[] || Out is Int32[] || Out is Int64[])
                    {
                        return Out;
                    }

                    return Fallback();
                default:
                    return null;
            }

            dynamic Fallback()
            {
                if (DeleteWrongType)
                {
                    RegistryKey Key = PathToKey(Path);

                    try
                    {
                        using (Key)
                        {
                            Key.DeleteValue(Value, false);
                        }
                        Key.Close();
                        Key.Dispose();
                    }
                    catch (System.ArgumentException) { }

                    return null;
                }

                return -1;
            }
        }

        ///<summary>Removes values from the Registry.</summary>
        ///<remarks>Takes a List of values and removes them in a specified path.</remarks>
        ///<returns><see langword="true"/> when at least one error occurred.</returns>
        public static Boolean DeleteValues(String Path, String[] Values, Boolean ContinueOnError = true)
        {
            RegistryKey Key = PathToKey(Path);

            if (Key == null) { return false; }

            Boolean RT = false;

            using (Key)

                foreach (String Value in Values)
                {
                    try
                    {

                        Key.DeleteValue(Value, true);

                    }
                    catch
                    {
                        if (ContinueOnError == false)
                        {
                            return true;
                        }

                        RT = true;
                    }
                }

            Key.Close();
            Key.Dispose();

            return RT;
        }

        ///<summary>Removes Key from the Registry.</summary>
        ///<remarks>Takes a List of Keys and removes them in a specified path.</remarks>
        ///<returns><see langword="true"/> when at least one error occurred.</returns>
        public static Boolean DeleteSubKeyTree(String KeyPath, String[] Keys, Boolean ContinueOnError = true)
        {
            RegistryKey MKey = PathToKey(KeyPath);

            if (MKey == null) { return false; }

            Boolean RT = false;

            using (MKey)

                foreach (String Key in Keys)
                {
                    try
                    {
                        MKey.DeleteSubKeyTree(Key);
                    }
                    catch
                    {
                        if (ContinueOnError == false)
                        {
                            return true;
                        }

                        RT = true;
                    }
                }

            MKey.Close();
            MKey.Dispose();

            return RT;
        }

        ///<summary>Tests if a value exists in the Registry.</summary>
        ///<returns><see langword="false"/> if not present.</returns>
        ///<returns> if not present.</returns>
        public static Boolean TestRegValuePresense(String Path, String Value)
        {
            try
            {
                if ((String)Registry.GetValue(Path, Value, null).ToString() == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex) when (ex is System.ArgumentNullException || ex is System.NullReferenceException)
            {
                return false;
            }
        }
    }
}