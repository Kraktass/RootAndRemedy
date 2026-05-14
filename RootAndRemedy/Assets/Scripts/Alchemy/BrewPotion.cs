using UnityEngine;

public class BrewPotion : MonoBehaviour, IInteractable {

    [SerializeField] Color highlightEmission = Color.green;
    [SerializeField] float highlightIntensity = 0.5f;

    Renderer _renderer;
    Material _materialInstance;
    Color _originalEmissionColor;
    bool _emissionWasEnabled;

    void Awake() {
        _renderer = GetComponent<MeshRenderer>();
        _materialInstance = _renderer.material;
        _originalEmissionColor = _materialInstance.GetColor("_EmissionColor");
        _emissionWasEnabled = _materialInstance.IsKeywordEnabled("_EMISSION");
    }

    public void Interact() {
        Debug.Log("Interacted with:" + this.gameObject.name);
    }

    public void Highlight() {
        _materialInstance.EnableKeyword("_EMISSION");
        _materialInstance.SetColor("_EmissionColor", highlightEmission * highlightIntensity);
    }

    public void UnHighlight() {
        _materialInstance.SetColor("_EmissionColor", _originalEmissionColor);
        if (_emissionWasEnabled) {
            _materialInstance.EnableKeyword("_EMISSION");
        }
        else {
            _materialInstance.DisableKeyword("_EMISSION");
        }
    }

    public bool CanInteract() {
        return true;
    }

    public void UseAlchemyTable() {

    }

}
