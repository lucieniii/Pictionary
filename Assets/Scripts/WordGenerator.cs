using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrawAndGuess.Guess
{
    public class WordGenerator : MonoBehaviour
    {
        public static string word;
        public Text wordText;
        public static bool wordGenerated = false;

        public static string GetWord()
        {
            return word;
        }

        public static bool GetWordGenerated()
        {
            return wordGenerated;
        }

        public void GenerateWord()
        {
            if (!wordGenerated) 
            {
                word = "APPLE";
                wordGenerated = true;
            }
        }

        public void ShowWord()
        {
            this.wordText.text = word;
        }
    }
}