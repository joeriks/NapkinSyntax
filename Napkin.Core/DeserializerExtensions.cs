using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Napkin
{
    public static class DeserializeExtensions
    {
        public static IEnumerable<T> Deserialize<T>(this string document)
        {
            var node = new Node(document);
            return node.As<T>();
        }
    }
}
