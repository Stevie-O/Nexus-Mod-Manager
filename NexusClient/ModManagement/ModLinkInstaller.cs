using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nexus.Client.Mods;
using Nexus.Client.ModManagement.UI;
using Nexus.Client.Util;

namespace Nexus.Client.ModManagement
{
    public class ModLinkInstaller : IModLinkInstaller
    {
        private List<string> m_lstOverwriteFolders = new List<string>();
        private List<string> m_lstDontOverwriteFolders = new List<string>();
        private List<string> m_lstOverwriteMods = new List<string>();
        private List<string> m_lstDontOverwriteMods = new List<string>();
        private bool m_booDontOverwriteAll = false;
        private bool m_booOverwriteAll = false;
        private ConfirmItemOverwriteDelegate m_dlgOverwriteConfirmationDelegate = ((s, b, m) => OverwriteResult.YesToAll);

        #region Properties

        /// <summary>
        /// Gets or sets the mod being installed.
        /// </summary>
        /// <value>The mod being installed.</value>
        protected VirtualModActivator VirtualModActivator { get; set; }

        #endregion

        #region Constructors

        public ModLinkInstaller(IVirtualModActivator p_ivaVirtualModActivator)
        {
            VirtualModActivator = (VirtualModActivator)p_ivaVirtualModActivator;
        }

        #endregion

        class ModLinkItemInstallData : IModLinkItemInstallData
        {
            public ModInstallAction InstallAction { get; set; }

            public string BasePath { get; set; }

            public IMod ConflictingMod { get; set; }

            public bool ConflictResolutionNeeded => (InstallAction == ModInstallAction.AskUser);

            public void SetConflictResolution(ModConflictResolution resolution)
            {
                if (!ConflictResolutionNeeded) throw new InvalidOperationException("Conflict resolution action already determined for this item.");
                InstallAction = (resolution == ModConflictResolution.KeepExisting) ? ModInstallAction.InstallInactive : ModInstallAction.InstallActive;
            }

            public IMod Mod;
            public string SourceFile;
            public bool IsSwitching;
            public bool HandlePlugin;
            public List<IVirtualModLink> FileLinks;
            public int Priority;
        }

        public IModLinkItemInstallData GetInstallData(IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching, bool p_booHandlePlugin = false)
        {
            Int32 intPriority = 0;
            List<IVirtualModLink> lstFileLinks;
            ModInstallAction action;
            IMod conflictingMod;

            action = (GetInstallAction(p_modMod, p_strBaseFilePath, out conflictingMod, out intPriority, out lstFileLinks));
            if (action == ModInstallAction.DoNotInstall) return null;

            return new ModLinkItemInstallData()
            {
                Mod = p_modMod,
                BasePath = p_strBaseFilePath,
                SourceFile = p_strSourceFile,
                IsSwitching = p_booIsSwitching,
                HandlePlugin = p_booHandlePlugin,
                InstallAction = action,
                Priority = intPriority,
                FileLinks = lstFileLinks,
                ConflictingMod = conflictingMod,
            };
        }

        public string InstallItem(IModLinkItemInstallData itemData)
        {
            ModLinkItemInstallData data = (ModLinkItemInstallData)itemData;
            if (data.ConflictResolutionNeeded) throw new InvalidOperationException("Conflict resolution not specified on this item");

            Int32 intPriority = data.Priority;
            List<IVirtualModLink> lstFileLink = data.FileLinks;
            ModInstallAction action = data.InstallAction;
            IMod p_modMod = data.Mod;
            string p_strBaseFilePath = data.BasePath;
            string p_strSourceFile = data.SourceFile;
            bool p_booIsSwitching = data.IsSwitching;
            bool p_booHandlePlugin = data.HandlePlugin;

            if (action == ModInstallAction.InstallActive)
            {
                if ((data.Priority >= 0) && (lstFileLink != null) && (lstFileLink.Count > 0))
                {
                    VirtualModActivator.UpdateLinkListPriority(lstFileLink);
                    p_booIsSwitching = false;
                }
                return VirtualModActivator.AddFileLink(p_modMod, p_strBaseFilePath, p_strSourceFile, p_booIsSwitching, false, p_booHandlePlugin, 0);
            }
            else
                VirtualModActivator.AddInactiveLink(p_modMod, p_strBaseFilePath, ++intPriority);

            return string.Empty;
        }


