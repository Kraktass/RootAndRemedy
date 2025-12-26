using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [Header("Tuning")]
    [SerializeField] float moveSpeed = 6f;     // units per second
    [SerializeField] float rotationSpeed = 10f; // smoothing factor (higher = snappier)

    Rigidbody rb;
    Vector2 moveInput;         // from Input System (WASD / left stick)
    Vector3 moveDir;           // world-space XZ

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Called by Player Input (Send Messages) for action named "Move"
    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate() {
        ProcessTranslation();
        ProcessRotation();
    }

    void ProcessTranslation() {
        // 2D input -> 3D XZ vector
        moveDir.x = moveInput.x;
        moveDir.y = 0f;
        moveDir.z = moveInput.y;

        // keep diagonal speed consistent
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    void ProcessRotation() {
        // face movement direction only when moving (Diablo-style)
        if (moveDir.sqrMagnitude <= 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime));
    }
}
