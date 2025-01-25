using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mesh Collider required to get the texture coordinates
[RequireComponent(typeof(MeshCollider))]
public class DirtObject : MonoBehaviour {

    [SerializeField] private Texture2D dirtMaskBase;
    [SerializeField] private Texture2D cleanTexture;
    private Texture2D templateDirtMask;
    public Texture2D Texture => templateDirtMask;

    [SerializeField] private float cleanlinessTolerance = 0.35f; // How clean a pixel has to be to be considered clean
    [SerializeField] private float cleanlinessThreshold = 5f; //80f; // How clean the object has to be to be considered clean
    public float CleanlinessTolerance => cleanlinessTolerance;
    public float Cleanliness => GetCleanliness() * 100f;
    public int CleanlinessRounded => (int)Cleanliness;

    public bool IsClean => Cleanliness < cleanlinessThreshold ? false : true;

    private int totalPixels = 0;
    private int cleanPixels = 0;

    bool flagTextureUpdate = false;

    void Start() {
        CreateTexture();
    }


    private void CreateTexture() {
        if (dirtMaskBase == null) {
            Debug.LogWarning($"{gameObject.name} does not have assigned textures.");
            return;
        }
        //  We don't want to modify the original texture, so we create a copy of it  
        var renderer = GetComponent<Renderer>();
        if (renderer != null) {
            templateDirtMask = new Texture2D(dirtMaskBase.width, dirtMaskBase.height);
            templateDirtMask.SetPixels(dirtMaskBase.GetPixels());
            templateDirtMask.Apply();
            renderer.material.SetTexture("_DirtMask", templateDirtMask);
            Debug.Log("Dirt texture applied");
        }
        totalPixels = templateDirtMask.GetPixels().Length;
    }
    public Color GetColour(int pixelX, int pixelY) {
        return templateDirtMask.GetPixel(pixelX, pixelY);
    }
    public void CleanLocation(DirtCleanData cleanData) {
        if (IsClean) return;
        var cleanedPixels = cleanData.GetCleanData();
        for (int i = 0; i < cleanedPixels.Count; i++) {
            if (cleanedPixels[i].Item2.g < cleanlinessTolerance && templateDirtMask.GetPixel(cleanedPixels[i].Item1.x, cleanedPixels[i].Item1.y).g > cleanlinessTolerance) {
                cleanPixels++;
            }
            templateDirtMask.SetPixel(cleanedPixels[i].Item1.x, cleanedPixels[i].Item1.y, cleanedPixels[i].Item2);
        }
        if (!flagTextureUpdate) {
            flagTextureUpdate = true;
            StartCoroutine(UpdateTexture());
        } 

        
        if (IsClean) {
            ClearAllDirt();
        }
    }

    private IEnumerator UpdateTexture() {
        // Magic optimisation number
        yield return new WaitForSeconds(0.045f);
        templateDirtMask.Apply();
        flagTextureUpdate = false;
    }
    public void ClearAllDirt() {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_DirtMask", cleanTexture);
    }
    private float GetCleanliness() {
        return (float)cleanPixels / totalPixels;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<DirtBrush>(out DirtBrush brush)) {
            Debug.Log($"What the fuck");
        }
    }
}
public class DirtCleanData {
    private List<System.Tuple<Vector2Int, Color>> dirtPixels;
    public List<System.Tuple<Vector2Int, Color>> DirtPixels => dirtPixels;


    #region Constructors
    public DirtCleanData() {
        dirtPixels = new List<System.Tuple<Vector2Int, Color>>();
    }
    public DirtCleanData(List<System.Tuple<Vector2Int, Color>> newData) {
        this.dirtPixels = newData;
    }
    #endregion
    public List<System.Tuple<Vector2Int, Color>> GetCleanData() {
        return dirtPixels;
    }
    public void AddPixelData(Vector2Int pixel, Color colour) {
        dirtPixels.Add(new System.Tuple<Vector2Int, Color>(pixel, colour));
    }
}