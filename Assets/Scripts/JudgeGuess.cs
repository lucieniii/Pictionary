using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrawAndGuess.Guess
{
    public class JudgeGuess : MonoBehaviour
    {
        public Text judgement;

        public void SetJudgement(Text input)
        {
            if (input)
            {
                bool wordGenerated = WordGenerator.wordGenerated;
                string word = WordGenerator.word;
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
