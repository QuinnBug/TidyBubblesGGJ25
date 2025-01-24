using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private PlayerCharacter character;
    [SerializeField] private PlayerCamera playerCamera;


    private InputActions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputActions = new InputActions();
        _inputActions.Enable();


        character.Initialize();
        playerCamera.Initialize(character.GetCameraTarget());
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    void Update()
    {
        var input = _inputActions.Gameplay;

        //Get camera input and update
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        var characterInput = new CharacterInput {
            rotation    = playerCamera.transform.rotation,
            Move        = input.Move.ReadValue<Vector2>()
        };
        character.UpdateInput(characterInput);

    }

    private void LateUpdate()
    {
        playerCamera.UpdatePostion(character.GetCameraTarget());
    }
}
