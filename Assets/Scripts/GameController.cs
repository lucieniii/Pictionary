using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using Ubiq.Samples;
using Ubiq.Rooms;
using DrawAndGuess.Guess;
using UnityEngine.UI;

namespace DrawAndGuess.Procedure
{
    public class GameController : MonoBehaviour
    {
        // TODO: Game button
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
        public float timeRemain;

        public RoomClient roomClient;

        private NetworkContext context;

        public bool isGameOwner;
        public int playerNumber;
        public string[] playerUuids;
        public string[] playerNames;
        
        public int artistNumber = 1; // Hard coded
        public float roundDuration; // Hard coded (second)
        public string[] artistUuids;
        public int roundCounter;

        public PanelSwitcher mainPanel;

        public WordGenerator wordGenerator;
        public JudgeGuess judgeGuess;
        public Ranking ranking;

        public GameObject startGamePanel;
        public GameObject othersPanel;
        public GameObject gameOwnerPanel;
        public GameObject rankPanel;
        public GameObject artistPanel;
        public GameObject guessPanel;

        public Image artistCountdown;
        public Image guesserCountdown;
        public Text artistCountdownText;
        public Text guesserCountdownText;

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
            this.roundDuration = 120.0f;
            this.roundCounter = 0;
        }

        public void CountPlayerNumber()
        {
            this.playerNumber = 1;
            foreach (var peer in roomClient.Peers)
            {
                this.playerNumber += 1;
            }
            this.playerUuids = new string [this.playerNumber];
            this.playerNames = new string [this.playerNumber];
            this.playerUuids[0] = roomClient.Me.uuid;
            this.playerNames[0] = roomClient.Me["ubiq.samples.social.name"];
            int i = 1;
            foreach (var peer in roomClient.Peers)
            {
                this.playerUuids[i] = peer.uuid;
                this.playerNames[i] = peer["ubiq.samples.social.name"];
                i += 1;
            }
            // Debug.Log(this.playerUuids);
            this.judgeGuess.guesserNumber = this.playerNumber - this.artistNumber;
            this.judgeGuess.correctTimeRecord = new float[this.judgeGuess.guesserNumber];
            this.judgeGuess.correctNameRecord = new string[this.judgeGuess.guesserNumber];
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
                    ranking.initRankingBoard(this.playerNumber, this.playerNames);
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
                    judgeGuess.roundStartTime = this.roundStartTime;
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

        public void DeleteDrawings()
        {
            GameObject[] obj = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject child in obj)
            {
                if (child.gameObject.name == "Drawing")
                {
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }
        }

        public void StepIntoRoundEndPhase()
        {
            ranking.updateRankingBoard(
                judgeGuess.correctNameRecord,
                judgeGuess.correctTimeRecord,
                this.roundStartTime,
                this.playerNumber,
                judgeGuess.correctCount,
                this.playerUuids,
                this.playerNames,
                this.artistUuids,
                this.artistNumber
            );
            ranking.showRankingBoard(this.playerNumber);
            wordGenerator.reset();
            judgeGuess.reset();
            this.DeleteDrawings();
            this.ChangeGameStatus(GameStatus.RoundPlayPhase, GameStatus.RoundEndPhase);
            mainPanel.SwitchPanel(this.rankPanel);
            this.timeRemain = 120;
            artistCountdown.fillAmount = 1;
            guesserCountdown.fillAmount = 1;
            artistCountdownText.text = "02:00";
            guesserCountdownText.text = "02:00";
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
                ranking.initRankingBoard(this.playerNumber, this.playerNames);
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
            else if (this.currentGameStatus == GameStatus.RoundStartPhase)
            {
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

            if (this.artistNumber == 1) 
            {
                int nowArtistIndex = roundCounter % this.playerNumber;
                this.artistUuids[0] = this.playerUuids[nowArtistIndex];
                roundCounter++;
            }
            else
            {
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
                judgeGuess.roundStartTime = this.roundStartTime;
                context.SendJson(new Message(
                    GameStatus.RoundPickWordPhase, 
                    GameStatus.RoundPlayPhase,
                    this.artistUuids));
            }
        }

        private void Update()
        {
            float t = Time.time;
            if (this.currentGameStatus == GameStatus.RoundPlayPhase)
            {
                if (t >= this.roundStartTime + this.roundDuration)
                {
                    this.StepIntoRoundEndPhase();
                    return;
                }
                if (this.judgeGuess.correctCount == this.playerNumber - this.artistNumber)
                {
                    this.StepIntoRoundEndPhase();
                    return;
                }
                this.timeRemain = this.roundDuration - (t - this.roundStartTime);
                artistCountdown.fillAmount = timeRemain / this.roundDuration;
                guesserCountdown.fillAmount = timeRemain / this.roundDuration;
                int minuteRemain = (int)timeRemain / 60;
                int secondRemain = (int)timeRemain % 60;
                string timeRemainText = System.String.Format("{0:00}:{1:00}", minuteRemain, secondRemain);
                artistCountdownText.text = timeRemainText;
                guesserCountdownText.text = timeRemainText;
            }
            
        }

        public void SwitchToStagePanel()
        {
            switch (this.currentGameStatus)
            {
                case GameStatus.GameStartPhase:
                    mainPanel.SwitchPanel(this.startGamePanel);
                    break;
                case GameStatus.RoundStartPhase:
                    if (this.isGameOwner)
                    {
                        mainPanel.SwitchPanel(this.gameOwnerPanel);
                    }
                    else
                    {
                        mainPanel.SwitchPanel(this.othersPanel);
                    }
                    break;
                case GameStatus.RoundPickWordPhase:
                    if (this.isArtist())
                    {
                        mainPanel.SwitchPanel(this.artistPanel);
                    }
                    else
                    {
                        mainPanel.SwitchPanel(this.othersPanel);
                    }
                    break;
                case GameStatus.RoundPlayPhase:
                    if (this.isArtist())
                    {
                        mainPanel.SwitchPanel(this.artistPanel);
                    }
                    else
                    {
                        mainPanel.SwitchPanel(this.guessPanel);
                    }
                    break;
                default:
                    mainPanel.SwitchPanel(this.rankPanel);
                    break;
            }
        }
    }
}