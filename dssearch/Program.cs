using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace dssearch
{
    public static class ActiveDirectoryInfo
    {
        static IEnumerable<string> EnumerateMessages(this Exception ex)
        {
            while (ex is not null)
            {
                yield return ex.Message;
                ex = ex.InnerException;
            }

            yield break;
        }

        public static int Main(string[] args)
        {
            try
            {
                return InternalMain(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(string.Join(" -> ", ex.EnumerateMessages().ToArray()));
                Console.ResetColor();
                return -1;
            }
        }

        public static int InternalMain(string[] args)
        {
            DirectoryEntry entry = null;

            // Examples to specify a domain
            //var adPath = "LDAP://DC=olofdom,DC=se";

            foreach (var arg in args)
            {
                if (arg.StartsWith("LDAP://", StringComparison.OrdinalIgnoreCase))
                {
                    if (entry is not null)
                    {
                        entry.Dispose();
                    }
                    entry = new DirectoryEntry(arg);
                }

                if (entry is null)
                {
                    entry = UserPrincipal.Current.GetUnderlyingObject() as DirectoryEntry;
                    if (entry is null)
                    {
                        Console.Error.WriteLine("No current LDAP user.");
                        return 1;
                    }

                    entry = entry.Parent;
                }

                // Examples to search for posix account values
                //var adSearch = new DirectorySearcher(new DirectoryEntry(adPath), "(&(objectClass=user)(sAMAccountName=olof))");
                //var adSearch = new DirectorySearcher(new DirectoryEntry(adPath), "(&(objectClass=user)(uidNumber=*)(msSFU30Password=*))");

                DirectorySearcher adSearch;

                if (string.IsNullOrEmpty(arg))
                {
                    adSearch = new DirectorySearcher(entry);
                }
                else
                {
                    adSearch = new DirectorySearcher(entry, arg);
                }

                using (adSearch)
                {
                    foreach (var userentry in adSearch.FindAll().OfType<SearchResult>().Select(u => u.GetDirectoryEntry()))
                    {
                        using (userentry)
                        {
                            Console.WriteLine();
                            Console.WriteLine("------------");

                            var props = userentry.Properties;

                            foreach (var prop in props.OfType<PropertyValueCollection>())
                            {
                                Console.WriteLine(FormatProp(prop));
                            }

                            // How to clear a field:
                            //userentry.Properties["mail"].Clear();
                            //userentry.CommitChanges();
                        }
                    }
                }
            }

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }

            return 0;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) => throw new NotImplementedException();

        public static void PrintProp(DirectoryEntry user)
        {
            if (user is not null)
            {
                var props = user.Properties;

                foreach (var prop in props.OfType<PropertyValueCollection>())
                {
                    Console.WriteLine(FormatProp(prop));
                }
            }
        }

        public static string FormatProp(PropertyValueCollection prop)
        {
            if (prop.Value is byte[] guidBytes &&
                guidBytes.Length == 16 &&
                prop.PropertyName.IndexOf("GUID", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return $"{prop.PropertyName} = {new Guid(guidBytes)}";
            }
            else if (prop.Value is byte[] sidBytes &&
                prop.PropertyName.IndexOf("Sid", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return $"{prop.PropertyName} = {new SecurityIdentifier(sidBytes, 0)}";
            }
            else
            {
                return $"{prop.PropertyName} = {FormatProp(prop.Value)}";
            }
        }

        private static string FormatProp(object propValue)
        {
            if (propValue is null)
            {
                return "(null)";
            }
            else if (propValue is string str)
            {
                return str;
            }
            else if (propValue is byte[] bytes)
            {
                if (bytes.Length > 32)
                {
                    return Convert.ToBase64String(bytes);
                }
                else
                {
                    return BitConverter.ToString(bytes);
                }
            }
            else if (propValue is object[] array)
            {
                return "{" +
                    Environment.NewLine +
                    string.Join(";" + Environment.NewLine,
                    Array.ConvertAll(array, o => $"  {FormatProp(o)}")) + Environment.NewLine +
                    "}";
            }
            else if (propValue.GetType() is IReflect reflect)
            {
                var type = reflect.UnderlyingSystemType;

                if (type.GUID.Equals(Guid.Empty))
                {
                    return "(unknown)";
                }

                var obj = Convert.ChangeType(propValue, type);

                return obj.ToString();
            }
            else
            {
                return propValue.ToString();
            }
        }
    }
}
