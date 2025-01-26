using UnityEngine;
using UnityEngine.UIElements;

public enum SlamStrength {
    Soft,
    Medium,
    Hard
}
public class BroomLeg : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private CleanBrushConstant walkingBrush;
    [SerializeField] private CleanBrushConstant slidingBrush;
    [SerializeField] private DirtBrush floorSlamSoft;
    [SerializeField] private DirtBrush floorSlamMed;
    [SerializeField] private DirtBrush floorSlamHard;
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
    public void ShitOnFloor(SlamStrength strength) {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit)) {
            var textureCoord = hit.textureCoord;
            switch (strength) {
                case SlamStrength.Soft:
                    floorSlamSoft.CleanDirt(hit.collider.GetComponent<DirtObject>(), textureCoord);
                    break;
                case SlamStrength.Medium:
                    floorSlamMed.CleanDirt(hit.collider.GetComponent<DirtObject>(), textureCoord);
                    break;
                case SlamStrength.Hard:
                    floorSlamHard.CleanDirt(hit.collider.GetComponent<DirtObject>(), textureCoord);
                    break;
                default:
                    break;
            }
        }
    }
}
