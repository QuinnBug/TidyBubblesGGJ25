using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MouseDirtPainter : MonoBehaviour
{
    [SerializeField] private Texture2D brush;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get player mouse input
        if (Input.GetMouseButton(0)) {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var raycastHit);
            if (raycastHit.collider.TryGetComponent<DirtObject>(out var dirt)) {
                //Debug.Log(raycastHit.collider.name);
                Vector2 texCoord = raycastHit.textureCoord;

                //get the pixel position in the texture
                var pixelX = (int)(texCoord.x * dirt.Texture.width);
                var pixelY = (int)(texCoord.y * dirt.Texture.height);
                var paintPosition = new Vector2Int(pixelX, pixelY);

                //Debug.Log($"Pixel position: {pixelX} / {texCoord.x}, {pixelY} / {texCoord.y}");
                //Set pixels in the texture
                for (int x = 0; x < brush.width; x++) {
                    for (int y = 0; y < brush.height; y++) {
                        var pixelDirtMask = dirt.GetColour(paintPosition.x + x, paintPosition.y + y);
                        var paintPixel = paintPosition + new Vector2Int(x, y);
                        var dirtCol = dirt.GetColour(paintPixel.x, paintPixel.y);
                        var pixelDirt = brush.GetPixel(x, y);
                        dirt.Texture.SetPixel(paintPixel.x, paintPixel.y, new Color(0, dirtCol.g * pixelDirt.g, 0));
                    }
                }
                Debug.Log("Painted");
                dirt.Texture.Apply();

            }
        }
    }
}
