using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using Ubiq.Samples;
using Ubiq.Rooms;
using DrawAndGuess.Guess;

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

        public float roundStartTime;

        public RoomClient roomClient;

        private NetworkContext context;

        public bool isGameOwner;
        public int playerNumber;
        public string[] playerUuids;
        
        public int artistNumber = 1; // Hard coded
        public float roundDuration; // Hard coded (second)
        public string[] artistUuids;

        public PanelSwitcher mainPanel;

        public WordGenerator wordGenerator;
        public JudgeGuess judgeGuess;

        public GameObject startGamePanel;
        public GameObject othersPanel;
        public GameObject gameOwnerPanel;
        public GameObject rankPanel;
        public GameObject artistPanel;
        public GameObject guessPanel;

        // private int standByCount = 0;

        private struct Message
        {
            public GameStatus previousGameStatus;
            public GameStatus nextGameStatus;
            public string[] artistUuids;

            public Message(GameStatus previousGameStatus, GameStatus nextGameStatus, string[] artistUuids)
            {
                this.previousGameStatus = previousGameStatus;
                this.nextGameStatus = nextGameStatus;
                this.artistUuids = artistUuids;
            }
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
            previousGameStatus = GameStatus.GameStartPhase;
            currentGameStatus = GameStatus.GameStartPhase;
            this.isGameOwner = false;
            this.roundDuration = 30.0f;
        }

        public void CountPlayerNumber()
        {
            this.playerNumber = 1;
            foreach (var peer in roomClient.Peers)
            {
                this.playerNumber += 1;
            }
            this.playerUuids = new string [this.playerNumber];
            this.playerUuids[0] = roomClient.Me.uuid;
            int i = 1;
            foreach (var peer in roomClient.Peers)
            {
                this.playerUuids[i] = peer.uuid;
                i += 1;
            }
            // Debug.Log(this.playerUuids);
            this.judgeGuess.guesserNumber = this.playerNumber - this.artistNumber;
            this.judgeGuess.correctTimeRecord = new float[this.judgeGuess.guesserNumber];
            this.judgeGuess.correctUuidRecord = new string[this.judgeGuess.guesserNumber];
        }

        public void ChangeGameStatus(GameStatus previousGameStatus, GameStatus nextGameStatus)
        {
            this.previousGameStatus = previousGameStatus;
            this.currentGameStatus = nextGameStatus;
        }

        public bool isArtist()
        {
            string myUuid = roomClient.Me.uuid;
            foreach (string uuid in this.artistUuids)
            {
                if (uuid == myUuid)
                {
                    return true;
                }
            }
            return false;
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            if (data.previousGameStatus == GameStatus.GameStartPhase)
            {
                if (data.nextGameStatus == GameStatus.RoundStartPhase)
                {
                    this.CountPlayerNumber();
                    mainPanel.SwitchPanel(this.othersPanel);
                    this.isGameOwner = false;
                }
            } 
            else if (data.previousGameStatus == GameStatus.RoundStartPhase)
            {
                if (data.nextGameStatus == GameStatus.GameEndPhase)
                {
                    this.StepIntoGameEndPhase();
                    mainPanel.SwitchPanel(this.rankPanel);
                }
                else if (data.nextGameStatus == GameStatus.RoundPickWordPhase)
                {
                    this.artistUuids = data.artistUuids;
                    if (this.isArtist())
                    {
                        mainPanel.SwitchPanel(this.artistPanel);
                    }
                    else 
                    {
                        mainPanel.SwitchPanel(this.othersPanel);
                    }
                }
            }
            else if (data.previousGameStatus == GameStatus.RoundPickWordPhase)
            {
                if (data.nextGameStatus == GameStatus.RoundPlayPhase)
                {
                    this.roundStartTime = Time.time;
                    if (!this.isArtist())
                    {
                        mainPanel.SwitchPanel(this.guessPanel);
                    }
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

        public void StepIntoRoundEndPhase()
        {
            wordGenerator.reset();
            judgeGuess.reset();
            this.ChangeGameStatus(GameStatus.RoundPlayPhase, GameStatus.RoundEndPhase);
            mainPanel.SwitchPanel(this.rankPanel);
        }

        public void StepIntoGameEndPhase()
        {
            this.isGameOwner = false;
        }

        public void PressStartButton()
        {
            if (!this.isGameOwner && currentGameStatus == GameStatus.GameStartPhase)
            {
                this.ChangeGameStatus(GameStatus.GameStartPhase, GameStatus.RoundStartPhase);
                context.SendJson(new Message(
                    GameStatus.GameStartPhase, 
                    GameStatus.RoundStartPhase,
                    this.artistUuids));
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
                mainPanel.SwitchPanel(this.rankPanel);
                this.ChangeGameStatus(GameStatus.RoundStartPhase, GameStatus.GameEndPhase);
                context.SendJson(new Message(
                    GameStatus.RoundStartPhase, 
                    GameStatus.GameEndPhase,
                    this.artistUuids));
                this.StepIntoGameEndPhase();
            }
        }

        public void PressRankOKButton()
        {
            if (this.currentGameStatus == GameStatus.GameEndPhase)
            {
                mainPanel.SwitchPanel(this.startGamePanel);
                this.ChangeGameStatus(GameStatus.GameEndPhase, GameStatus.GameStartPhase);
            }
            else if (this.currentGameStatus == GameStatus.GameStartPhase)
            {
                mainPanel.SwitchPanel(this.startGamePanel);
            }
            else if (this.currentGameStatus == GameStatus.RoundEndPhase)
            {
                this.ChangeGameStatus(GameStatus.RoundEndPhase, GameStatus.RoundStartPhase);
                if (this.isGameOwner)
                {
                    mainPanel.SwitchPanel(this.gameOwnerPanel);
                }
                else
                {
                    mainPanel.SwitchPanel(this.othersPanel);
                }
                
            }
        }

        // RoundStartPhase -> RoundPickWordPhase
        public void PressNewRoundButton()
        {
            if (!this.isGameOwner)
            {
                return;
            }
            // this.CountPlayerNumber();
            this.artistUuids = new string[this.artistNumber];
            this.artistNumber = this.artistNumber > this.playerNumber ? this.playerNumber : this.artistNumber;
            // Debug.Log(artistNumber);
            // Debug.Log(playerNumber);
            for (int i = 0; i < this.artistNumber; i++) 
            {
                while (true)
                {
                    int r = Random.Range(0, this.playerNumber);
                    bool selected = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (this.playerUuids[r] == this.artistUuids[j])
                        {
                            selected = true;
                        }
                    }
                    if (!selected) 
                    {
                        this.artistUuids[i] = this.playerUuids[r];
                        break;
                    }
                }
            }
            if (this.isArtist())
            {
                mainPanel.SwitchPanel(this.artistPanel);
            }
            else 
            {
                mainPanel.SwitchPanel(this.othersPanel);
            }
            this.ChangeGameStatus(GameStatus.RoundStartPhase, GameStatus.RoundPickWordPhase);
            context.SendJson(new Message(
                GameStatus.RoundStartPhase, 
                GameStatus.RoundPickWordPhase,
                this.artistUuids));
        }

        public void PressPickWordButton()
        {
            if (this.isArtist() && this.currentGameStatus == GameStatus.RoundPickWordPhase)
            {
                wordGenerator.GenerateWord();
                string word = wordGenerator.word;
                wordGenerator.ShowWord();
                this.ChangeGameStatus(GameStatus.RoundPickWordPhase, GameStatus.RoundPlayPhase);
                this.roundStartTime = Time.time;
                context.SendJson(new Message(
                    GameStatus.RoundPickWordPhase, 
                    GameStatus.RoundPlayPhase,
                    this.artistUuids));
            }
        }

        private void Update()
        {
            if (this.currentGameStatus == GameStatus.RoundPlayPhase)
            {
                if (Time.time >= this.roundStartTime + this.roundDuration)
                {
                    this.StepIntoRoundEndPhase();
                    return;
                }
                if (this.judgeGuess.correctCount == this.playerNumber - this.artistNumber)
                {
                    this.StepIntoRoundEndPhase();
                    return;
                }
            }
        }
    }
}