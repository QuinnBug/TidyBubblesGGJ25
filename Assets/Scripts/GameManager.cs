using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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

    private List<GameObject> dirtystuff = new();
    private List<DirtObject> cleanedObjects = new();
    [SerializeField] private int percentageToWin = 80;

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

            for (int i = 0; i < 3; i++)
            {
                GameObject newFx = Instantiate(completeCleanFx);
                newFx.SetActive(false);
                cleanFxPool.Add(newFx);
            }
            GameObject[] dirtObjects = GameObject.FindGameObjectsWithTag("Dirt");
            dirtystuff = dirtObjects.ToList();
            Debug.Log($"Found {dirtystuff.Count} dirty objects.");
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
        var cleanedPercentage = ((float)cleanedObjects.Count / dirtystuff.Count) * 100f;
        if (cleanedPercentage > percentageToWin) {
            //Win;
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
    }
    public void AddCleanedObject(DirtObject dirtyObject) {
        cleanedObjects.Add(dirtyObject);
        var cleanedPercentage = ((float)cleanedObjects.Count / dirtystuff.Count) * 100f;

        Debug.Log($"Object has been cleaned. You are {cleanedPercentage} complete cleaning.");

    }

}
