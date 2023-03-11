using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;

namespace DrawAndGuess.Guess
{
    public class JudgeGuess : MonoBehaviour
    {
        public Text judgement;

        public WordGenerator wordGenerator;

        public int correctCount = 0;
        public string[] correctUuidRecord;
        public float[] correctTimeRecord;
        public int playerNumber;

        private NetworkContext context;

        private void Start()
        {
            context = NetworkScene.Register(this);
        }

        private struct Message
        {
            public string uuid;
            public float correctTime;

            public Message(string uuid, float correctTime)
            {
                this.uuid = uuid;
                this.correctTime = correctTime;
            }
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            this.correctUuidRecord[this.correctCount] = data.uuid;
            this.correctTimeRecord[this.correctCount] = data.correctTime;
            this.correctCount += 1;
        }

        public void SetJudgement(Text input)
        {
            if (input)
            {
                bool wordGenerated = wordGenerator.wordGenerated;
                string word = wordGenerator.word;
                if (wordGenerated) 
                {
                    if (input.text.ToUpper() == word.ToUpper()) 
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

        public void reset()
        {
            this.judgement.text = "";
            this.correctCount = 0;
        }
    }
}
