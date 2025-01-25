using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void OnPlayClicked() 
    {
        SceneHandler.Instance.TransitionScene(1);
    }

    public void OnCreditsClicked() 
    {

    }
}
