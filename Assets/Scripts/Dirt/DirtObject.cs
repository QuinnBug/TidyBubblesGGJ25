using System.Collections.Generic;
using UnityEngine;

//Mesh Collider required to get the texture coordinates
[RequireComponent(typeof(MeshCollider))]
public class DirtObject : MonoBehaviour {

    [SerializeField] private Texture2D dirtMaskBase;
    private Texture2D templateDirtMask;
    public Texture2D Texture => templateDirtMask;
    void Start() {
        CreateTexture();
    }

    
    private void CreateTexture() {
        //  We don't want to modify the original texture, so we create a copy of it  
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
    public void CleanLocation(DirtCleanData cleanData) {
        var cleanedPixels = cleanData.GetCleanData();
        for (int i = 0; i < cleanedPixels.Count; i++) {
            templateDirtMask.SetPixel(cleanedPixels[i].Item1.x, cleanedPixels[i].Item1.y, cleanedPixels[i].Item2);
        }
        templateDirtMask.Apply();
    }
}
public class DirtCleanData {
    private List<System.Tuple<Vector2Int, Color>> cleanedPixels;
    public List<System.Tuple<Vector2Int, Color>> CleanedPixels => cleanedPixels;


    #region Constructors
    public DirtCleanData() {
        cleanedPixels = new List<System.Tuple<Vector2Int, Color>>();
    }
    public DirtCleanData(List<System.Tuple<Vector2Int, Color>> newData) {
        this.cleanedPixels = newData;
    }
    #endregion
    public List<System.Tuple<Vector2Int, Color>> GetCleanData() {
        return cleanedPixels;
    }
    public void AddCleanedPixel(Vector2Int pixel, Color colour) {
        cleanedPixels.Add(new System.Tuple<Vector2Int, Color>(pixel, colour));
    }


}
