using UnityEngine;

namespace Ubiq.Samples
{
    public class PanelSwitcher : MonoBehaviour
    {
        public GameObject defaultPanel;
        public GameObject titlePanel;


        private GameObject currentPanel;

        // Expected to be called by a UI element
        public void SwitchPanel (GameObject newPanel)
        {
            if (!currentPanel)
            {
                currentPanel = titlePanel;
            }

            if (currentPanel != newPanel)
            {
                currentPanel.SetActive(false);
            }

            newPanel.SetActive(true);
            currentPanel = newPanel;
        }

        public void SwitchPanelToDefault()
        {
            SwitchPanel(defaultPanel);
        }
    }
}
