using Barotrauma;
using HarmonyLib;
using System.Runtime.CompilerServices;

// 'Barotrauma' makes client-side work.
[assembly: IgnoresAccessChecksTo("Barotrauma")]
// 'DedicatedServer' makes server-side work.
[assembly: IgnoresAccessChecksTo("DedicatedServer")]

namespace BarotraumaEasyOutposts;

public partial class Plugin : IAssemblyPlugin {

	public const string HarmonyId = "GrantShotwell.Barotrauma.EasyOutposts";

	private Harmony? _harmony = null;

	/// <summary>
	/// When your plugin is loading, use this instead of the constructor.
	/// Put any code here that does not rely on other plugins.
	/// </summary>
	public void Initialize() {
		this._harmony = new(HarmonyId);
		this._harmony.PatchAll();
		ModUtils.Logging.PrintMessage("Initialized Easy Outposts");
	}

	/// <summary>
	/// After all plugins have loaded.
	/// Put code that interacts with other plugins here.
	/// </summary>
	public void OnLoadCompleted() {
		ModUtils.Logging.PrintMessage("Loaded Easy Outposts");
	}

	/// <summary>
	/// Not yet supported: Called during the Barotrauma startup phase before vanilla content is loaded.
	/// </summary>
	public void PreInitPatching() {
	}

	public void Dispose() {
		this._harmony?.UnpatchAll();
	}

}
