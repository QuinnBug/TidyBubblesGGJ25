using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{

    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [Space]
    [SerializeField] private Volume volume;
    [SerializeField] private StanceVignette stanceVignette;


    private InputActions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputActions = new InputActions();
        _inputActions.Enable();


        playerCharacter.Initialize();


        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        cameraSpring.Initialize();
        cameraLean.Initialize();

        stanceVignette.Initialize(volume.profile);
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    void Update()
    {
        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;

        //Get camera input and update
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();

        playerCamera.UpdateRotation(cameraInput);
        playerCamera.UpdatePostion(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide ,state.Acceleration, cameraTarget.up);
        stanceVignette.UpdateVignette(deltaTime, state.Stance);

        var characterInput = new CharacterInput {
            rotation = playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch = input.Crouch.IsPressed() ? CrouchInput.Toggle : CrouchInput.None,
            airSlam = input.AirSlam.triggered,
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.updateBody(deltaTime);

#if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                Teleport(hit.point);
            }
        }
#endif

    }
    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }

}
