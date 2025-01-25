using UnityEngine;
using KinematicCharacterController;
using JetBrains.Annotations;

public enum CrouchInput
{
    None, Toggle
}

public enum Stance
{
    Stand, Crouch
}

public struct CharacterInput
{
    public Quaternion rotation;

    public Vector2 Move;

    public bool Jump;

    public CrouchInput Crouch;
}


public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    [Space]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float crouchSpeed = 7f;
    [Space]
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float gravity = -90f;
    [Space]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchHeightResponse = 15f;

    [Range(0f,1f)]
    [SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0f, 1f)]
    [SerializeField] private float crouchCameraTargetHeight = 0.7f;



    private Stance _stance;



    private Vector3 _requestedMovement;

    private Quaternion _requestedRotation;

    private bool _requestedJump;

    private bool _requestedCrouch;

    private Collider[] _uncrouchOverlapResults;
    public void Initialize()
    {
        _stance = Stance.Stand;
        _uncrouchOverlapResults = new Collider[8];
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

        _requestedMovement = input.rotation * _requestedMovement;

        _requestedJump = _requestedJump || input.Jump;

        _requestedCrouch = input.Crouch switch
        {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            _ => _requestedCrouch
        };
    }

    public void updateBody(float deltaTIme)
    {
        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;
        var cameraTargetHeight = currentHeight *
            (
            _stance is Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight
            );

        var rootTargetScale = new Vector3(1f,normalizedHeight , 1f);
        cameraTarget.localPosition = Vector3.Lerp
            (
            a: cameraTarget.localPosition,
            b: new Vector3(0f, cameraTargetHeight, 0f),
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTIme)
            );
        root.localScale = Vector3.Lerp
            (
            a: root.localScale,
            b: rootTargetScale,
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTIme)
            );
    }
    public void AfterCharacterUpdate(float deltaTime)
    {
        // Uncrouch.
        if (!_requestedCrouch && _stance is not Stance.Stand)
        {
            //Test stand up
            motor.SetCapsuleDimensions
                (
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
                );

            //Check for terrain overlap
            var pos = motor.TransientPosition;
            var rot = motor.TransientRotation;
            var mask = motor.CollidableLayers;

            if (motor.CharacterOverlap(pos,rot,_uncrouchOverlapResults,mask,QueryTriggerInteraction.Ignore) > 0)
            {
                //Recrouch
                _requestedCrouch = true;
                motor.SetCapsuleDimensions
                    (
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                    );
            }
            else
            {
                _stance = Stance.Stand;
            }
        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        //Crouch
        if (_requestedCrouch && _stance is Stance.Stand)
        {
            _stance = Stance.Crouch;
            motor.SetCapsuleDimensions
                (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
                );

        }
    }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

        var forward = Vector3.ProjectOnPlane
            (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
            );
        if (forward != Vector3.zero)
        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {

        //If on the ground:
        if (motor.GroundingStatus.IsStableOnGround)
        {
            var groundedMovement = motor.GetDirectionTangentToSurface
                (

                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal

                ) * _requestedMovement.magnitude;


            var speed = _stance is Stance.Stand ? walkSpeed : crouchSpeed;

            currentVelocity = groundedMovement * speed;
        }
        //Else in the air:
        else
        {
            //Gravity
            currentVelocity += deltaTime * -gravity * motor.CharacterUp;
        }

        if (_requestedJump)
        {
            _requestedJump = false;

            //Unstick from ground
            motor.ForceUnground(time: 0f);

            //set minimum vertical speed to jump speed
            var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
            //Add the difference
            currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
        }
    }

    public Transform GetCameraTarget() => cameraTarget;
}
