using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrawAndGuess.Guess
{
    public class JudgeGuess : MonoBehaviour
    {
        public Text judgement;

        public WordGenerator wordGenerator;

        public void SetJudgement(Text input)
        {
            Debug.Log(wordGenerator.word);
            Debug.Log(wordGenerator.wordGenerated);
            if (input)
            {
                bool wordGenerated = wordGenerator.wordGenerated;
                string word = wordGenerator.word;
                if (wordGenerated) 
                {
                    if (input.text.ToUpper() == word) 
                    {
                        this.judgement.text = "CORRECT";
                    }
                    else 
                    {
                        this.judgement.text = "WRONG";
                        input.text = "";
                    }
                }
                else
                {
                    this.judgement.text = "PLEASE WAIT";
                }
            }
        }
    }
}
