using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Napkin.Wpf
{
    public class NapkinPack : IScriptPackContext
    {
        public string Context { get; set; }
        public Napkin.Node Node()
        {
            return new Napkin.Node(Context);
        }
        public NapkinPack(string textBoxText)
        {
            Context = textBoxText;
        }
    }
}
