using Barotrauma;
using HarmonyLib;

namespace BarotraumaEasyOutposts.Patches;

[HarmonyPatch(typeof(Character), nameof(Character.UpdateNetInput))]
class Character_UpdateNetInput {

	public static void Prefix(Character __instance) {
		if (__instance is AICharacter && !__instance.IsRemotePlayer) {
			return;
		}
		// Get network input.
		if (!__instance.CanMove || __instance.memInput.Count == 0) {
			return;
		}
		var input = __instance.memInput[^1];
		// Force clients' focus to NPCs with campaign interactions.
		var interact = Entity.FindEntityByID(input.interact);
		if (interact is not Character focused || focused.CampaignInteractionType == CampaignMode.InteractionType.None) {
			//ForcedFocusManager.SetForcedFocus(__instance, null);
			return;
		}
		// This combined with the Character.CanInteractWith(Character c, ...) patch does what we want.
		ForcedFocusManager.SetForcedFocus(__instance, focused);
	}

}
