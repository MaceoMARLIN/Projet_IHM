using UnityEngine;

public class LabelManager : MonoBehaviour
{
    public void ToggleCorrectColors(bool active)
    {
        DechetLabel[] labels = FindObjectsByType<DechetLabel>(FindObjectsSortMode.None);

        foreach (DechetLabel label in labels)
        {
            label.SetCorrectColor(active);
        }
    }
}