using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Napkin
{
    class Program
    {
        static void Main(string[] args)
        {

            //var x = 



            //var lines = x.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            //var tb = lines.Select((line, row) => new { row, tab = line.Length - line.TrimStart(' ').Length, content = line.TrimStart(' ') });

            ////	tb.Dump();

            var n = new NapkinSyntaxTree(@"
    .foo

        #tab2

            .tab3

                %h1={title}

                tab4

        .tab2b

    .tab1b

        This is pretty useful {foo}");           

            var sw = new StringWriter();

            var model = new { title = "Hello World", foo="Foo!" };

            var renderers = new List<Action<TextWriter, NapkinSyntaxTree>>();
            renderers.Add(new Action<TextWriter, NapkinSyntaxTree>((tw, node) => {

                if (node.Attributes != null)
                {
                    var text = node.Attributes.FirstOrDefault();

                    text = Napkin.NapkinSyntaxTree.TemplateReplace(text, model);
                    node.Row = Napkin.NapkinSyntaxTree.TemplateReplace(node.Row, model);

                    if (text != null)
                    {

                        if (text.ToString().StartsWith("."))
                        {

                            var t1 = text.ToString().Replace(".", "div class='") + "'";
                            var t2 = "div";
                            tw.WriteLine(node.TrimStart + "<{0}>", t1);
                            foreach (var child in node.Children)
                            {
                                child.Renderers = renderers;
                                child.TextWriter = tw;
                                child.Render();
                            }
                            tw.WriteLine(node.TrimStart + "</{0}>", t2);

                        }
                        else if (text.ToString().StartsWith("#"))
                        {

                            var t1 = text.ToString().Replace("#", "div id='") + "'";
                            var t2 = "div";
                            tw.WriteLine(node.TrimStart + "<{0}>", t1);
                            foreach (var child in node.Children)
                            {
                                child.Renderers = renderers;
                                child.TextWriter = tw;
                                child.Render();
                            }
                            tw.WriteLine(node.TrimStart + "</{0}>", t2);

                        }
                        else if (text.ToString().StartsWith("%"))
                        {


                            var t1 = text.ToString().Substring(1).Split('=');
                            var tagName = t1[0];
                            var tagContent = t1.Count() > 0 ? t1[1] : "";

                            if (node.Children != null && node.Children.Any())
                            {
                                tw.WriteLine(node.TrimStart + "<{0}>", tagName);
                                tw.WriteLine(tagContent);
                                foreach (var child in node.Children)
                                {
                                    child.Renderers = renderers;
                                    child.TextWriter = tw;
                                    child.Render();
                                }
                                tw.WriteLine(node.TrimStart + "</{0}>", tagName);

                            }
                            else
                            {
                                tw.WriteLine(node.TrimStart + "<{0}>{1}</{0}>", tagName, tagContent);
                            }


                        }
                        else if (text.ToString().StartsWith("//"))
                        {
                        }
                        else
                        {
                            tw.WriteLine(node.Row);
                            foreach (var child in node.Children)
                            {
                                child.Renderers = renderers;
                                child.TextWriter = tw;
                                child.Render();
                            }

                        }
                    }
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        child.Renderers = renderers;
                        child.TextWriter = tw;
                        child.Render();
                    }
                }

            }));

            n.Render(renderers, sw);

            Console.Write(sw.ToString());
            Console.ReadLine();


        }
    }
}
