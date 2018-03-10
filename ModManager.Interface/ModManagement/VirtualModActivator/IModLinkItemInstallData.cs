using Nexus.Client.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Client.ModManagement
{
    /// <summary>
    /// Encapsulates the information needed by a <see cref="IModLinkInstaller"/> to install a single item.
    /// </summary>
    public interface IModLinkItemInstallData
    {
        /// <summary>
        /// Gets the base path for the item (the path relative to the game's data directory).
        /// </summary>
        string BasePath { get; }

        /// <summary>
        /// If there is a conflict, gets the mod that this mod conflicts with.
        /// </summary>
        IMod ConflictingMod { get; }

        /// <summary>
        /// Gets whether or not to ask the user to resolve the conflict.
        /// </summary>
        bool ConflictResolutionNeeded { get; }

        /// <summary>
        /// Sets the conflict resolution on the item.
        /// </summary>
        /// <param name="resolution">Whether to keep or replace the existing item.</param>
        /// <exception cref="InvalidOperationException">The item's conflict resolution has already been set.</exception>
        void SetConflictResolution(ModConflictResolution resolution);
    }
}
