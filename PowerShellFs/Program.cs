using DokanNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace PowerShellFs
{
    public static class Program
    {
        private static class CollectionExtensions<T>
        {
            private static readonly MethodInfo GetItemsMethod = typeof(Collection<T>).GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetGetMethod(nonPublic: true);
            private static readonly ParameterExpression CollectionParam = Expression.Parameter(typeof(Collection<T>), "collection");
            public static Func<Collection<T>, IList<T>> GetItems { get; } = Expression.Lambda<Func<Collection<T>, IList<T>>>(Expression.Call(CollectionParam, GetItemsMethod), CollectionParam).Compile();
            
            private static readonly ParameterExpression ListParam = Expression.Parameter(typeof(List<T>), "list");
            private static readonly FieldInfo ListItemsField = typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            public static Func<List<T>, T[]> GetArray { get; } = Expression.Lambda<Func<List<T>, T[]>>(Expression.Field(ListParam, ListItemsField), ListParam).Compile();
        }

        public static T[] GetBuffer<T>(this Collection<T> collection)
        {
            if (CollectionExtensions<T>.GetItems(collection) is List<T> list)
            {
                return CollectionExtensions<T>.GetArray(list);
            }

            var array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }

        public static void Main(params string[] args)
        {
            RunspaceConnectionInfo ci = null;

            if (args?.FirstOrDefault() is string host)
            {
                ci = new WSManConnectionInfo
                {
                    ComputerName = args[0],
                    AuthenticationMechanism = AuthenticationMechanism.Default
                };
            }
            
            using var runspace = ci is not null ? RunspaceFactory.CreateRunspace(ci) : RunspaceFactory.CreateRunspace();

            var fs = new PowerShellFs(runspace);

            Console.CancelKeyPress += (sender, e) =>
            {
                Dokan.RemoveMountPoint("Q:");
                e.Cancel = true;
            };

            fs.Mount("Q:", DokanOptions.EnableFCBGC | DokanOptions.WriteProtection);
        }
    }
}
