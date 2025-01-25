using UnityEngine;

public class DirtObject : MonoBehaviour {

    [SerializeField] private Texture2D dirtMaskBase;
    private Texture2D templateDirtMask;
    public Texture2D Texture => templateDirtMask;
    void Start() {
        CreateTexture();
    }
    private void CreateTexture() {
        var renderer = GetComponent<Renderer>();
        if (renderer != null) {
            templateDirtMask = new Texture2D(dirtMaskBase.width, dirtMaskBase.height);
            templateDirtMask.SetPixels(dirtMaskBase.GetPixels());
            templateDirtMask.Apply();
            renderer.material.SetTexture("_DirtMask", templateDirtMask);
            Debug.Log("Dirt texture applied");
        }
    }
    public Color GetColour(int pixelX, int pixelY) {
        return templateDirtMask.GetPixel(pixelX, pixelY);
    }
    
}
