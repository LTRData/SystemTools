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
    static class ActiveDirectoryInfo
    {
        static IEnumerable<string> GetMessages(this Exception ex)
        {
            while (ex != null)
            {
                yield return ex.Message;
                ex = ex.InnerException;
            }

            yield break;
        }

        static int Main(string[] args)
        {
            try
            {
                return InternalMain(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Join(" -> ", ex.GetMessages().ToArray()));
                return -1;
            }
        }

        static int InternalMain(string[] args)
        {
            DirectoryEntry entry = null;

            //var adPath = "LDAP://DC=olofdom,DC=se";

            foreach (var arg in args)
            {
                if (arg.StartsWith("LDAP://", StringComparison.OrdinalIgnoreCase))
                {
                    if (entry != null)
                    {
                        entry.Dispose();
                    }
                    entry = new DirectoryEntry(arg);
                }

                if (entry == null)
                {
                    entry = UserPrincipal.Current.GetUnderlyingObject() as DirectoryEntry;
                    if (entry == null)
                    {
                        Console.Error.WriteLine("No current LDAP user.");
                        return 1;
                    }

                    entry = entry.Parent;
                }

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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void PrintProp(DirectoryEntry user)
        {
            if (user != null)
            {
                var props = user.Properties;

                foreach (var prop in props.OfType<PropertyValueCollection>())
                {
                    Console.WriteLine(FormatProp(prop));
                }
                //userentry.Properties["mail"].Clear();
                //userentry.CommitChanges();
            }
        }

        public static string FormatProp(PropertyValueCollection prop)
        {
            if (prop.Value is byte[] &&
                (prop.Value as byte[]).Length == 16 &&
                prop.PropertyName.Contains("GUID"))
            {
                return prop.PropertyName + " = " + new Guid(prop.Value as byte[]).ToString();
            }
            else if (prop.Value is byte[] &&
                prop.PropertyName.Contains("Sid"))
            {
                return prop.PropertyName + " = " + new SecurityIdentifier(prop.Value as byte[], 0).ToString();
            }
            else
            {
                return prop.PropertyName + " = " + FormatProp(prop.Value);
            }
        }

        private static string FormatProp(object propValue)
        {
            if (propValue == null)
            {
                return "(null)";
            }
            else if (propValue is string)
            {
                return propValue as string;
            }
            else if (propValue is byte[])
            {
                return BitConverter.ToString(propValue as byte[]);
            }
            else if (propValue is Array)
            {
                return "{" +
                    Environment.NewLine +
                    string.Join(";" + Environment.NewLine,
                    Array.ConvertAll(propValue as object[], o => "  " + FormatProp(o))) + Environment.NewLine +
                    "}";
            }
            else if (propValue.GetType() is IReflect)
            {
                var reflect = propValue.GetType() as IReflect;
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
