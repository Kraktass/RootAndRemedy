using UnityEngine;

public class devSettings : MonoBehaviour {
    void Awake() {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 144;
    }
}
