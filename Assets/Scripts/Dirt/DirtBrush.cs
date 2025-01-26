using UnityEngine;

public class DirtBrush : MonoBehaviour
{
    [SerializeField] private BrushData brushData;
    public Texture2D BrushTexture => brushData.BrushTexture;
    private Color[] brushPixels;
    private int brushWidth;
    private int brushHeight;
    public Vector2Int BrushSize;
    private Color colourToApply;
    DirtCleanData cleanData;
    private void Awake() {
       
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BrushSize = new Vector2Int((int)(brushData.BrushTexture.width * brushData.BrushSizeMultiplier.x), (int)(brushData.BrushTexture.height * brushData.BrushSizeMultiplier.y));
        brushPixels = BrushTexture.GetPixels();
        brushWidth = BrushTexture.width;
        brushHeight = BrushTexture.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CleanDirt(DirtObject dirt, Vector2 textureCoordinate) {
        var pixelX = (int)(textureCoordinate.x * dirt.Texture.width);
        var pixelY = (int)(textureCoordinate.y * dirt.Texture.height);
        var paintPosition = new Vector2Int(pixelX, pixelY);
        Vector2Int paintPixel = new();
        Color dirtCol = new();
        Color pixelDirt = new();
        cleanData = new();
        cleanData.DirtPixels.Capacity = (BrushSize.x * BrushSize.y);

        //Debug.Log($"Pixel position: {pixelX} / {texCoord.x}, {pixelY} / {texCoord.y}");
        //Set pixels in the texture
        for (int x = 0; x < BrushSize.x; x++) {
            for (int y = 0; y < BrushSize.y; y++) {
                paintPixel = paintPosition + new Vector2Int(x, y);
                dirtCol = dirt.GetColour(paintPixel.x, paintPixel.y);

                //  Attempt to optimise a tiny bit by skipping clean pixels
                if (dirtCol.g <= dirt.CleanlinessTolerance) {
                    continue;
                }
                pixelDirt = brushPixels[x + brushWidth * y];
                colourToApply = new Color(0, dirtCol.g * pixelDirt.g, 0);
                cleanData.AddPixelData(paintPixel, colourToApply);
            }
        }
        if (cleanData.DirtPixels.Count == 0) {
            return;
        }
        dirt.CleanLocation(cleanData);
    }
}
