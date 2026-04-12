using System.Collections;
using UnityEngine;

public class PlantSeed : MonoBehaviour, IInteractable {

    [SerializeField] PlanterState state = PlanterState.Empty;
    [SerializeField] Color highlightEmission = Color.green;
    [SerializeField] float highlightIntensity = 0.5f;
    [SerializeField] Inventory inventory;

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
    }



    public void Interact() {
        switch (state) {
            case PlanterState.Empty:
                ItemStack stack = inventory.GetSelectedItemStack();
                if (stack == null || stack.item == null || stack.item.itemUseType != ItemUseType.Plantable) break;
                seed = stack.item;
                inventory.ConsumeSelectedItem();
                UnHighlight();
                Debug.Log("You have planted a seed");
                state = PlanterState.Growing;
                StartCoroutine(GrowthTimer());
                break;
            case PlanterState.Growing:
                Debug.Log("Still growing...");
                break;
            case PlanterState.Harvestable:
                state = PlanterState.Empty;
                inventory.AddItem(seed.harvestResult, seed.harvestAmount);
                Debug.Log("You harvested a plant");
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
        return state != PlanterState.Growing;
    }

    private IEnumerator GrowthTimer() {
        yield return new WaitForSeconds(seed.growthTime);
        state = PlanterState.Harvestable;
        Debug.Log("A planter is now harvestable");
    }
}
