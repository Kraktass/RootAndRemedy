using System.Collections;
using UnityEngine;

public class PlantSeed : MonoBehaviour, IInteractable {

    [SerializeField] PlanterState state = PlanterState.Empty;
    [SerializeField] Color highlightEmission = Color.green;
    [SerializeField] float highlightIntensity = 0.5f;

    Renderer _renderer;
    Material _materialInstance;
    Color _originalEmissionColor;
    bool _emissionWasEnabled;
    ItemSO seed;

    void Awake() {
        _renderer = GetComponent<MeshRenderer>();
        _materialInstance = _renderer.material;
        _originalEmissionColor = _materialInstance.GetColor("_EmissionColor");
        _emissionWasEnabled = _materialInstance.IsKeywordEnabled("_EMISSION");
        Debug.Log(_originalEmissionColor);
    }

    public void Interact() {
        switch (state) {
            case PlanterState.Empty:
                Debug.Log("You have planted a seed");
                state = PlanterState.Growing;
                break;
            case PlanterState.Growing:
                GrowthTimer();
                Debug.Log("Still growing...");
                break;
            case PlanterState.Harvestable:
                state = PlanterState.Empty;
                break;
        }
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
        return state == PlanterState.Empty || state == PlanterState.Harvestable;
    }

    private IEnumerator GrowthTimer() {
        yield return new WaitForSeconds(seed.growthTime);
        state = PlanterState.Harvestable;
    }
}
