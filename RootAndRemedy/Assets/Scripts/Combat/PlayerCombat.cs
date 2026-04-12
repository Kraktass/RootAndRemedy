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
    [SerializeField] private Transform throwSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float throwRotation = 10f;

    [Header("Arc Shape")]
    [SerializeField] private float arcHeight = 3.5f;

    [Header("Arc Speed")]
    [SerializeField] private float throwSpeedMultiplier = 1.5f;

    [Header("Arc Preview")]
    [SerializeField] private LineRenderer arcLine;
    [SerializeField] private int arcPoints = 30;
    [SerializeField] private float arcTimeStep = 0.05f;
    [SerializeField] private LayerMask arcCollisionMask;

    [Header("Refs")]
    [SerializeField] private Inventory inventory;

    private bool isAiming;
    private Vector3 lastAimPoint;

    private void Awake() {
        if (!cam) cam = Camera.main;

        if (aimReticle)
            aimReticle.gameObject.SetActive(false);

        if (arcLine) {
            arcLine.positionCount = 0;
            arcLine.enabled = false;
        }
    }

    // Input System (Send Messages)
    public void OnThrow(InputValue value) {
        if (inventory == null) {
            Debug.LogError("Inventory not assigned!");
            return;
        }

        ItemStack stack = inventory.GetSelectedItemStack();

        if (stack == null || stack.item == null)
            return;

        if (stack.item.itemUseType != ItemUseType.Throwable)
            return;

        if (stack.amount <= 0)
            return;

        bool pressed = value.isPressed;

        if (pressed) {
            if (!isAiming) {
                StartAiming();
            }
        }
        else {
            if (isAiming) {
                StopAiming();
                inventory.ConsumeSelectedItem();
            }
        }
    }

    private void StartAiming() {
        isAiming = true;

        if (aimReticle)
            aimReticle.gameObject.SetActive(true);

        if (arcLine)
            arcLine.enabled = true;
    }

    private void StopAiming() {
        FireProjectile();

        isAiming = false;

        if (aimReticle)
            aimReticle.gameObject.SetActive(false);

        if (arcLine) {
            arcLine.positionCount = 0;
            arcLine.enabled = false;
        }
    }

    private void FixedUpdate() {
        if (!isAiming || !cam)
            return;

        Vector2 screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        if (Physics.Raycast(cam.ScreenPointToRay(screenPos), out RaycastHit hit, 500f, groundMask, QueryTriggerInteraction.Ignore)) {
            lastAimPoint = hit.point;

            if (aimReticle) {
                aimReticle.position = hit.point + Vector3.up * hoverHeight;

                if (alignToSurface)
                    aimReticle.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }

            // Temporary direct rotation.
            // Later, pass this direction into your KCC movement script instead.
            Vector3 look = hit.point;
            look.y = transform.position.y;

            Vector3 dir = look - transform.position;
            if (dir.sqrMagnitude > 0.001f) {
                Quaternion target = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, target, turnSpeed * Time.fixedDeltaTime);
            }
        }

        if (arcLine && throwSpawn) {
            Vector3 vel = ComputeInitialVelocity();
            DrawArc(throwSpawn.position, vel);
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer) {
        obj.layer = layer;

        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void FireProjectile() {
        if (!projectilePrefab) {
            Debug.LogError("Projectile prefab missing!");
            return;
        }

        if (!throwSpawn) {
            Debug.LogError("Throw spawn missing!");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, throwSpawn.position, Quaternion.identity);

        int projectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        if (projectileLayer != -1) {
            SetLayerRecursively(proj, projectileLayer);
        }
        else {
            Debug.LogWarning("Layer 'PlayerProjectile' does not exist.");
        }

        if (!proj.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb = proj.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Vector3 velocity = ComputeInitialVelocity();
        rb.linearVelocity = velocity;

        float gravityMultiplier = throwSpeedMultiplier * throwSpeedMultiplier;

        ProjectileCustomGravity customGravity = proj.GetComponent<ProjectileCustomGravity>();
        if (customGravity == null) {
            customGravity = proj.AddComponent<ProjectileCustomGravity>();
        }

        customGravity.SetGravityMultiplier(gravityMultiplier);

        if (throwRotation != 0f) {
            rb.AddTorque(Random.onUnitSphere * throwRotation, ForceMode.Impulse);
        }
    }

    private Vector3 ComputeInitialVelocity() {
        Vector3 start = throwSpawn.position;
        Vector3 target = lastAimPoint;
        Vector3 gravity = Physics.gravity;
        float g = Mathf.Abs(gravity.y);

        float apexY = Mathf.Max(start.y, target.y) + arcHeight;
        float rise = Mathf.Max(0.01f, apexY - start.y);
        float fall = Mathf.Max(0.01f, apexY - target.y);

        float timeUp = Mathf.Sqrt(2f * rise / g);
        float timeDown = Mathf.Sqrt(2f * fall / g);
        float totalTime = timeUp + timeDown;

        Vector3 deltaXZ = new Vector3(target.x - start.x, 0f, target.z - start.z);
        Vector3 horizontalVelocity = deltaXZ / totalTime;
        float verticalVelocity = g * timeUp;

        Vector3 baseVelocity = horizontalVelocity + Vector3.up * verticalVelocity;

        return baseVelocity * throwSpeedMultiplier;
    }

    private void DrawArc(Vector3 startPos, Vector3 startVel) {
        if (!arcLine)
            return;

        arcLine.positionCount = arcPoints;

        Vector3 pos = startPos;
        Vector3 vel = startVel;

        for (int i = 0; i < arcPoints; i++) {
            arcLine.SetPosition(i, pos);

            Vector3 step = vel * arcTimeStep + 0.5f * Physics.gravity * (throwSpeedMultiplier * throwSpeedMultiplier) * (arcTimeStep * arcTimeStep);

            if (Physics.Raycast(pos, step.normalized, out RaycastHit hit, step.magnitude, arcCollisionMask, QueryTriggerInteraction.Ignore)) {
                arcLine.positionCount = i + 1;
                arcLine.SetPosition(i, hit.point);
                return;
            }

            pos += step;
            vel += Physics.gravity * (throwSpeedMultiplier * throwSpeedMultiplier) * arcTimeStep;
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (arcPoints < 2) arcPoints = 2;
        if (arcTimeStep <= 0f) arcTimeStep = 0.01f;
        if (arcHeight < 0.1f) arcHeight = 0.1f;
        if (throwSpeedMultiplier < 0.1f) throwSpeedMultiplier = 0.1f;
    }
#endif
}