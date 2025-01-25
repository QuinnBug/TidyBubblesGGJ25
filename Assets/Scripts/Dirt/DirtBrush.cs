using UnityEngine;

public class DirtBrush : MonoBehaviour
{
    [SerializeField] private BrushData brushData;
    public Texture2D BrushTexture => brushData.BrushTexture;
    public Vector2Int BrushSize => new Vector2Int((int)(brushData.BrushTexture.width * brushData.BrushSizeMultiplier.x), (int)(brushData.BrushTexture.height * brushData.BrushSizeMultiplier.y));
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CleanDirt(DirtObject dirt, Vector2 textureCoordinate) {
        var pixelX = (int)(textureCoordinate.x * dirt.Texture.width);
        var pixelY = (int)(textureCoordinate.y * dirt.Texture.height);
        var paintPosition = new Vector2Int(pixelX, pixelY);

        //Debug.Log($"Pixel position: {pixelX} / {texCoord.x}, {pixelY} / {texCoord.y}");
        //Set pixels in the texture
        DirtCleanData cleanData = new();
        for (int x = 0; x < BrushSize.x; x++) {
            for (int y = 0; y < BrushSize.y; y++) {
                var pixelDirtMask = dirt.GetColour(paintPosition.x + x, paintPosition.y + y);
                var paintPixel = paintPosition + new Vector2Int(x, y);
                var dirtCol = dirt.GetColour(paintPixel.x, paintPixel.y);

                var pixelDirt = BrushTexture.GetPixel(x, y);
                var colourToApply = new Color(0, dirtCol.g * pixelDirt.g, 0);
                cleanData.AddCleanedPixel(paintPixel, colourToApply);


                dirt.Texture.SetPixel(paintPixel.x, paintPixel.y, colourToApply);
            }
        }
        Debug.Log("Painted");
        dirt.CleanLocation(cleanData);
    }
}
