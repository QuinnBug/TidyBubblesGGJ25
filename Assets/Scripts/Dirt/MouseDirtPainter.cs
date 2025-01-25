using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(DirtBrush))]
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
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var raycastHit))
                return;
            if (raycastHit.collider.TryGetComponent<DirtObject>(out var dirt)) {
                //Debug.Log(raycastHit.collider.name);
                Vector2 texCoord = raycastHit.textureCoord;
                brush.CleanDirt(dirt, texCoord);
            }
            else {
                Debug.Log("No dirt object found");
            }
        }
    }
}
