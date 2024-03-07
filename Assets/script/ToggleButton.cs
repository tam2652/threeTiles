using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    public GameObject panel;
    private bool isPanelActive = false;

    public void OnButtonClick()
    {
        isPanelActive = !isPanelActive;
        panel.SetActive(isPanelActive);
    }
}
