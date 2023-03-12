using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;
using Ubiq.Rooms;

namespace DrawAndGuess.Guess
{
    public class JudgeGuess : MonoBehaviour
    {
        public Text judgement;
        public Text inputText;

        public WordGenerator wordGenerator;

        public bool corrected;

        public int correctCount;
        public string[] correctNameRecord;
        public float[] correctTimeRecord;
        public int guesserNumber;
        public float roundStartTime;

        public RoomClient roomClient;
        private NetworkContext context;

        private void Start()
        {
            context = NetworkScene.Register(this);
            this.corrected = false;
            this.correctCount = 0;
            // Debug.Log(roomClient.Me["ubiq.samples.social.name"]);
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
            this.correctNameRecord[this.correctCount] = data.uuid;
            this.correctTimeRecord[this.correctCount] = data.correctTime;
            this.correctCount += 1;
        }

        public void SetJudgement(Text input)
        {
            if (input && !corrected)
            {
                bool wordGenerated = wordGenerator.wordGenerated;
                string word = wordGenerator.word;
                if (wordGenerated) 
                {
                    if (input.text.ToUpper() == word.ToUpper()) 
                    {
                        this.judgement.text = "CORRECT";
                        float ct = Time.time - this.roundStartTime;
                        context.SendJson(new Message(roomClient.Me["ubiq.samples.social.name"], ct));
                        this.correctNameRecord[this.correctCount] = roomClient.Me["ubiq.samples.social.name"];
                        this.correctTimeRecord[this.correctCount] = ct;
                        this.correctCount += 1;
                        corrected = true;
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
            this.inputText.text = "";
            this.correctCount = 0;
            this.corrected = false;
        }
    }
}