        public string AddFileLink(IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching, bool p_booHandlePlugin)
        {
            var data = GetInstallData(p_modMod, p_strBaseFilePath, p_strSourceFile, p_booIsSwitching, p_booHandlePlugin);

            if (data == null) return string.Empty;

            if (data.ConflictResolutionNeeded)
            {
                // ask the user what to do, right this second
                string strLoweredPath = p_strBaseFilePath.ToLowerInvariant();
                var p_modConflictingMod = data.ConflictingMod;
                string strModFile = p_modConflictingMod.Filename;
                string strModFileID = p_modConflictingMod.Id;
                string strMessage = "";

                strMessage = String.Format("Data file '{{0}}' has already been installed by '{1}'", p_strBaseFilePath, p_modConflictingMod.ModName);
                strMessage += Environment.NewLine + "Activate this mod's file instead?";

                switch (OverwriteForm.ShowDialog(String.Format(strMessage, p_strBaseFilePath), true, (p_modConflictingMod != null)))
                {
                    case OverwriteResult.Yes:
                        data.SetConflictResolution(ModConflictResolution.ReplacingExisting);
                        break;
                    case OverwriteResult.No:
                        data.SetConflictResolution(ModConflictResolution.KeepExisting);
                        break;
                    case OverwriteResult.NoToAll:
                        m_booDontOverwriteAll = true;
                        data.SetConflictResolution(ModConflictResolution.KeepExisting);
                        break;
                    case OverwriteResult.YesToAll:
                        m_booOverwriteAll = true;
                        data.SetConflictResolution(ModConflictResolution.ReplacingExisting);
                        break;
                    case OverwriteResult.NoToGroup:
                        Queue<string> folders = new Queue<string>();
                        folders.Enqueue(Path.GetDirectoryName(strLoweredPath));
                        while (folders.Count > 0)
                        {
                            strLoweredPath = folders.Dequeue();
                            if (!m_lstOverwriteFolders.Contains(strLoweredPath))
                            {
                                m_lstDontOverwriteFolders.Add(strLoweredPath);
                                if (Directory.Exists(strLoweredPath))
                                    foreach (string s in Directory.GetDirectories(strLoweredPath))
                                    {
                                        folders.Enqueue(s.ToLowerInvariant());
                                    }
                            }
                        }
                        data.SetConflictResolution(ModConflictResolution.KeepExisting);
                        break;
                    case OverwriteResult.YesToGroup:
                        folders = new Queue<string>();
                        folders.Enqueue(Path.GetDirectoryName(strLoweredPath));
                        while (folders.Count > 0)
                        {
                            strLoweredPath = folders.Dequeue();
                            if (!m_lstDontOverwriteFolders.Contains(strLoweredPath))
                            {
                                m_lstOverwriteFolders.Add(strLoweredPath);
                                if (Directory.Exists(strLoweredPath))
                                    foreach (string s in Directory.GetDirectories(strLoweredPath))
                                    {
                                        folders.Enqueue(s.ToLowerInvariant());
                                    }
                            }
                        }
                        data.SetConflictResolution(ModConflictResolution.ReplacingExisting);
                        break;
                    case OverwriteResult.NoToMod:
                        strModFile = p_modConflictingMod.Filename;
                        strModFileID = p_modConflictingMod.Id;
                        if (!String.IsNullOrEmpty(strModFileID))
                        {
                            if (!m_lstOverwriteMods.Contains(strModFileID))
                                m_lstDontOverwriteMods.Add(strModFileID);
                        }
                        else
                        {
                            if (!m_lstOverwriteMods.Contains(strModFile))
                                m_lstDontOverwriteMods.Add(strModFile);
                        }
                        data.SetConflictResolution(ModConflictResolution.KeepExisting);
                        break;
                    case OverwriteResult.YesToMod:
                        strModFile = p_modConflictingMod.Filename;
                        strModFileID = p_modConflictingMod.Id;
                        if (!String.IsNullOrEmpty(strModFileID))
                        {
                            if (!m_lstDontOverwriteMods.Contains(strModFileID))
                                m_lstOverwriteMods.Add(strModFileID);
                        }
                        else
                        {
                            if (!m_lstDontOverwriteMods.Contains(strModFile))
                                m_lstOverwriteMods.Add(strModFile);
                        }
                        data.SetConflictResolution(ModConflictResolution.ReplacingExisting);
                        break;
                    default:
                        throw new Exception("Sanity check failed: OverwriteDialog returned a value not present in the OverwriteResult enum");
                }
            }

            return InstallItem(data);
        }

        private ModInstallAction GetInstallAction(IMod p_modMod, string p_strBaseFilePath, out IMod p_modConflictingMod, out Int32 p_intPriority, out List<IVirtualModLink> p_lstFileLinks)
        {
            Int32 intPriority = VirtualModActivator.CheckFileLink(p_strBaseFilePath, out p_modConflictingMod, out p_lstFileLinks);
            p_intPriority = intPriority;
            string strLoweredPath = p_strBaseFilePath.ToLowerInvariant();

            if (intPriority >= 0)
            {
                if (m_lstOverwriteFolders.Contains(Path.GetDirectoryName(strLoweredPath)))
                    return ModInstallAction.InstallActive;
                if (m_lstDontOverwriteFolders.Contains(Path.GetDirectoryName(strLoweredPath)))
                    return ModInstallAction.InstallInactive;
                if (m_booOverwriteAll)
                    return ModInstallAction.InstallActive;
                if (m_booDontOverwriteAll)
                    return ModInstallAction.InstallInactive;
            }

            if (p_modConflictingMod == p_modMod)
                return ModInstallAction.DoNotInstall;


            if (p_modConflictingMod == VirtualModActivator.DummyMod)
            {
                VirtualModActivator.OverwriteLooseFile(p_strBaseFilePath, Path.GetFileName(p_modMod.Filename));
                return ModInstallAction.InstallActive;
            }
            else if (p_modConflictingMod != null)
            {
                string strModFile = p_modConflictingMod.Filename;
                string strModFileID = p_modConflictingMod.Id;
                if (!String.IsNullOrEmpty(strModFileID))
                {
                    if (m_lstOverwriteMods.Contains(strModFileID))
                        return ModInstallAction.InstallActive;
                    if (m_lstDontOverwriteMods.Contains(strModFileID))
                        return ModInstallAction.InstallInactive;
                }
                else
                {
                    if (m_lstOverwriteMods.Contains(strModFile))
                        return ModInstallAction.InstallActive;
                    if (m_lstDontOverwriteMods.Contains(strModFile))
                        return ModInstallAction.InstallInactive;
                }
                return ModInstallAction.AskUser;
            }
            else
                return ModInstallAction.InstallActive;
        }
    }
}
