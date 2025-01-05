using Barotrauma;
using System.Diagnostics.CodeAnalysis;

namespace BarotraumaEasyOutposts;

public static partial class ForcedFocusManager {

	/// <summary>
	/// Sets the forced focus for a character and assigns <see cref="Character.FocusedCharacter"/> when not <see langword="null"/>.
	/// </summary>
	/// <param name="interactor">The character to set the focus of.</param>
	/// <param name="focused">The focus of <paramref name="interactor"/>, or <see langword="null"/> to unset.</param>
	public static partial void SetForcedFocus(Character interactor, Character? focused = null);

	/// <summary>
	/// Gets the forced focus of a character.
	/// </summary>
	/// <param name="interactor">The character to get the forced focus of.</param>
	/// <param name="focused">The focus of <paramref name="interactor"/>, otherwise <see langword="null"/>.</param>
	/// <returns>Returns <see langword="true"/> when there is a forced focus.</returns>
	public static partial bool TryGetForcedFocus(Character interactor, [NotNullWhen(true)] out Character? focused);

	public static void UpdateForcedFocus(Character interactor) {
		if (!TryGetForcedFocus(interactor, out var focused)) {
			return;
		}
		interactor.FocusedCharacter = focused;
	}

}
