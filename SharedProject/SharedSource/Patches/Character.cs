using Barotrauma;
using HarmonyLib;

namespace BarotraumaEasyOutposts.Patches;

[HarmonyPatch(typeof(Character), nameof(Character.CanInteractWith), typeof(Character), typeof(float), typeof(bool), typeof(bool))]
class Character_CanInteractWith {

	public static void Prefix(Character __instance, Character c, ref bool checkVisibility, ref bool skipDistanceCheck) {
		if (ForcedFocusManager.TryGetForcedFocus(__instance, out var focused) && focused == c) {
			checkVisibility = false;
			skipDistanceCheck = true;
		}
	}

}

[HarmonyPatch(typeof(Character), nameof(Character.DeselectCharacter))]
class Character_DelectCharacter {

	public static void Postfix(Character __instance) {
		// For some reason, this breaks focusing in multiplayer.
		// If it didn't, this line should be here.
		//ForcedFocusManager.SetForcedFocus(__instance, null);
	}

}

[HarmonyPatch(typeof(Character), nameof(Character.DoInteractionUpdate))]
class Character_DoInteractionUpdate {

	public static void Postfix(Character __instance) {
		if (ForcedFocusManager.TryGetForcedFocus(__instance, out var focused)) {
			// Undo unsetting focused character because they are not visible.
			__instance.FocusedCharacter = focused;
#if CLIENT
			// I intended this to open the menu immediately on click,
			// but this acts as a double-click instead for some reason. I like that better!
			if (__instance.IsKeyHit(InputType.Select) && focused.AllowCustomInteract) {
				focused.onCustomInteract?.Invoke(focused, __instance);
			}
#endif
		}
	}

}
