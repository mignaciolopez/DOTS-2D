using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    public int targetFrameRate = 30;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    public void Set(int target)
    {
        targetFrameRate = target;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}