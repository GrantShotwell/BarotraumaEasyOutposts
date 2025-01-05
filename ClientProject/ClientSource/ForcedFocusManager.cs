using Barotrauma;
using System.Diagnostics.CodeAnalysis;

namespace BarotraumaEasyOutposts;

public static partial class ForcedFocusManager {

	// Client should only need to keep track of their own forced focus.
	private static (Character character, Character clicked)? ForcedFocus { get; set; } = null;

	private static bool TryGetForcedFocus(
		[NotNullWhen(true)] out Character? character,
		[NotNullWhen(true)] out Character? clicked
	) {
		var value = ForcedFocus;
		if (value == null) {
			character = null;
			clicked = null;
			return false;
		} else {
			(character, clicked) = value.Value;
			return true;
		}
	}

	public static partial void SetForcedFocus(Character interactor, Character? focused) {
#if DEBUG
		bool hasFocus = TryGetForcedFocus(interactor, out var current);
		if ((hasFocus && current != focused) || (!hasFocus && focused != null)) {
			ModUtils.Logging.PrintMessage($"Setting forced focus for {interactor.Name} to {focused?.Name ?? "null"}");
		}
#endif
		ForcedFocus = focused == null ? null : (interactor, focused);
		if (focused != null) {
			interactor.FocusedCharacter = focused;
		}
	}

	public static partial bool TryGetForcedFocus(Character interactor, out Character? focused) {
		if (TryGetForcedFocus(out var otherInteractor, out var otherFocus) && interactor == otherInteractor) {
			focused = otherFocus;
			return true;
		} else {
			focused = null;
			return false;
		}
	}

}
