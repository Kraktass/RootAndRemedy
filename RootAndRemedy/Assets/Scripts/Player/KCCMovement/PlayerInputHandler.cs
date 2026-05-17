using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    [SerializeField] private KCCPlayerMovement character;

    public void OnMove(InputValue value) {
        Vector2 input = value.Get<Vector2>();
        character.SetInputs(input);
    }
}
