using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeGuess : MonoBehaviour
{
    public Text judgement;

    public void SetJudgement(Text input)
    {
        if (input)
        {
            if (input.text.ToUpper() == "APPLE") 
            {
                this.judgement.text = "CORRECT";
            }
            else 
            {
                this.judgement.text = "WRONG";
                input.text = "";
            }
            
        }
    }
}