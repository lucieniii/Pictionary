using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ubiq.Samples.Social
{
    public class SetWordButton : MonoBehaviour
    {
        public NameManager wordManager;
        public Text wordText;

        // Expected to be called by a UI element    
        public void SetName()
        {
            if (wordText && wordManager)
            {
                wordManager.SetName(wordText.text);
            }
        }
    }
}