using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using Ubiq.Samples;
using Ubiq.Rooms;

namespace DrawAndGuess.Procedure
{
    public class GameController : MonoBehaviour
    {
        public enum GameStatus
        {
            GameStartPhase,
            RoundStartPhase,
            RoundPickWordPhase,
            RoundPlayPhase,
            RoundEndPhase,
            GameEndPhase,
        };

        public GameStatus previousGameStatus;
        public GameStatus currentGameStatus;

        public RoomClient roomClient;

        private NetworkContext context;
        public bool isGameOwner;
        public int playerCount = 0;
        public PanelSwitcher mainPanel;

        public GameObject startGamePanel;
        public GameObject othersPanel;
        public GameObject gameOwnerPanel;
        public GameObject rankPanel;

        private struct Message
        {
            public GameStatus previousGameStatus;
            public GameStatus nextGameStatus;

            public Message(GameStatus previousGameStatus, GameStatus nextGameStatus)
            {
                this.previousGameStatus = previousGameStatus;
                this.nextGameStatus = nextGameStatus;
            }
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
            previousGameStatus = GameStatus.GameStartPhase;
            currentGameStatus = GameStatus.GameStartPhase;
            this.isGameOwner = false;
        }

        public void CountPlayerNumber()
        {
            //Debug.Log(roomClient.Peers);
            this.playerCount = 1;
            foreach (var peer in roomClient.Peers)
            {
                this.playerCount += 1;
            }
            //Debug.Log(cnt);
        }

        public void ChangeGameStatus(GameStatus previousGameStatus, GameStatus nextGameStatus)
        {
            this.previousGameStatus = previousGameStatus;
            this.currentGameStatus = nextGameStatus;
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            if (data.previousGameStatus == GameStatus.GameStartPhase)
            {
                this.CountPlayerNumber();
                if (data.nextGameStatus == GameStatus.RoundStartPhase)
                {
                    mainPanel.SwitchPanel(this.othersPanel);
                    this.isGameOwner = false;
                }
            } 
            else if (data.previousGameStatus == GameStatus.RoundStartPhase)
            {
                if (data.nextGameStatus == GameStatus.GameEndPhase)
                {
                    this.isGameOwner = false;
                    mainPanel.SwitchPanel(this.rankPanel);
                }
            }
            else if (data.previousGameStatus == GameStatus.GameEndPhase)
            {
                if (data.nextGameStatus == GameStatus.GameStartPhase)
                {
                    mainPanel.SwitchPanel(this.startGamePanel);
                }
            }
            this.ChangeGameStatus(data.previousGameStatus, data.nextGameStatus);
            Debug.Log("Receive Message");
        }  

        public void PressStartButton()
        {
            if (!this.isGameOwner && currentGameStatus == GameStatus.GameStartPhase)
            {
                this.ChangeGameStatus(GameStatus.GameStartPhase, GameStatus.RoundStartPhase);
                context.SendJson(new Message(GameStatus.GameStartPhase, GameStatus.RoundStartPhase));
                mainPanel.SwitchPanel(this.gameOwnerPanel);
                this.isGameOwner = true;
                this.CountPlayerNumber();
            }
        }

        public void PressEndButton()
        {
            if (this.isGameOwner && currentGameStatus == GameStatus.RoundStartPhase) 
            {
                this.isGameOwner = false;
                mainPanel.SwitchPanel(this.startGamePanel);
                this.ChangeGameStatus(GameStatus.RoundStartPhase, GameStatus.GameEndPhase);
                context.SendJson(new Message(GameStatus.RoundStartPhase, GameStatus.GameEndPhase));
            }
        }

        public void PressRankOKButton()
        {
            if (this.currentGameStatus == GameStatus.GameEndPhase)
            {
                mainPanel.SwitchPanel(this.startGamePanel);
                context.SendJson(new Message(GameStatus.GameEndPhase, GameStatus.GameStartPhase));
            }
        }
    }
}