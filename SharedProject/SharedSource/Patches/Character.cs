using Barotrauma;
using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace BarotraumaAsphyxia.Patches;

[HarmonyPatch(typeof(Character), nameof(Character.DeselectCharacter))]
class Character_DelectCharacter {

	public static void Postfix(Character __instance) {
		if (
			Character_DoInteractionUpdate.TryGetOverrideFocusedCharacter(out var character, out _)
			&& character == __instance
		) {
			Character_DoInteractionUpdate.OverrideFocusedCharacter = null;
		}
	}

}

[HarmonyPatch(typeof(Character), nameof(Character.DoInteractionUpdate))]
class Character_DoInteractionUpdate {

	public static (Character character, Character clicked)? OverrideFocusedCharacter { get; set; } = null;

	public static bool TryGetOverrideFocusedCharacter(
		[NotNullWhen(true)] out Character? character,
		[NotNullWhen(true)] out Character? clicked
	) {
		var value = OverrideFocusedCharacter;
		if (value == null) {
			character = null;
			clicked = null;
			return false;
		} else {
			(character, clicked) = value.Value;
			return true;
		}
	}

	public static void Postfix(Character __instance) {
		if (
			TryGetOverrideFocusedCharacter(out var character, out var clicked)
			&& character == __instance
		) {
			// Undo unsetting focused character because they are not visible.
			__instance.FocusedCharacter = clicked;
			// I intended this to open the menu immediately on click,
			// but this acts as a double-click instead for some reason. I like that better!
			if (__instance.IsKeyHit(InputType.Select) && clicked.AllowCustomInteract) {
				clicked.onCustomInteract?.Invoke(clicked, __instance);
			}
		}
	}

}
