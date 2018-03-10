using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexus.Client.ModManagement.UI
{
    /// <summary>
    /// TreeView implementation that fixes rapid check/uncheck
    /// </summary>
    class FixedTreeView : TreeView
    {
        // based on https://stackoverflow.com/questions/14647216/c-sharp-treeview-ignore-double-click-only-at-checkbox

        const int WM_LBUTTONDBLCLK = 0x0203;
        const int WM_LBUTTONDOWN = 0x0201;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                var ht = HitTest(new System.Drawing.Point((int)m.LParam));
                if (ht.Location == TreeViewHitTestLocations.StateImage)
                    m.Msg = WM_LBUTTONDOWN;
            }
            base.WndProc(ref m);
        }
    }
}
