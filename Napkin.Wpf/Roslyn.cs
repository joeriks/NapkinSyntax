using Roslyn.Scripting.CSharp;
using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Napkin.Wpf
{
    public class Roslyn
    {
        public static void Compile()
        {

            var se = new ScriptEngine().CreateSession();

            var x = se.Execute(
                "var x = 123;"
            );


        }
    }
}
