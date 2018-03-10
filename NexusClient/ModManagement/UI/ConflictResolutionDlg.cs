using Nexus.Client.Mods;
using Nexus.Client.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexus.Client.ModManagement.UI
{
    public partial class ConflictResolutionDlg : ManagedFontForm
    {
        List<IModLinkItemInstallData> _items;

        HashSet<IModLinkItemInstallData> GetCheckedItems()
        {
            HashSet<IModLinkItemInstallData> hs = new HashSet<IModLinkItemInstallData>();
            GetCheckedItems(hs, tvConflicts.Nodes);
            return hs;
        }

        void GetCheckedItems(HashSet<IModLinkItemInstallData> hs, TreeNodeCollection children)
        {
            foreach (TreeNode node in children)
            {
                if (node.Nodes.Count == 0)
                {
                    if (node.Checked && node.Tag is IModLinkItemInstallData)
                    {
                        hs.Add((IModLinkItemInstallData)node.Tag);
                    }
                }
                else
                {
                    GetCheckedItems(hs, node.Nodes);
                }
            }
        }

        public ConflictResolutionDlg(IMod newMod, List<IModLinkItemInstallData> conflictedItems)
        {
            InitializeComponent();

            label1.Text = $"The mod '{newMod.ModName}' conflicts with one or more existing mods.  To overwrite a file, put a check mark next to it; to keep the existing version, leave it unchecked.";
            _items = conflictedItems;
            RebuildTree();
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvConflicts.Nodes)
                node.Checked = true;
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvConflicts.Nodes)
                node.Checked = false;
        }

        /// <summary>
        /// Rebuilds the entire tree.
        /// </summary>
        void RebuildTree()
        {
            // remember which items were checked
            var checkedItems = GetCheckedItems();
            List<TreeNode> nodesToCheck = new List<TreeNode>();
            tvConflicts.Nodes.Clear();

            if (chkGroupByMod.Checked)
            {
                var mods = _items.GroupBy((_) => _.ConflictingMod.ModName ?? "(Unnamed Mod)");
                foreach (var mod in mods)
                {
                    var modNode = new TreeNode(mod.Key);
                    tvConflicts.Nodes.Add(modNode);
                    BuildTree(modNode.Nodes, mod, checkedItems, nodesToCheck);
                }
            }
            else
            {
                BuildTree(tvConflicts.Nodes, _items, checkedItems, nodesToCheck);
            }

            while (nodesToCheck.Count > 0)
            {
                HashSet<TreeNode> parentNodes = new HashSet<TreeNode>();
                foreach (TreeNode node in nodesToCheck)
                {
                    SetCheck(node, true);
                    if (node.Parent != null)
                        parentNodes.Add(node.Parent);
                }

                nodesToCheck.Clear();

                // if all of a node's children are checked, then check that node too
                foreach (TreeNode parent in parentNodes)
                {
                    if (AreAllChildrenChecked(parent))
                        nodesToCheck.Add(parent);
                }
            }
            tvConflicts.ExpandAll();
        }

        bool AreAllChildrenChecked(TreeNode parent)
        {
            return (parent.Nodes.Cast<TreeNode>().All((kid) => kid.Checked));
        }


        bool _programmaticCheck;

        void SetCheck(TreeNode node, bool checkState)
        {
            _programmaticCheck = true;
            node.Checked = checkState;
            _programmaticCheck = false;
        }

        TreeNodeCollection EnsureDirectoryNode(TreeNodeCollection rootNode, string path)
        {
            TreeNodeCollection parentNode;
            string dirName = Path.GetDirectoryName(path);
            if (dirName.Length > 0)
                parentNode = EnsureDirectoryNode(rootNode, dirName);
            else
                parentNode = rootNode;
            string nodeName = Path.GetFileName(path);
            string nodeKey = nodeName.ToLowerInvariant();
            TreeNode dirNode;
            if (parentNode.ContainsKey(nodeKey))
                dirNode = parentNode.Find(nodeKey, false).First();
            else
                dirNode = parentNode.Add(nodeKey, nodeName);
            return dirNode.Nodes;
        }

        /// <summary>
        /// Builds a subtree in the tree view, for the given set of items.
        /// </summary>
        /// <param name="rootNode">Root node for the items to be added</param>
        /// <param name="items">The items to be placed under <paramref name="rootNode"/></param>
        /// <param name="checkedItems">Items that need to be checked after they are added</param>
        /// <param name="nodesToCheck">List populated with the nodes corresponding to <paramref name="checkedItems"/></param>
        void BuildTree(TreeNodeCollection rootNode, IEnumerable<IModLinkItemInstallData> items, HashSet<IModLinkItemInstallData> checkedItems, List<TreeNode> nodesToCheck)
        {
            foreach (var item in items)
            {
                string itemDir = Path.GetDirectoryName(item.BasePath);
                string itemName = Path.GetFileName(item.BasePath);
                TreeNodeCollection parent;
                if (itemDir.Length == 0)
                    parent = rootNode;
                else
                    parent = EnsureDirectoryNode(rootNode, itemDir);
                var node = parent.Add(itemName);
                node.Tag = item;
                if (checkedItems.Contains(item))
                    nodesToCheck.Add(node);
            }
        }

        private void chkGroupByMod_CheckedChanged(object sender, EventArgs e)
        {
            RebuildTree();
        }

        private void tvConflicts_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_programmaticCheck) return; // ignore check notifications generated by ourselves
            System.Diagnostics.Debug.Print("After check");

            var node = e.Node;
            var chk = node.Checked;

            // check/uncheck all children
            // NOTE: we don't set _programmaticCheck to true; this will cause us recursivevly apply the operation
            foreach (TreeNode child in node.Nodes)
                child.Checked = chk;

            _programmaticCheck = true;
            TreeNode parent = node.Parent;
            if (chk)
            {
                // check all parents where all children are checked
                while (parent != null && AreAllChildrenChecked(parent))
                {
                    parent.Checked = true;
                    parent = parent.Parent;
                }
            }
            else
            {
                // uncheck all parents
                while (parent != null)
                {
                    parent.Checked = false;
                    parent = parent.Parent;
                }
            }
            _programmaticCheck = false;
        }

        /// <summary>
        /// Calls <see cref="IModLinkItemInstallData.SetConflictResolution"/> for all items based on the user's selections.
        /// </summary>
        public void ApplySelections()
        {
            ApplySelections(tvConflicts.Nodes);
        }

        void ApplySelections(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    // recurse to children
                    ApplySelections(node.Nodes);
                }
                else if (node.Tag is IModLinkItemInstallData)
                {
                    IModLinkItemInstallData item = (IModLinkItemInstallData)node.Tag;
                    item.SetConflictResolution(node.Checked ? ModConflictResolution.ReplacingExisting : ModConflictResolution.KeepExisting);
                }
            }
        }
    }
}
