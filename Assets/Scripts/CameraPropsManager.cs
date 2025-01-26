using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class CameraPropsManager : Singleton<CameraPropsManager>
{
    [Tooltip("SpeedDisplay")]
    public FloatRange speedBarRange;
    public Image speedImage;
    public float speedBarLerpSpeed;
    private float speedValue;
    private float targetSpeedValue;
    [Space]
    [Tooltip("Gun")]
    public Transform gunTF;
    public float verticalRecoil = 0.1f;
    public float recoilRecoveryRate = 0.5f;
    private Quaternion gunDefaultRot;
    [Space]
    [Tooltip("The Face")]
    public Image faceImage;
    public Sprite[] faceTxtrs;
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
    bool queueLock = false;
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
            StartTalking(10);
        }

        RecoilRecovery();
        SpeedBarUpdate();
    }

    public void SetSpeed(float _speed) 
    {
        targetSpeedValue = _speed;
    }

    public void Recoil() 
    {
        gunTF.localRotation = gunDefaultRot;
        gunTF.Rotate(verticalRecoil, 0, 0);
    }

    private void SpeedBarUpdate() 
    {
        if (speedValue != targetSpeedValue)
        {
            speedValue = Mathf.Lerp(speedValue, targetSpeedValue, speedBarLerpSpeed * Time.deltaTime);
        }
        speedImage.fillAmount = speedBarRange.ValueAsPercent(speedValue);
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

    public void QueueFace(Face newFace, float duration, bool lockQueue = false, bool purgeQueue = false) 
    {
        if (queueLock) return;

        if (purgeQueue)
        {
            faceQueue.Clear();
        }

        if (lockQueue) 
        {
            queueLock = true;
        }

        faceQueue.Enqueue(new FaceChange(newFace, duration));
        if (!changingFace)
        {
            StartCoroutine(FaceChanging());
        }
    }

    public void StartTalking(int loops) 
    {
        --loops;
        QueueFace(Face.TALKING, 0, false, true);
        for (int i = 0; i < loops; i++)
        {
            QueueFace(Face.NEUTRAL, 0);
            QueueFace(Face.TALKING, 0.01f);
        }
        QueueFace(Face.NEUTRAL, 0, true);
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
            faceImage.sprite = faceTxtrs[(int)nextChange.face];

            yield return new WaitForSeconds(nextChange.duration);
        }
        faceImage.sprite = faceTxtrs[(int)Face.NEUTRAL];

        changingFace = false;
        queueLock = false;
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

            QueueFace(Face.SAD, shakeRefreshDelay * 1.25f, false, true);
            yield return new WaitForSeconds(shakeRefreshDelay);
        }

        screenShakeTF.transform.localPosition = screenShakeTFDefaultPos;
        shakingScreen = false;
    }
}
