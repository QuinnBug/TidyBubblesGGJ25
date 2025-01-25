using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPropsManager : Singleton<CameraPropsManager>
{
    public float shakeStrength;
    public float shakeDrag;
    public float shakeMaxIntensity;
    public FloatRange shakeRange;
    public Transform screenShakeTF;

    bool shakingScreen = false;
    [Space]
    [SerializeField] private float shakeIntensity;
    Vector2[] shakeDirections = 
        { new Vector2(1, 0.5f), new Vector2(-1, 0.5f), new Vector2(1, -0.5f), new Vector2(-1, -0.5f) };


    void Start()
    {
        
    }

    private void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) 
        {
            AddScreenShake(5);
        }
    }

    public void AddScreenShake(float value) 
    {
        shakeIntensity = Mathf.Clamp(shakeIntensity + value, 0, shakeMaxIntensity);
        if (!shakingScreen) { StartCoroutine(ScreenShaking()); }
    }

    IEnumerator ScreenShaking()
    {
        shakingScreen = true;
        int dirIndex = 0;
        bool doShakeOut = true;
        while (shakeIntensity > 0)
        {
            if (doShakeOut)
            {
                float shakeScale = shakeRange.Clamp(shakeIntensity * shakeStrength);
                screenShakeTF.Translate(shakeDirections[dirIndex] * shakeScale, screenShakeTF.parent);
                ++dirIndex;
                if (dirIndex >= shakeDirections.Length) { dirIndex = 0; }

                doShakeOut = false;
            }
            else
            {
                screenShakeTF.transform.localPosition = Vector3.zero;
                doShakeOut = true;
            }

            shakeIntensity *= shakeDrag;
            if (shakeIntensity <= 0.1f) { shakeIntensity = 0; }

            yield return new WaitForSeconds(0.1f);
        }

        screenShakeTF.transform.localPosition = Vector3.zero;
        shakingScreen = false;
    }
}
