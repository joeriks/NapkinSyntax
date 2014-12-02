using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Napkin
{
    public static class SerializerExtensions
    {

        private static void nodeWriter(StringBuilder sb, int level, Node node) {


            sb.AppendLine(new string(' ',level) + node.HeaderName);

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
    }
}
