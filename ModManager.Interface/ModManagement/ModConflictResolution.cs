using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Client.ModManagement
{
    /// <summary>
    /// Conflict resolution options for mod item installation.
    /// </summary>
    public enum ModConflictResolution
    {
        /// <summary>
        /// Replace the existing item with the version from the new mod.
        /// </summary>
        ReplacingExisting = 1,
        /// <summary>
        /// Keep the existing item.
        /// </summary>
        KeepExisting = 2,
    }
}
