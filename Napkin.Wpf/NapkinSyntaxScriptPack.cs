using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Napkin.Wpf
{
    public class NapkinSyntaxScriptPack : IScriptPack
    {
        private readonly NapkinPack _ctx;

        public NapkinSyntaxScriptPack(string textBoxText)
        {
            _ctx = new NapkinPack(textBoxText);
        }

        public void Initialize(IScriptPackSession session)
        {
            session.ImportNamespace("System");
            session.ImportNamespace("Napkin");
        }

        public IScriptPackContext GetContext()
        {
            return _ctx;
        }

        public void Terminate()
        {

        }
    }
}
