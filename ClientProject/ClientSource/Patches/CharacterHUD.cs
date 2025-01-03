using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace BarotraumaAsphyxia.Patches;

[HarmonyPatch(typeof(CharacterHUD), nameof(CharacterHUD.Update))]
class CharacterHUD_Update {

	public static bool CanFocus(Character character) {
		return !character.DisableFocusingOnEntities
			&& !character.IsIncapacitated
			&& character.Stun <= 0f
			&& !Character.IsMouseOnUI;
	}

	public static void Postfix(float deltaTime, Character character, Camera cam) {
		try {
			if (CanFocus(character) && PlayerInput.PrimaryMouseButtonClicked()) {
				// Find out if mouse is hovering over campaign interaction icon.
				// See local function: CharacterHUD.Draw(SpriteBatch, Character, Camera) -> DrawInteractionIcon(Entity, Identifier)
				bool CursorOverInteractionIcon(Entity entity, Identifier style) {
					if (GUIStyle.GetComponentStyle(style) is not GUIComponentStyle componentStyle) {
						return false;
					}
					Sprite sprite = componentStyle.GetDefaultSprite();
					Vector2 worldPosition = entity.DrawPosition;
					// Find the position of the icon on the screen.
					Vector2 diff = worldPosition - cam.WorldViewCenter;
					float symbolScale = Math.Min(64.0f / sprite.size.X, 1.0f) * GUI.Scale;
					float angle = MathUtils.VectorToAngle(diff);
					Vector2 targetScreenPos = cam.WorldToScreen(worldPosition);
					float screenDist = Vector2.Distance(cam.WorldToScreen(cam.WorldViewCenter), targetScreenPos);
					Vector2 iconDiff = new Vector2(
						(float)Math.Cos(angle) * Math.Min(GameMain.GraphicsWidth * 0.4f, screenDist + 10),
						(float)-Math.Sin(angle) * Math.Min(GameMain.GraphicsHeight * 0.4f, screenDist + 10));
					Vector2 iconPosition = cam.WorldToScreen(cam.WorldViewCenter) + iconDiff;
					// Find if the cursor is over the icon.
					float distance = Vector2.Distance(PlayerInput.MousePosition, iconPosition);
					return distance < sprite.size.X * symbolScale;
				}
				bool focusedSomething = false;
				foreach (var npc in Character.CharacterList) {
					var interactionType = npc.CampaignInteractionType;
					if (interactionType == CampaignMode.InteractionType.None) {
						continue;
					}
					var identifier = $"CampaignInteractionIcon.{interactionType}".ToIdentifier();
					if (CursorOverInteractionIcon(npc, identifier)) {
						character.FocusedCharacter = npc;
						Character_DoInteractionUpdate.OverrideFocusedCharacter = (character, npc);
						focusedSomething = true;
					}
				}
				if (!focusedSomething) {
					Character_DoInteractionUpdate.OverrideFocusedCharacter = null;
				}
			}
		} catch (Exception exception) {
			ModUtils.Logging.PrintError(exception.ToString());
		}
	}

}
