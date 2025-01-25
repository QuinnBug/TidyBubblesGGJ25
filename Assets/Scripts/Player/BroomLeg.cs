using UnityEngine;

public class BroomLeg : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private CleanBrushConstant walkingBrush;
    [SerializeField] private CleanBrushConstant slidingBrush;
    private CharacterState prevState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        walkingBrush.enabled = true;
        slidingBrush.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevState.Stance == playerCharacter.CurrentState.Stance) return;
        if (playerCharacter.CurrentState.Stance == Stance.Stand) {
            walkingBrush.enabled = true;
            slidingBrush.enabled = false;
        }
        else if (playerCharacter.CurrentState.Stance == Stance.Slide) {
            walkingBrush.enabled = false;
            slidingBrush.enabled = true;
        }
        else {
            walkingBrush.enabled = true;
            slidingBrush.enabled = false;
        }
        prevState = playerCharacter.CurrentState;
    }
}
