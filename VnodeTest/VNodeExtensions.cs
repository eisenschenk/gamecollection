using ACL.UI.React;
using System;

namespace VnodeTest
{
    static class VNodeExtensions
    {
        public static VNode WithOnclick(this VNode node, Action onclick)
        {
            node.OnClick = onclick;
            return node;
        }
    }
}
