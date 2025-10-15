using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 10f;

    [Header("Aim")]
    [SerializeField] float rightStickDeadzone = 0.2f; // prefer RS when above this
    [SerializeField] LayerMask groundMask;            // set to your Ground layer

    private Rigidbody rb;
    private Vector2 moveInput;   // WASD / Left Stick
    private Vector2 lookInput;   // Mouse position OR Right Stick
    private Vector3 moveDir;     // world-space XZ

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // === Input System (Player Input set to Send Messages) ===
    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }
    public void OnLook(InputValue value) { lookInput = value.Get<Vector2>(); }

    void FixedUpdate() {
        ProcessTranslation();
        ProcessRotation();
    }

    void ProcessTranslation() {
        // world-space XZ from input
        moveDir.x = moveInput.x;
        moveDir.y = 0f;
        moveDir.z = moveInput.y;

        // Prevent diagonal speed boost
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    void ProcessRotation() {
        Vector3 aimDir;

        // 1) Prefer gamepad right stick if actuated
        if (lookInput.sqrMagnitude >= rightStickDeadzone * rightStickDeadzone && Gamepad.current != null) {
            aimDir = new Vector3(lookInput.x, 0f, lookInput.y);
            if (aimDir.sqrMagnitude > 1f) aimDir.Normalize();
        }
        // 2) Otherwise try mouse cursor world aim
        else if (Mouse.current != null && TryGetMouseAimDirection(out aimDir)) {
            // aimDir already normalized by TryGetMouseAimDirection
        }
        // 3) Fallback: face movement direction (Diablo-style)
        else if (moveDir.sqrMagnitude > 0.0001f) {
            aimDir = moveDir;
        }
        else {
            return; // nothing to face
        }

        Quaternion targetRot = Quaternion.LookRotation(aimDir, Vector3.up);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
    }

    bool TryGetMouseAimDirection(out Vector3 dir) {
        dir = Vector3.zero;
        var cam = Camera.main;
        if (cam == null || Mouse.current == null) return false;

        // Cast a ray from mouse to ground
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask, QueryTriggerInteraction.Ignore)) {
            Vector3 to = hit.point - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f) {
                dir = to.normalized;
                return true;
            }
        }
        return false;
    }
}
