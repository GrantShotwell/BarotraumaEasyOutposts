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
			__instance.FocusedCharacter = clicked;
			if (__instance.IsKeyHit(InputType.Left) && clicked.AllowCustomInteract) {
				clicked.onCustomInteract?.Invoke(clicked, __instance);
			}
		}
	}

}
