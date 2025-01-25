using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneId 
{
    MENU,
    HQ,
    WORLD_MAP,
    FIGHT,
    GAME_OVER
}

public class SceneHandler: PersistentSingleton<SceneHandler>
{
    public delegate void SceneChange(int _sceneId);
    public static event SceneChange OnSceneChange;

    public float transitionSpeed;
    [Space]
    [SerializeField] Animator transitionAnimator;

    public void StartTransition()
    {
        transitionAnimator.speed = transitionSpeed;
        transitionAnimator.Play("SwipeIn_Right");
    }

    public void EndTransition()
    {
        transitionAnimator.speed = transitionSpeed;
        transitionAnimator.Play("SwipeOut_Left");
    }

    public void TransitionScene(int _sceneToTransitionTo)
    {
        if (SceneManager.GetActiveScene().buildIndex != _sceneToTransitionTo)
        {
            StartTransition();
            StartCoroutine(waitForSceneTransition(_sceneToTransitionTo, 1));
        }
    }

    IEnumerator waitForSceneTransition(int sceneNumber, float waitTime)
    {
        float timer = waitTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        
        SceneManager.LoadScene(sceneNumber);
        StartCoroutine(waitForSceneLoad(sceneNumber));
    }

    IEnumerator waitForSceneLoad(int sceneNumber)
    {
        while (SceneManager.GetActiveScene().buildIndex != sceneNumber)
        {
            yield return null;
        }

        EndTransition();

        // Do anything after proper scene has been loaded
        if (SceneManager.GetActiveScene().buildIndex == sceneNumber)
        {
            OnSceneChange?.Invoke(sceneNumber);
        }

    }
}
