using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Napkin;
using System.Text;

namespace Napkin.Core.NewSyntaxTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void HamlishSyntax()
        {
            var napkinDocument = @"

    h1 Some header
    p Then some text
    div
        p
            And some enclosed
            paragraphs.

";
            var sb = new StringBuilder();

            Action<Node> writer = null;
            writer = new Action<Node>(n =>
            {
                if (new[] { "h1", "p", "div" }.Contains(n.HeaderName))
                {
                    if (n.Children.Any())
                    {
                        sb.AppendFormat(n.Header.HeaderIndentation()+ "<{0}>" + Environment.NewLine, n.HeaderName);
                        n.Children.ForEach(writer);
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

            new Node(napkinDocument).Children.ForEach(writer);

            Assert.AreEqual(@"    <h1>Some header</h1>
    <p>Then some text</p>
    <div>
        <p>
            And some enclosed
            paragraphs.
        </p>
    </div>
", sb.ToString());


        }
    }
}
