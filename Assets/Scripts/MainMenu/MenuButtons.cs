using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public float scrollSpeed;
    public RectTransform[] scrollObjects;

    public void OnPlayClicked() 
    {
        SceneHandler.Instance.TransitionScene(1);
    }

    public void OnCreditsClicked() 
    {

    }

    private void Update()
    {
        foreach (var item in scrollObjects)
        {
            item.Translate(Vector2.up * scrollSpeed * Time.deltaTime);
        }
    }
}
