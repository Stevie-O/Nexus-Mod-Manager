using System;
using Nexus.Client.Mods;

namespace Nexus.Client.ModManagement
{
	public interface IModLinkInstaller
	{
        /// <summary>
        /// Gets data needed to install the specified item.
        /// </summary>
        /// <param name="p_modMod"></param>
        /// <param name="p_strBaseFilePath"></param>
        /// <param name="p_strSourceFile"></param>
        /// <param name="p_booIsSwitching"></param>
        /// <param name="p_booHandlePlugin"></param>
        /// <returns>If there is any action to be taken, a <see cref="IModLinkItemInstallData"/> instance
        /// is returned that MUST be passed to <see cref="InstallItem"/> before the mod is fully activated.
        /// If there is no action to be taken, the return value is null.
        /// </returns>
        IModLinkItemInstallData GetInstallData(IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching, bool p_booHandlePlugin = false);

        /// <summary>
        /// Installs an item.
        /// </summary>
        /// <param name="itemData">An object returned by <see cref="GetInstallData"/>.</param>
        /// <exception cref="InvalidOperationException"><paramref name="itemData"/> conflicts with an existing mod and no
        /// conflict resolution has been set.</exception>
        string InstallItem(IModLinkItemInstallData itemData);

        #region Mod Link Installer
        /// <summary>
        /// (Deprecated) All-in-one, including user interaction for conflict resolution.
        /// </summary>
        /// <param name="p_modMod"></param>
        /// <param name="p_strBaseFilePath"></param>
        /// <param name="p_strSourceFile"></param>
        /// <param name="p_booIsSwitching"></param>
        /// <param name="p_booHandlePlugin"></param>
        /// <returns></returns>
        string AddFileLink(IMod p_modMod, string p_strBaseFilePath, string p_strSourceFile, bool p_booIsSwitching, bool p_booHandlePlugin = false);
		#endregion
	}
}
