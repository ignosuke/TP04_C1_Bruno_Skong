using UnityEngine;

[CreateAssetMenu(fileName = "ColorChangerDataSo", menuName = "Data/ColorChanger/ColorChangerData")]
// Suggestion: este ScriptableObject no tiene ningún .asset creado ni lo referencia ningún script. Las dos duraciones que define terminaron viviendo en PlayerDataSo (colorFlashHoldDuration y colorFlashFadeDuration).
// Borralo, o usalo y sacá esos dos campos de PlayerDataSo.
public class ColorChangerDataSo : ScriptableObject
{
    public float holdDuration = .5f;
    public float fadeDuration = .5f;
}
