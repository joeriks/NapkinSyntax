using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Napkin
{

    public static class SerializerExtensions
    {

        private static void nodeWriter(StringBuilder sb, int level, Node node)
        {


            sb.AppendLine(new string(' ', level) + node.HeaderName);

            foreach (var p in node.Properties)
            {
                sb.AppendLine(new string(' ', level + 1) + p.Key + "=" + p.Value);
            }

            foreach (var n in node.Children)
            {
                nodeWriter(sb, level + 1, n);
            }

        }

        public static string Serialize(this Node nodeTree)
        {

            var sb = new StringBuilder();
            nodeWriter(sb, 0, nodeTree);
            return sb.ToString();
        }
        //private static bool isValueType(object obj)
        //{
        //    return obj != null && obj.GetType().IsValueType;
        //}


        //private static void propertyWriter<T>(StringBuilder sb, int level, T obj)
        //{
        //    if (obj == null) return;

        //    var type = obj.GetType();

        //    if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type.GetGenericTypeDefinition() == typeof(List<>)))
        //    {
        //        Type underlyingType = type.GetGenericArguments()[0];
        //        //do something here
                

        //        foreach (var o in (IEnumerable<object>)obj)
        //        {
        //            propertyWriter(sb, level, o);
        //        }

        //    }
        //    else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
        //    {
        //        //foreach (var o in (Dictionary<object,object>)obj)
        //        //{
        //        //    propertyWriter(sb, level, o);
        //        //}
        //    }
        //    else
        //    {

        //        foreach (var prop in type.GetProperties())
        //        {
        //            var p = prop.GetValue(obj, null);
        //            if (p != null)
        //            {
        //                if (p.GetType().IsPrimitive || p is string)
        //                {
        //                    sb.AppendFormat("{0}={1}" + Environment.NewLine, prop.Name, p.ToString());
        //                }
        //                else
        //                {
        //                    propertyWriter(sb, level + 1, p);
        //                }
        //            }
        //        }
        //    }
        //}

        //public static string Serialize<T>(this T obj)
        //{
        //    var sb = new StringBuilder();
        //    propertyWriter(sb, 1, obj);
        //    return sb.ToString();
        //}

    }
}
