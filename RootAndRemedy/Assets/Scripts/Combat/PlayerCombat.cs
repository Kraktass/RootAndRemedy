using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour {
    [Header("Aim Reticle (world)")]
    [SerializeField] private Transform aimReticle;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Camera cam;
    [SerializeField] private float hoverHeight = 0.02f;
    [SerializeField] private bool alignToSurface = true;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Throw Settings")]
    [SerializeField] private Transform throwSpawn;          // spawn point for projectile
    [SerializeField] private GameObject projectilePrefab;   // prefab with Rigidbody
    [SerializeField, Tooltip("Random spin torque applied to projectile")]
    private float throwRotation = 10f;

    [Header("Ballistic (lands on reticle)")]
    [SerializeField, Tooltip("Seconds of airtime per meter of horizontal distance")]
    private float timePerMeter = 0.06f;            // 0.05–0.08 feels good
    [SerializeField, Tooltip("Minimum total flight time (seconds)")]
    private float minFlightTime = 0.30f;           // prevents snapping close up
    [SerializeField, Tooltip("Maximum total flight time (seconds)")]
    private float maxFlightTime = 1.20f;           // caps floaty long shots
    [SerializeField, Tooltip("Extra upward bias added to computed velocity")]
    private float extraUpBias = 0.0f;

    [Header("Arc Preview")]
    [SerializeField] private LineRenderer arcLine;          // assign in inspector
    [SerializeField, Tooltip("Number of points used for the arc line")]
    private int arcPoints = 30;
    [SerializeField, Tooltip("Time step between arc samples (s)")]
    private float arcTimeStep = 0.05f;
    [SerializeField, Tooltip("Layers the arc should stop at when previewing")]
    private LayerMask arcCollisionMask;

    private bool isAiming;
    private Vector3 lastAimPoint;

    void Awake() {
        if (!cam) cam = Camera.main;
        if (aimReticle) aimReticle.gameObject.SetActive(false);
        if (arcLine) { arcLine.positionCount = 0; arcLine.enabled = false; }
    }

    // Called by Player Input (Send Messages) for the "Throw" action
    public void OnThrow(InputValue value) {
        bool pressed = value.isPressed;

        if (pressed) {
            isAiming = true;
            if (aimReticle) aimReticle.gameObject.SetActive(true);
            if (arcLine) arcLine.enabled = true;
        }
        else {
            if (isAiming)
                FireProjectile();

            isAiming = false;
            if (aimReticle) aimReticle.gameObject.SetActive(false);
            if (arcLine) { arcLine.positionCount = 0; arcLine.enabled = false; }
        }
    }

    void FixedUpdate() {
        if (!cam) return;

        if (isAiming) {
            // Update reticle position
            Vector2 screenPos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : (Vector2)Input.mousePosition;

            if (Physics.Raycast(cam.ScreenPointToRay(screenPos),
                                out var hit, 500f, groundMask, QueryTriggerInteraction.Ignore)) {
                lastAimPoint = hit.point;

                if (aimReticle) {
                    aimReticle.position = hit.point + Vector3.up * hoverHeight;
                    if (alignToSurface)
                        aimReticle.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }

                // Rotate player toward aim point (Y only)
                Vector3 look = hit.point; look.y = transform.position.y;
                Vector3 dir = look - transform.position;
                if (dir.sqrMagnitude > 0.001f) {
                    Quaternion target = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation, target, turnSpeed * Time.fixedDeltaTime);
                }
            }

            // Update arc preview
            if (arcLine && throwSpawn) {
                Vector3 initVel = ComputeInitialVelocity();
                DrawArc(throwSpawn.position, initVel);
            }
        }
    }

    private void FireProjectile() {
        if (!projectilePrefab || !throwSpawn) return;

        GameObject proj = Instantiate(projectilePrefab, throwSpawn.position, throwSpawn.rotation);

        if (!proj.TryGetComponent<Rigidbody>(out var rb)) {
            rb = proj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        Vector3 initialVelocity = ComputeInitialVelocity();
        rb.linearVelocity = initialVelocity;
        if (throwRotation != 0f)
            rb.AddTorque(Random.onUnitSphere * throwRotation, ForceMode.Impulse);
    }

    // ---------------------------------------------------------------------
    // ComputeInitialVelocity: solves for velocity that lands on lastAimPoint
    // ---------------------------------------------------------------------
    private Vector3 ComputeInitialVelocity() {
        Vector3 start = throwSpawn.position;
        Vector3 target = lastAimPoint;
        Vector3 delta = target - start;

        // Horizontal distance
        Vector3 deltaXZ = new Vector3(delta.x, 0f, delta.z);
        float horizDist = deltaXZ.magnitude;

        // Choose a flight time based on distance
        float t = Mathf.Clamp(horizDist * timePerMeter, minFlightTime, maxFlightTime);
        if (t < 1e-3f) t = minFlightTime;

        // Solve for initial velocity
        Vector3 gravity = Physics.gravity;
        Vector3 v = (delta - 0.5f * gravity * (t * t)) / t;

        if (extraUpBias != 0f) v += Vector3.up * extraUpBias;
        return v;
    }

    // Draws a parabolic arc using physics integration
    private void DrawArc(Vector3 startPos, Vector3 startVel) {
        if (!arcLine) return;

        arcLine.positionCount = arcPoints;

        Vector3 pos = startPos;
        Vector3 vel = startVel;

        for (int i = 0; i < arcPoints; i++) {
            arcLine.SetPosition(i, pos);

            Vector3 step = vel * arcTimeStep + 0.5f * Physics.gravity * (arcTimeStep * arcTimeStep);
            if (Physics.Raycast(pos, step.normalized, out var hit, step.magnitude, arcCollisionMask, QueryTriggerInteraction.Ignore)) {
                arcLine.positionCount = i + 1;
                arcLine.SetPosition(i, hit.point);
                return; // stop at collision
            }

            pos += step;
            vel += Physics.gravity * arcTimeStep;
        }
    }

#if UNITY_EDITOR
    void OnValidate() {
        if (arcPoints < 2) arcPoints = 2;
        if (arcTimeStep <= 0f) arcTimeStep = 0.01f;
        if (minFlightTime <= 0f) minFlightTime = 0.1f;
        if (maxFlightTime < minFlightTime) maxFlightTime = minFlightTime;
    }
#endif
}
