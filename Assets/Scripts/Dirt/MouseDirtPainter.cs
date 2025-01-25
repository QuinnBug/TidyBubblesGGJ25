using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MouseDirtPainter : MonoBehaviour
{
    [SerializeField] private DirtBrush brush;   
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
                brush.CleanDirt(dirt, texCoord);
            }
        }
    }
}
