using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;
using VnodeTest.General;

namespace VnodeTest
{
    public class RootController
    {
        private readonly Session Session;
        private Func<VNode> CurrentContent;

        public RootController(Session session)
        {
            Session = session;
        }

        //public VNode Render() => CurrentContent();
        public VNode Render() => GeneralController.Render();

        private GeneralController _GeneralController;
        private GeneralController GeneralController =>
        _GeneralController ??
        (_GeneralController = ((Application)Application.Instance).GeneralContext.CreateGeneralController());
    }

}
