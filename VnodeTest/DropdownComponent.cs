using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;


namespace VnodeTest
{
    class DropdownComponent<T>
    {
        private bool IsSelected;


        public static VNode Render(IEnumerable<T> content, Action<T> selectEntry, string title, Func<T, VNode> render, int pageSize = 10)
        {
            return new ComponentNode<DropdownComponent<T>>(state =>
                Div(
                    RenderDropdownTitle(state, title),
                    Div(state.IsSelected ? RenderDropdownEntries(state, content, selectEntry, render, pageSize) : null))
            );
        }
        private static VNode RenderDropdownEntries(DropdownComponent<T> state, IEnumerable<T> content, Action<T> selectEntry, Func<T, VNode> render, int pageSize)
        {
            selectEntry += _ => state.IsSelected = false;
            return Div(
                Styles.Dropdown & Styles.MX2,
                PaginationComponent<T>.Render(content, s => render(s), selectEntry, pageSize)
            );
        }

        private static VNode RenderDropdownTitle(DropdownComponent<T> state, string title)
        {
            return Row(
                Styles.TabNameTagNoWidthMargin & Styles.FlexRow & Styles.W8C,
                Text(title, Styles.FlexStart),
                DOM.Icon(!state.IsSelected ? "fas fa-chevron-circle-right" : "fas fa-chevron-circle-down")
            ).WithOnclick(() => state.IsSelected = !state.IsSelected);
        }
    }
}

