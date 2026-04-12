using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {
    public KCCPlayerMovement character;

    void Update() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        character.SetInputs(new Vector2(x, z));
    }
}

