using Barotrauma;
using System.Runtime.CompilerServices;

namespace BarotraumaEasyOutposts;

public static partial class ForcedFocusManager {

	// Server needs to keep track of all clients' forced focus.
	private static ConditionalWeakTable<Character, Character> ForcedFocus { get; } = new();

	public static partial void SetForcedFocus(Character interactor, Character? focused) {
#if DEBUG
		bool hasFocus = TryGetForcedFocus(interactor, out var current);
		if ((hasFocus && current != focused) || (!hasFocus && focused != null)) {
			ModUtils.Logging.PrintMessage($"Setting forced focus for {interactor.Name} to {focused?.Name ?? "null"}");
		}
#endif
		if (focused == null) {
			ForcedFocus.Remove(interactor);
		} else {
			ForcedFocus.AddOrUpdate(interactor, focused);
			interactor.FocusedCharacter = focused;
		}
	}

	public static partial bool TryGetForcedFocus(Character interactor, out Character? focused) {
		return ForcedFocus.TryGetValue(interactor, out focused);
	}

}
