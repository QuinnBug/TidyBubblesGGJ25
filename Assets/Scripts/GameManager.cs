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
    float voicelineTimer;

    private void Start()
    {
        voicelineTimer = voicelineDelayRange.Min;
        SceneHandler.OnSceneChange += OnSceneLoaded;
    }

    void OnSceneLoaded(int sceneNumber) 
    {
        currentScene = (SceneId)sceneNumber;
        if (currentScene == SceneId.GAME)
        {
            player = FindFirstObjectByType<PlayerCharacter>();
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
