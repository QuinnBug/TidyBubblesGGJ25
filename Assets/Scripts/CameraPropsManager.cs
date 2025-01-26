using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class CameraPropsManager : Singleton<CameraPropsManager>
{
    [Tooltip("Gun")]
    public Transform gunTF;
    public float verticalRecoil = 0.1f;
    public float recoilRecoveryRate = 0.5f;
    private Quaternion gunDefaultRot;
    [Space]
    [Tooltip("The Face")]
    public RawImage faceImage;
    public Texture2D[] faceTxtrs;
    public Color damageColour;
    private bool changingFace = false;
    public enum Face { HAPPY, NEUTRAL, TALKING, SAD }
    private struct FaceChange
    {
        public Face face;
        public float duration;
        public FaceChange(Face _face, float _duration) { face = _face; duration = _duration; }
    }
    private Queue<FaceChange> faceQueue = new Queue<FaceChange>();
    [Space]
    [Tooltip("Camera Shake")]
    public float shakeStrength;
    public float shakeDrag;
    public float shakeMaxIntensity;
    public float shakeRefreshDelay;
    public FloatRange shakeRange;
    public Transform screenShakeTF;

    private Vector3 screenShakeTFDefaultPos;
    private bool shakingScreen = false;
    private float shakeIntensity;
    Vector2[] shakeDirections = 
        { new Vector2(1, 0.5f), new Vector2(-1, 0.5f), new Vector2(1, -0.5f), new Vector2(-1, -0.5f) };


    void Start()
    {
        screenShakeTFDefaultPos = screenShakeTF.localPosition;
        gunDefaultRot = gunTF.localRotation;
    }

    private void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) 
        {
            AddScreenShake(1);
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            QueueFace(Face.TALKING, 0.1f);
            QueueFace(Face.NEUTRAL, 0.01f);
        }

        RecoilRecovery();
    }
    
    public void Recoil() 
    {
        gunTF.localRotation = gunDefaultRot;
        gunTF.Rotate(verticalRecoil, 0, 0);
    }

    private void RecoilRecovery() 
    {
        if (gunTF.localRotation != gunDefaultRot)
        {
            gunTF.localRotation = Quaternion.Lerp(gunTF.localRotation, gunDefaultRot, recoilRecoveryRate);
            if (Quaternion.Angle(gunTF.localRotation, gunDefaultRot) < 1)
            {
                gunTF.localRotation = gunDefaultRot;
            }
        }
    }

    public void QueueFace(Face newFace, float duration, bool purgeQueue = false) 
    {
        if (purgeQueue)
        {
            faceQueue.Clear();
        }

        faceQueue.Enqueue(new FaceChange(newFace, duration));
        if (!changingFace)
        {
            StartCoroutine(FaceChanging());
        }
    }

    public void AddScreenShake(float value) 
    {
        shakeIntensity = Mathf.Clamp(shakeIntensity + value, 0, shakeMaxIntensity);
        if (!shakingScreen) { StartCoroutine(ScreenShaking()); }
    }

    IEnumerator FaceChanging() 
    {
        changingFace = true;
        FaceChange nextChange;
        while (faceQueue.TryDequeue(out nextChange)) 
        {
            faceImage.texture = faceTxtrs[(int)nextChange.face];

            yield return new WaitForSeconds(nextChange.duration);
        }
        faceImage.texture = faceTxtrs[(int)Face.NEUTRAL];

        changingFace = false;
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
                screenShakeTF.Translate(shakeDirections[dirIndex].normalized * shakeScale, screenShakeTF.parent);
                ++dirIndex;
                if (dirIndex >= shakeDirections.Length) { dirIndex = 0; }

                doShakeOut = false;
            }
            else
            {
                screenShakeTF.transform.localPosition = screenShakeTFDefaultPos;
                doShakeOut = true;
            }

            shakeIntensity *= shakeDrag;
            if (shakeIntensity <= 0.1f) { shakeIntensity = 0; }

            QueueFace(Face.SAD, 0.1f, true);
            yield return new WaitForSeconds(shakeRefreshDelay);
        }

        screenShakeTF.transform.localPosition = screenShakeTFDefaultPos;
        shakingScreen = false;
    }
}
