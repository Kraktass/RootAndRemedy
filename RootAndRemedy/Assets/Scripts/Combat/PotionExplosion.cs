using UnityEngine;

public class PotionExplosion : MonoBehaviour {
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float radius = 5f;

    private void Start() {
        sphereCollider.radius = radius;
        sphereCollider.isTrigger = true;
        Destroy(gameObject, 5f);
        Debug.Log("Explosion created");
    }
}
