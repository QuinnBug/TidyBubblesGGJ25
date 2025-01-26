using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [Tooltip("Game Values")]
    public float[] speedBoundaries;
    public FloatRange voicelineDelayRange;
    SceneId currentScene;
    PlayerCharacter player;
    float playerSpeed = 0;
    int intensityLevel = 0;
    [SerializeField] private GameObject completeCleanFx;
    private List<GameObject> cleanFxPool = new();
    float voicelineTimer = 1;

    private void Start()
    {
        SceneHandler.OnSceneChange += OnSceneLoaded;

       
    }

    void OnSceneLoaded(int sceneNumber) 
    {
        cleanFxPool.Clear();
        currentScene = (SceneId)sceneNumber;
        if (currentScene == SceneId.GAME)
        {
            player = FindFirstObjectByType<PlayerCharacter>();

            for (int i = 0; i < 3; i++) {
                GameObject newFx = Instantiate(completeCleanFx);
                newFx.SetActive(false);
                cleanFxPool.Add(newFx);
            }
        }
        else
        {
            player = null;
        }
    }

    private void Update()
    {
        switch (currentScene)
        {
            case SceneId.MENU:
                AudioManager.Instance.SetMusicLevel(-1);
                break;
            case SceneId.GAME:
                GameUpdate();
                break;
            case SceneId.END:
                break;
            default:
                Debug.LogAssertion("Scene not updated in GameManager");
                break;
        }
    }

    void GameUpdate()
    {
        playerSpeed = player.CurrentState.Velocity.magnitude;
        CameraPropsManager.Instance.SetSpeed(playerSpeed);

        intensityLevel = -1;
        foreach (var item in speedBoundaries)
        {
            if (playerSpeed >= item)
            {
                intensityLevel++;
            }
        }

        AudioManager.Instance.SetMusicLevel(intensityLevel);
        if (intensityLevel >= 1)
        {
            if (intensityLevel >= 2)
            {
                CameraPropsManager.Instance.QueueFace(CameraPropsManager.Face.HAPPY, 0.01f);
            }

            if (voicelineTimer <= 0)
            {
                AudioManager.Instance.RandomVoiceLine();
                voicelineTimer = voicelineDelayRange.RandomValue();
            }
            else
            {
                voicelineTimer -= Time.deltaTime;
            }
        }
    }

        public void PlayCleanVfx(Vector3 location)
        {
            if (cleanFxPool.Find(x => !x.activeInHierarchy) != null)
            {
                var activeFx = cleanFxPool.Find(x => !x.activeInHierarchy);
                activeFx.transform.position = location;
                activeFx.SetActive(true);
            }
            else
            {
                GameObject newFx = Instantiate(completeCleanFx);
                newFx.transform.position = location;
                newFx.SetActive(true);
                cleanFxPool.Add(newFx);
            }
        }
}
