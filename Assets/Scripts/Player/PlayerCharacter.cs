using UnityEngine;
using KinematicCharacterController;
using JetBrains.Annotations;

public enum CrouchInput
{
    None, Toggle
}

public enum Stance
{
    Stand, Crouch, Slide
}

public struct CharacterState
{
    public bool Grounded;
    public Stance Stance;
}

public struct CharacterInput
{
    public Quaternion rotation;

    public Vector2 Move;

    public bool Jump;

    public bool JumpSustain;

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
    [Range(0f,1f)]
    [SerializeField] private float jumpSustainGravity = 0.4f;
    [SerializeField] private float gravity = -90f;
    [Space]
    [SerializeField] private float slideStartSpeed = 25f;
    [SerializeField] private float slideEndSpeed = 15f;
    [SerializeField] private float slideFriction = 0.8f;
    [Space]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;
    [Space]
    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 70f;
    [Space]
    [SerializeField] private float crouchHeightResponse = 15f;


    [Range(0f,1f)]
    [SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0f, 1f)]
    [SerializeField] private float crouchCameraTargetHeight = 0.7f;

    private CharacterState _state;

    private CharacterState _lastState;

    private CharacterState _tempState;



    private Vector3 _requestedMovement;

    private Quaternion _requestedRotation;

    private bool _requestedJump;

    private bool _requestedSustainedJump;

    private bool _requestedCrouch;

    private Collider[] _uncrouchOverlapResults;
    public void Initialize()
    {
        _state.Stance = Stance.Stand;
        _lastState = _state;
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
        _requestedSustainedJump = input.JumpSustain;

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
            _state.Stance is Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight
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
        if (!_requestedCrouch && _state.Stance is not Stance.Stand)
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
                _state.Stance = Stance.Stand;
            }
        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        _tempState = _state;
        //Crouch
        if (_requestedCrouch && _state.Stance is Stance.Stand)
        {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions
                (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
                );

        }
        // Update state to reflect relevant motor properties.
        _state.Grounded = motor.GroundingStatus.IsStableOnGround;

        // Update last state
        _lastState = _tempState;
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
        if (!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide)
        {
            _state.Stance = Stance.Crouch;
        }
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
            // Snap the requested movement direction to the angle of the surface
            // the character is currently walking on.
            var groundedMovement = motor.GetDirectionTangentToSurface
            (

                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal

            ) * _requestedMovement.magnitude;


            //Start sliding
            var moving = groundedMovement.sqrMagnitude > 0;
            var crouching = _state.Stance is Stance.Crouch;
            var wasStanding = _lastState.Stance is Stance.Stand;
            var wasInAir = !_lastState.Grounded;
            if (moving && crouching && (wasStanding || wasInAir))
            {
                _state.Stance = Stance.Slide;
                var slideSpeed = Mathf.Max(slideStartSpeed, currentVelocity.magnitude);
                currentVelocity = motor.GetDirectionTangentToSurface
                    (
                        direction: currentVelocity,
                        surfaceNormal: motor.GroundingStatus.GroundNormal
                    ) * slideSpeed;

            }
            //Move.
            if (_state.Stance is Stance.Stand or Stance.Crouch)
            {
                var speed = _state.Stance is Stance.Stand ? walkSpeed : crouchSpeed;
                var response = _state.Stance is Stance.Stand ? walkResponse : crouchResponse;

                var targetVelocity = groundedMovement * speed;
                currentVelocity = Vector3.Lerp
                    (
                        a: currentVelocity,
                        b: targetVelocity,
                        t: 1f - Mathf.Exp(-response * deltaTime)
                    );
            }
            //Continue sliding.
            else
            {
                //Friction
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                //Stop
                if (currentVelocity.magnitude < slideEndSpeed)
                {
                    _state.Stance = Stance.Crouch;
                }
            }
            
        }
        //Else in the air:
        else
        {
            //air Movement
            if(_requestedMovement.sqrMagnitude > 0)
            {
                var planarMovement = Vector3.ProjectOnPlane
                    (
                        vector: _requestedMovement,
                        planeNormal: motor.CharacterUp
                    ) * _requestedMovement.magnitude;

                var currentPlanarVelocity = Vector3.ProjectOnPlane
                    (
                        vector: currentVelocity,
                        planeNormal: motor.CharacterUp
                    );

                //Calculate movement force
                var movementForce = planarMovement * airAcceleration * deltaTime;

                var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                //Steer towards current velocity.
                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;
            }
            //Gravity
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (_requestedSustainedJump && verticalSpeed > 0f)
            {
                effectiveGravity *= jumpSustainGravity;
            }
            currentVelocity += deltaTime * effectiveGravity * motor.CharacterUp;
        }

        if (_requestedJump)
        {
            _requestedJump = false;
            _requestedCrouch = false;

            //Unstick from ground
            motor.ForceUnground(time: 0.1f);

            //set minimum vertical speed to jump speed
            var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
            //Add the difference
            currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
        }
    }

    public Transform GetCameraTarget() => cameraTarget;

    public void SetPosition(Vector3 positon, bool killVelocity = true)
    {
        motor.SetPosition(positon);
        if (killVelocity)
        {
            motor.BaseVelocity = Vector3.zero;
        }
    }
}
