using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileCustomGravity : MonoBehaviour {
    [SerializeField] private float gravityMultiplier = 1f;

    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (rb == null || rb.isKinematic)
            return;

        Vector3 extraGravity = Physics.gravity * (gravityMultiplier - 1f);
        rb.AddForce(extraGravity, ForceMode.Acceleration);
    }

    public void SetGravityMultiplier(float value) {
        gravityMultiplier = Mathf.Max(0f, value);
    }
}
