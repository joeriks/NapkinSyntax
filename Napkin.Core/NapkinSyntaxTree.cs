using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Napkin
{
    public class NapkinSyntaxTree
    {
        public List<string> Attributes;
        public List<NapkinSyntaxTree> Children;
        public string Row;

        public string TrimStart;
        public int NodeNumber;
        public int NodeLevel;


        public NapkinSyntaxTree()
        {
            Attributes = new List<string>();
            Children = new List<NapkinSyntaxTree>();
        }
        public static string TemplateReplace(string template, object parameters)
        {
            var parametersDictionary = parameters.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(parameters, null).ToString());
            return Regex.Replace(template, @"\{(.+?)\}", m => parametersDictionary[m.Groups[1].Value]);
        }
        public List<Action<TextWriter, NapkinSyntaxTree>> Renderers { get; set; }
        public TextWriter TextWriter { get; set; }
        public void Render(List<Action<TextWriter, NapkinSyntaxTree>> renderers, TextWriter textWriter)
        {

            Renderers = renderers;
            TextWriter = textWriter;

            foreach (var renderer in renderers)
            {
                renderer(textWriter, this);
            }

        }
        public void Render()
        {

            foreach (var renderer in Renderers)
            {
                renderer(TextWriter, this);
            }

        }

        public NapkinSyntaxTree(string fromString, string[] attributes = null)
        {
            if (attributes != null)
            {
                Attributes = attributes.ToList();
            }


            Children = new List<NapkinSyntaxTree>();
            if (!string.IsNullOrEmpty(fromString))
            {

                var lines = fromString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                var tb = lines.Select((line, row) =>
                   new {
                       row,
                       tab = line.Length - line.TrimStart().Length,
                       content = line,
                       isEmpty = string.IsNullOrEmpty(line)
                   });

                var findNodeLevel = tb.Where(t => !t.isEmpty).Min(t => t.tab);

                var content = "";
                var newNodeAttributes = default(string[]);

                var tablevel = 0;
                var rowCount = tb.Count();
                var trimStart = "";
                var rowContent = "";
                for (int i = 0; i < rowCount; i++)
                {

                    var row = tb.ToArray()[i];

                    if (row.tab == findNodeLevel && !row.isEmpty)
                    {

                        trimStart = row.content.Substring(0, row.content.Length - row.content.TrimStart().Length);
                        rowContent = row.content;
                        if (newNodeAttributes != null)
                        {
                            Children.Add(new NapkinSyntaxTree(content, newNodeAttributes) { TrimStart = trimStart, Row = rowContent });
                        }

                        tablevel = row.tab;

                        newNodeAttributes = row.content.Trim().Split(' ');
                        content = "";

                    }
                    else if (!row.isEmpty)
                    {
                        content += row.content + Environment.NewLine;
                    }
                }

                if (newNodeAttributes != null)
                {

                    Children.Add(new NapkinSyntaxTree(content, newNodeAttributes) { TrimStart = trimStart, Row = rowContent });
                }

            }
        }
    }
}
