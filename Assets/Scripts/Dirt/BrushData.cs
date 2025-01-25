using UnityEngine;

[CreateAssetMenu(fileName = "BrushData", menuName = "Brush/BrushData")]
public class BrushData : ScriptableObject {
    public Texture2D BrushTexture;
    public Vector2 BrushSizeMultiplier;
    public Color BrushColor; // unused
}
