using Barotrauma;
using BarotraumaAsphyxia.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace BarotraumaEasyOutposts.Patches;

[HarmonyPatch(typeof(CampaignMode), nameof(CampaignMode.DoCharacterWait))]
class CampaignMode_DoCharacterWait {

	public static bool Prefix(CampaignMode __instance, ref IEnumerable<CoroutineStatus> __result, Character npc, Character interactor) {
		__result = GetCoroutine(__instance, npc, interactor);
		return false;
	}

	public static IEnumerable<CoroutineStatus> GetCoroutine(CampaignMode __instance, Character npc, Character interactor) {
		// Original function with one line changed.

		if (npc == null || interactor == null) {
			yield return CoroutineStatus.Failure;
			yield break;
		}

		HumanAIController? humanAI = npc.AIController as HumanAIController;
		if (humanAI == null) {
			yield return CoroutineStatus.Success;
			yield break;
		}

		var waitOrder = OrderPrefab.Prefabs["wait"].CreateInstance(OrderPrefab.OrderTargetType.Entity);
		humanAI.SetForcedOrder(waitOrder);
		var waitObjective = humanAI.ObjectiveManager.ForcedOrder;
		humanAI.FaceTarget(interactor);

		while (
			!npc.Removed && !interactor.Removed &&
			( // This was changed to not unfocus when forced.
				Character_DoInteractionUpdate.TryGetOverrideFocusedCharacter(out var character, out var clicked) &&
				character == interactor &&
				clicked == npc ||
				Vector2.DistanceSquared(npc.WorldPosition, interactor.WorldPosition) < 300.0f * 300.0f
			) &&
			humanAI.ObjectiveManager.ForcedOrder == waitObjective &&
			humanAI.AllowCampaignInteraction() &&
			!interactor.IsIncapacitated
		) {
			yield return CoroutineStatus.Running;
		}

#if CLIENT
		__instance.ShowCampaignUI = false;
#endif
		if (!npc.Removed) {
			humanAI.ClearForcedOrder();
		}
		yield return CoroutineStatus.Success;
	}

}
