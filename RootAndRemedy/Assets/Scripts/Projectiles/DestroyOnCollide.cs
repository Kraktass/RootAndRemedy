using UnityEngine;

public class DestroyOnCollide : MonoBehaviour {

    [SerializeField] float destroyDelay = 0.05f;

    void OnCollisionEnter(Collision collision) {
        Destroy(gameObject, destroyDelay);
    }
}
