using Nexus.Client.ModManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Client
{
    /// <summary>
    /// Classifies the action that should be taken when a conflict is discovered while installing a mod.
    /// </summary>
    public enum ModInstallAction
    {
        /// <summary>
        /// No conflict detected.
        /// </summary>
        DoNotInstall = 0,
        /// <summary>
        /// Replace the existing item with the one from the mod.
        /// </summary>
        InstallActive = ModConflictResolution.ReplacingExisting,
        /// <summary>
        /// Keep the existing item.
        /// </summary>
        InstallInactive = ModConflictResolution.KeepExisting,
        /// <summary>
        /// Ask the user what they want us to do.
        /// </summary>
        AskUser,
    }
}
