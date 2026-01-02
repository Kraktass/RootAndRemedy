using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [Header("Tuning")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 10f;

    Rigidbody rb;
    Vector2 moveInput;
    Vector3 moveDir;

    public GameObject container;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
        if (container.activeInHierarchy) {
            moveInput = Vector2.zero;
        }
    }

    void FixedUpdate() {
        ProcessTranslation();
        ProcessRotation();
    }

    void ProcessTranslation() {
        moveDir.x = moveInput.x;
        moveDir.y = 0f;
        moveDir.z = moveInput.y;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    void ProcessRotation() {
        if (moveDir.sqrMagnitude <= 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime));
    }


}
