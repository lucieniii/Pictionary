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
        public List<string> wordlist = new List<string>(){"apple","bike", "car","desk","computer"};

        public static string GetWord()
        {
            return word;
        }

        public static bool GetWordGenerated()
        {
            return wordGenerated;
        }

        public static int GetRandomSeed(int min = 0, int max = 10000)
        {
            return Random.Range(min, max);
        }

        public void GenerateWord()
        {
            if (!wordGenerated) 
            {
                int index = GetRandomSeed();
                index = index % wordlist.Count;
                Debug.Log(index);
                word = wordlist[index];
                wordGenerated = true;
            }
        }

        public void ShowWord()
        {
            this.wordText.text = word;
        }
    }
}