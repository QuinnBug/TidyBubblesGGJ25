using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    SceneId currentScene;
    PlayerCharacter player;

    private void Start()
    {
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
        CameraPropsManager.Instance.SetSpeed(player.CurrentState.Velocity.magnitude);
    }
}
