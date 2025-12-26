using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject {

    [Header("Identity")]
    public string itemId;
    public string displayName;

    [Header("UI")]
    public Sprite icon;

    [Header("Stacking")]
    [Min(1)] public int maxStackSize = 64;
}
