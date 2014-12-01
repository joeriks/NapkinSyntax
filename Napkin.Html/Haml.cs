using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Napkin.Html
{
    public class Haml
    {
        public string Render(string napkinDocument)
        {
            var sb = new StringBuilder();

            Action<Node> writer = null;
            writer = new Action<Node>(n =>
            {
                if (new[] { "h1", "h2", "h3", "h4", "h5", "strong", "i", "b", "span", "p", "div" }.Contains(n.HeaderName))
                {
                    if (n.Children.Any())
                    {
                        sb.AppendFormat(n.Header.HeaderIndentation() + "<{0}>" + Environment.NewLine, n.HeaderName);
                        
                        foreach(var item in n.Children) writer(item);

                        sb.AppendFormat(n.Header.HeaderIndentation() + "</{0}>" + Environment.NewLine, n.HeaderName);
                    }
                    else
                    {
                        sb.AppendFormat(n.Header.HeaderIndentation() + "<{0}>{1}</{0}>" + Environment.NewLine, n.HeaderName, n.HeaderBody);
                    }
                }
                else
                {
                    sb.AppendLine(n.Header.Content);
                }
            });

            var doc = new Node(napkinDocument);
            foreach (var item in doc.Children) writer(item);

            return sb.ToString();

        }
    }
}
