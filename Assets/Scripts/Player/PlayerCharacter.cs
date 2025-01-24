using UnityEngine;
using KinematicCharacterController;

public struct CharacterInput
{
    public Quaternion rotation;

    public Vector2 Move;
}


public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform cameraTarget;
    [Space]
    [SerializeField] private float walkSpeed = 20f;
    private Vector3 _requestedMovement;

    private Quaternion _requestedRotation;
    public void Initialize()
    {
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

        _requestedMovement = input.rotation * _requestedMovement;



    }
    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        //throw new System.NotImplementedException();

        return false;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        throw new System.NotImplementedException();
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        throw new System.NotImplementedException();
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        throw new System.NotImplementedException();
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

        var forward = Vector3.ProjectOnPlane
            (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
            );
        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = _requestedMovement;
    }

    public Transform GetCameraTarget() => cameraTarget;
}
