using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using Ubiq.Samples;

namespace DrawAndGuess.Procedure
{
    public class GameController : MonoBehaviour
    {
        private NetworkContext context;
        public static bool isGameOwner;
        private bool hasGameOwner;
        public PanelSwitcher mainPanel;

        public GameObject startGamePanel;
        public GameObject othersPanel;
        public GameObject gameOwnerPanel;

        private struct Message
        {
            public bool haveOwner;

            public Message(bool haveOwner)
            {
                this.haveOwner = haveOwner;
            }
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
            isGameOwner = false;
            this.hasGameOwner = false;
        }

        public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            if (data.haveOwner)
            {
                mainPanel.SwitchPanel(this.othersPanel);
                isGameOwner = false;
                this.hasGameOwner = true;
            }
            else
            {
                isGameOwner = false;
                this.hasGameOwner = false;
                mainPanel.SwitchPanel(this.startGamePanel);
            }
            
        }

        public void PressStartButtom()
        {
            if (!isGameOwner && !this.hasGameOwner) 
            {
                context.SendJson(new Message(true));
                mainPanel.SwitchPanel(this.gameOwnerPanel);
                isGameOwner = true;
                this.hasGameOwner = true;
            }
        }

        // Call by game owner's panel
        public void PressEndButtom()
        {
            if (isGameOwner && this.hasGameOwner) 
            {
                isGameOwner = false;
                this.hasGameOwner = false;
                mainPanel.SwitchPanel(this.gameOwnerPanel);
                context.SendJson(new Message(this.hasGameOwner));
            }
        }
    }
}