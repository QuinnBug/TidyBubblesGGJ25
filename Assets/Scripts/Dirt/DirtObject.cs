using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mesh Collider required to get the texture coordinates
[RequireComponent(typeof(MeshCollider))]
public class DirtObject : MonoBehaviour {

    [SerializeField] private Texture2D dirtMaskBase;
    private int width;
    private int height;
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

    private Color[] dirtPixels;

    private void Awake() {
    }
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
        }
        totalPixels = templateDirtMask.GetPixels().Length;
        dirtPixels = templateDirtMask.GetPixels();
        width = templateDirtMask.width;
        height = templateDirtMask.height;

    }
    public Color GetColour(int pixelX, int pixelY) {
        return templateDirtMask.GetPixel(pixelX, pixelY);
    }
    public void CleanLocation(DirtCleanData cleanData) {
        if (IsClean) return;
        var cleanedPixels = cleanData.GetCleanData();
        for (int i = 0; i < cleanedPixels.Count; i++) {
            if (cleanedPixels[i].Item2.g < cleanlinessTolerance && dirtPixels[cleanedPixels[i].Item1.x + width * cleanedPixels[i].Item1.y].g > cleanlinessTolerance) {
                cleanPixels++;
            }
            dirtPixels[cleanedPixels[i].Item1.x + width * cleanedPixels[i].Item1.y ] = cleanedPixels[i].Item2;
            //  I really want to optimise this but it keeps throwing
            //templateDirtMask.SetPixel(cleanedPixels[i].Item1.x, cleanedPixels[i].Item1.y, cleanedPixels[i].Item2);

        }
        
        if (IsClean) {
            ClearAllDirt();
        }
        templateDirtMask.SetPixels(dirtPixels);
        templateDirtMask.Apply();
    }

        
    public void ClearAllDirt() {
        var renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_DirtMask", cleanTexture);
        renderer.material.SetTexture("_DirtTexture", null);

        GameManager.Instance.AddCleanedObject(this);
        GameManager.Instance.PlayCleanVfx(transform.position);

    }
    private float GetCleanliness() {
        return (float)cleanPixels / totalPixels;
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