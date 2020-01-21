using ACL.CSS;
using System;
using System.Collections.Generic;
using System.Text;
using VnodeTest.General;

namespace VnodeTest
{
    public class Application : ACL.UI.Manager.Application
    {
        private object LockObject = new object();

        private GeneralContext _GeneralContext;
        public GeneralContext GeneralContext
        {
            get
            {
                // absicherung mit lock weil sonst bei gleichzeitigem mehrfachzugriff der kontext mehrfach versucht auf dieselbe datei zuzugreifen
                if (_GeneralContext == default)
                    lock (LockObject)
                        if (_GeneralContext == default)
                            _GeneralContext = new GeneralContext();
                return _GeneralContext;
            }
        }

        private ChessContext _ChessContext;
        public ChessContext ChessContext
        {
            get
            {
                // absicherung mit lock weil sonst bei gleichzeitigem mehrfachzugriff der kontext mehrfach versucht auf dieselbe datei zuzugreifen
                if (_ChessContext == default)
                    lock (LockObject)
                        if (_ChessContext == default)
                            _ChessContext = new ChessContext();
                return _ChessContext;
            }
        }

        private SolitaireContext _SolitaireContext;
        public SolitaireContext SolitaireContext
        {
            get
            {
                // absicherung mit lock weil sonst bei gleichzeitigem mehrfachzugriff der kontext mehrfach versucht auf dieselbe datei zuzugreifen
                if (_SolitaireContext == default)
                    lock (LockObject)
                        if (_SolitaireContext == default)
                            _SolitaireContext = new SolitaireContext();
                return _SolitaireContext;
            }
        }

        public Application(string[] args) : base(args) { }

        public override bool OnRun()
        {
            UseStyles = true;
            return true;
        }
        public override Rule[] GetCSSRules(string path) =>
    new Styles().GetRules();

        public override ACL.UI.Manager.ISimpleSession CreateSession(ACL.UI.Manager.SessionStartupInfo info)
        {
            var session = new Session(this, info);
            return session;
        }
    }

}
