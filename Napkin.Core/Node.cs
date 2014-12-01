using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Napkin
{
    public enum Style
    {
        Indent,
        RowBreak,
        EmptyRowBefore
    }
    public class UnfoldSettings
    {
        public bool EachHeaderHasTypeName;
        public Style Style = Style.Indent;

    }
    public class Node
    {
        public UnfoldSettings Settings = new UnfoldSettings
        {            
            Style = Style.Indent
        };

        private void constructor(RowInformation header, string rawDocument, UnfoldSettings unfoldSettings = null)
        {
            this.header = header;
            this.rawDocument = rawDocument;

            if (unfoldSettings == null)
                unfoldSettings = new UnfoldSettings
                {
                    Style = Style.Indent
                };
            this.unfoldSettings = unfoldSettings;

            isUnfolded = false;
            children = new List<Node>();
            properties = new Dictionary<string, string>();

        }
        public Node(RowInformation header, string rawDocument, UnfoldSettings unfoldSettings = null)
        {
            constructor(header, rawDocument, unfoldSettings);
        }
        public Node(string rawDocument, UnfoldSettings unfoldSettings = null)
        {
            constructor(new RowInformation { Content = "" }, rawDocument, unfoldSettings);
        }

        private RowInformation header;
        private string rawDocument;

        public string RawDocument
        {
            get
            {
                return rawDocument;
            }
        }

        public RowInformation Header
        {
            get
            {
                return header;
            }
        }

        public string HeaderName
        {
            get
            {
                return Header.HeaderName();
            }
        }

        private List<Node> children;
        public List<Node> Children
        {
            get
            {
                if (!isUnfolded) Unfold();
                return children;
            }
        }

        private Dictionary<string, string> properties;
        public Dictionary<string, string> Properties
        {
            get
            {
                if (!isUnfolded) Unfold();
                return properties;
            }
        }
        public T GetPropertyValue<T>(string propertyName, T defaultValue = default(T))
        {
            if (Properties.ContainsKey(propertyName)) return (T)Convert.ChangeType(Properties[propertyName], typeof(T));
            return defaultValue;
        }
        public T CreateInstance<T>(Action<Node, T> recursiveSetters = null)
        {
            var instance = Activator.CreateInstance<T>();

            // if we've got unnamed attributes                
            foreach (var property in Properties)
            {
                var prop = instance.GetType().GetProperty(property.Key);
                if (prop != null)
                {
                    prop.SetValue(instance, Convert.ChangeType(property.Value, prop.PropertyType), null);
                }
            }

            if (recursiveSetters != null)
            {
                recursiveSetters(this, instance);
            }

            return instance;

        }

        public T CreateInstance<T>()
        {
            var instance = Activator.CreateInstance<T>();

            // if we've got unnamed attributes                
            foreach (var property in Properties)
            {
                var prop = instance.GetType().GetProperty(property.Key);
                if (prop != null)
                {
                    prop.SetValue(instance, Convert.ChangeType(property.Value, prop.PropertyType), null);
                }
            }

            var childProperty = instance.GetType().GetProperties().SingleOrDefault(t => t.PropertyType == typeof(IEnumerable<T>));
            if (childProperty != null)
            {
                childProperty.SetValue(instance, Children.Select(t => t.CreateInstance<T>()), null);
            }
            return instance;

        }

        public IEnumerable<T> As<T>()
        {
            return Children.Select(t=> t.CreateInstance<T>());
        }

        private bool isUnfolded = false;
        public bool IsUnfolded
        {
            get
            {
                return isUnfolded;
            }
        }
        private UnfoldSettings unfoldSettings;
        public void Unfold()
        {
            // unfold

            // find each header and add them as childNodes

            if (Settings.Style == Style.Indent) unfoldIndented();

            // then set flag
            isUnfolded = true;
        }

        private void unfoldIndented()
        {
            // unfold header

            foreach (var attribute in header.HeaderAttributes())
            {
                properties.Add(attribute.Key, attribute.Value);
            }

            // unfold document
            var rawDocumentLines = rawDocument.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select((content, rowNumber) => new RowInformation { Content = content, RowNumber = rowNumber }).ToList();

            var nonEmpty = rawDocumentLines.Where(t => !t.IsEmpty());
            if (!nonEmpty.Any()) return;

            var findLowestNodeLevel = nonEmpty.Min(t => t.Tab());

            var rowCount = rawDocumentLines.Count();

            var ast = default(Node);

            var isNewItemRow = new Func<RowInformation, RowInformation, bool>(

                (ri, pr) =>

                    ri.Tab() == findLowestNodeLevel &&
                    !ri.IsEmpty()

                );

            var isPropertyRow = new Func<Node, RowInformation, bool>(

                (nst, ri) =>

                    ri.Tab() == findLowestNodeLevel &&
                    ri.Content.Contains("=") &&
                    ri.Property().Key != null

                );

            var isContentRow = new Func<Node, RowInformation, bool>(

                (nst, ri) =>

                    nst != null &&
                    !ri.IsEmpty()

                );


            var previousRow = default(RowInformation);
            foreach (var row in rawDocumentLines)
            {


                // determine if node or property or nothing

                if (isPropertyRow(ast, row))
                {

                    var prop = row.Property();
                    properties.Add(prop.Key, prop.Value);

                }
                else if (isNewItemRow(row, previousRow))
                {

                    if (ast != null) children.Add(ast);
                    switch (unfoldSettings.Style)
                    {
                        case Style.Indent:
                            ast = new Node(row, "", unfoldSettings);
                            break;
                        case Style.EmptyRowBefore:
                            throw new NotImplementedException();
                            break;
                        case Style.RowBreak:
                            throw new NotImplementedException();
                            break;
                        default:
                            throw new NotImplementedException();
                            break;
                    }
                }
                else if (isContentRow(ast, row))
                {
                    ast.rawDocument += row.Content + Environment.NewLine;
                }
                previousRow = row;
            }

            if (ast != null) children.Add(ast);

        }


    }



}
