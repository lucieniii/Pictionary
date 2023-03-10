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
        public static int memberCount;
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
                memberCount += 1;
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

        public void PressStartButton()
        {
            if (!isGameOwner && !this.hasGameOwner) 
            {
                memberCount = 1;
                context.SendJson(new Message(true));
                mainPanel.SwitchPanel(this.gameOwnerPanel);
                isGameOwner = true;
                this.hasGameOwner = true;
            }
            Debug.Log(memberCount);
        }

        // Call by game owner's panel
        public void PressEndButton()
        {
            if (isGameOwner && this.hasGameOwner) 
            {
                memberCount = 0;
                isGameOwner = false;
                this.hasGameOwner = false;
                mainPanel.SwitchPanel(this.startGamePanel);
                context.SendJson(new Message(this.hasGameOwner));
            }
        }
    }
}