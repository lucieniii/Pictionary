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
        // = new List<string>();
        //List<string> wordlist;

        public static string GetWord()
        {
            return word;
        }

        private class WordBox{
            public List<string> wordlist;
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
                WordBox wordbox = LoadJson.LoadJsonFromFile<WordBox>();
                int index = GetRandomSeed();
                Debug.Log(wordbox.wordlist.Count);
                Debug.Log(wordbox.wordlist);
                index = index % wordbox.wordlist.Count;
                
                word = wordbox.wordlist[index];
                wordGenerated = true;
            }
        }

        public void ShowWord()
        {
            this.wordText.text = word;
        }
    }
}