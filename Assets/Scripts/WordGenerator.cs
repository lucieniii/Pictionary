using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;

namespace DrawAndGuess.Guess
{
    public class WordGenerator : MonoBehaviour
    {
        public string word;
        public Text wordText;
        public bool wordGenerated = false;
        
        private NetworkContext context;
        // = new List<string>();
        //List<string> wordlist;

        private void Start()
        {
            context = NetworkScene.Register(this);
        }

        private struct Message
        {
            public string word;
            public bool wordGenerated;

            public Message(string word, bool wordGenerated)
            {
                this.word = word;
                this.wordGenerated = wordGenerated;
            }
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            this.word = data.word;
            this.wordGenerated = data.wordGenerated;
            this.ShowWord();
            Debug.Log("Receive word");
        }

        public string GetWord()
        {
            return this.word;
        }

        private class WordBox{
            public List<string> wordlist;
        }

        public bool GetWordGenerated()
        {
            return wordGenerated;
        }

        public static int GetRandomSeed(int min = 0, int max = 10000)
        {
            return Random.Range(min, max);
        }

        public void GenerateWord()
        {   
            if (!this.wordGenerated) 
            {
                // WordBox wordbox = LoadJson.LoadJsonFromFile<WordBox>();
                // int index = GetRandomSeed();
                // Debug.Log(wordbox.wordlist.Count);
                // Debug.Log(wordbox.wordlist);
                // index = index % wordbox.wordlist.Count;               
                // word = wordbox.wordlist[index];
                // wordGenerated = true;
                Debug.Log("here");
                List<string> wordlist = new List<string>(){"apple","bike","car","desk","computer","horse","house","chair","bottle","glass","skateboard","rainbow","pen"};
                int index = GetRandomSeed();
                Debug.Log(wordlist.Count);
                Debug.Log(wordlist);
                index = index % wordlist.Count;
                
                this.word = wordlist[index];
                this.wordGenerated = true;

                this.context.SendJson(new Message(this.word, this.wordGenerated));
            }
        }

        public void ShowWord()
        {
            this.wordText.text = this.word;
        }

        public void reset()
        {
            this.word = "";
            this.wordText.text = "";
            this.wordGenerated = false;
        }
    }
}