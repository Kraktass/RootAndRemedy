using UnityEngine;

public class PotionExplosion : MonoBehaviour {

    public void Explosion(Vector3 explosionPosition) {
        GameObject explosionObject = new GameObject("Potion Explosion");

        explosionObject.transform.position = explosionPosition;

        SphereCollider sphereCollider =
            explosionObject.AddComponent<SphereCollider>();

        sphereCollider.radius = 5f;
        sphereCollider.isTrigger = true;

        Debug.Log("Explosion created");
    }

}
