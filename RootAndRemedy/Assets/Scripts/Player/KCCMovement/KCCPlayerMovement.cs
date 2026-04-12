using UnityEngine;
using KinematicCharacterController;

public class KCCPlayerMovement : MonoBehaviour, ICharacterController {
    public KinematicCharacterMotor Motor;

    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;

    private Vector3 moveInput;

    private void Awake() {
        Motor.CharacterController = this;
    }

    public void SetInputs(Vector2 input) {
        moveInput = new Vector3(input.x, 0f, input.y);
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
        if (moveInput.sqrMagnitude > 0f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, deltaTime * rotationSpeed);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
        Vector3 targetVelocity = moveInput * moveSpeed;

        if (Motor.GroundingStatus.IsStableOnGround) {
            currentVelocity = targetVelocity;
        }
        else {
            currentVelocity += Vector3.up * gravity * deltaTime;
        }
    }

    public void BeforeCharacterUpdate(float deltaTime) { }
    public void AfterCharacterUpdate(float deltaTime) { }
    public bool IsColliderValidForCollisions(Collider coll) => true;
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void PostGroundingUpdate(float deltaTime) { }
    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void ProcessHitStabilityReport(
        Collider hitCollider,
        Vector3 hitNormal,
        Vector3 hitPoint,
        Vector3 atCharacterPosition,
        Quaternion atCharacterRotation,
        ref HitStabilityReport hitStabilityReport) {
    }
}
