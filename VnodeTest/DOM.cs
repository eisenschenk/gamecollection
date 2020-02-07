using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class DOM
    {
        [Hidden]
        public static VNode Icon(string icon, ACL.UI.React.Style style = null, Style wrapperDivStyle = null)
        {
            string css = style?.CSSName ?? style?.Selector;
            // Ein extra DIV um "cannot find htmlelement for path ..1.1....5.. at index 7" zu vermeiden
            return Div(Styles.DInlineBlock & wrapperDivStyle, RawCode($"<i class=\"{icon} {css}\"></i>"));
        }
    }
}
