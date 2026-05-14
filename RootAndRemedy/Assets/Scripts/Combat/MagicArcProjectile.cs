using UnityEngine;

public class MagicArcProjectile : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float duration = 0.45f;
    [SerializeField] private float arcHeight = 3f;

    [Header("Rotation")]
    [SerializeField] private bool rotateAlongPath = true;
    [SerializeField] private float rotationFollowSpeed = 12f;

    [Header("Potion Spin")]
    [SerializeField] private bool addSpin = true;
    [SerializeField] private Vector3 spinAxis = new Vector3(1f, 0.25f, 0f);
    [SerializeField] private float spinSpeed = 360f;

    private Vector3 startPoint;
    private Vector3 targetPoint;
    private float elapsed;
    private bool launched;

    private Quaternion spinRotation = Quaternion.identity;

    public void Launch(Vector3 start, Vector3 target, float travelTime, float height) {
        startPoint = start;
        targetPoint = target;
        duration = Mathf.Max(0.01f, travelTime);
        arcHeight = height;
        elapsed = 0f;
        launched = true;

        spinRotation = Quaternion.identity;
        transform.position = startPoint;
    }

    private void Update() {
        if (!launched)
            return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        Vector3 flatPosition = Vector3.Lerp(startPoint, targetPoint, t);
        float arc = arcHeight * 4f * t * (1f - t);
        Vector3 newPosition = flatPosition + Vector3.up * arc;

        Vector3 moveDir = newPosition - transform.position;

        Quaternion pathRotation = transform.rotation;

        if (rotateAlongPath && moveDir.sqrMagnitude > 0.0001f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);
            pathRotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationFollowSpeed * Time.deltaTime
            );
        }

        if (addSpin) {
            Vector3 axis = spinAxis.sqrMagnitude > 0.001f ? spinAxis.normalized : Vector3.right;
            spinRotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, axis);
            transform.rotation = pathRotation * spinRotation;
        }
        else {
            transform.rotation = pathRotation;
        }

        transform.position = newPosition;

        if (t >= 1f) {
            OnArrival();
        }
    }

    private void OnArrival() {
        Destroy(gameObject);
    }
}