using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Messaging;
using Ubiq.Rooms;

namespace DrawAndGuess.Procedure
{
    public class Ranking : MonoBehaviour
    {
        public Text name1;
        public Text change1;
        public Text score1;
        public Text name2;
        public Text change2;
        public Text score2;
        public Text name3;
        public Text change3;
        public Text score3;
        public Text name4;
        public Text change4;
        public Text score4;
        public Text name5;
        public Text change5;
        public Text score5;
        public Text name6;
        public Text change6;
        public Text score6;
        public Text name7;
        public Text change7;
        public Text score7;

        private Text[] names;
        private Text[] changes;
        private Text[] scores;

        public int[] scoresInt;
        public int[] changesInt;
        public string[] namesString;

        public int roundDuration = 120;

        private int listLength;

        private void constructTextArrays()
        {
            names = new Text[this.listLength];
            namesString = new string[this.listLength];
            changes = new Text[this.listLength];
            changesInt = new int[this.listLength];
            scores = new Text[this.listLength];
            scoresInt = new int[this.listLength];

            names[0] = name1;
            changes[0] = change1;
            scores[0] = score1;
            names[1] = name2;
            changes[1] = change2;
            scores[1] = score2;
            names[2] = name3;
            changes[2] = change3;
            scores[2] = score3;
            names[3] = name4;
            changes[3] = change4;
            scores[3] = score4;
            names[4] = name5;
            changes[4] = change5;
            scores[4] = score5;
            names[5] = name6;
            changes[5] = change6;
            scores[5] = score6;
            names[6] = name7;
            changes[6] = change7;
            scores[6] = score7;
        }

        private void Start()
        {
            this.listLength = 7;
            this.roundDuration = 20;
        }

        public void initRankingBoard(int playerNumber, string[] playerNames)
        {
            // Debug.Log("init ranking board");
            this.constructTextArrays();
            for (int i = 0; i < playerNumber; i++)
            {
                namesString[i] = playerNames[i];
                names[i].text = playerNames[i];
                changesInt[i] = 0;
                changes[i].text = "";
                scoresInt[i] = 0;
                scores[i].text = scoresInt[i].ToString();
            }
            for (int i = playerNumber; i < this.listLength; i++)
            {
                namesString[i] = "";
                names[i].text = "";
                scoresInt[i] = 0;
                scores[i].text = "";
            }
        }

        public void showRankingBoard(int playerNumber)
        {
            for (int i = 0; i < playerNumber; i++)
            {
                for (int j = i; j < playerNumber; j++)
                {
                    if (scoresInt[j] > scoresInt[i])
                    {
                        string tempString = namesString[i];
                        namesString[i] = namesString[j];
                        namesString[j] = tempString;

                        int tempInt = scoresInt[i];
                        scoresInt[i] = scoresInt[j];
                        scoresInt[j] = tempInt;

                        tempInt = changesInt[i];
                        changesInt[i] = changesInt[j];
                        changesInt[j] = tempInt;
                    }
                }
            }
            for (int i = 0; i < playerNumber; i++)
            {
                names[i].text = namesString[i];
                changes[i].text = "+" + changesInt[i].ToString();
                scores[i].text = scoresInt[i].ToString();
            }
        }

        public bool isArtist(string name, 
            string[] playerUuids,
            string[] playerNames,
            string[] artistUuids,
            int playerNumber,
            int artistNumber)
        {
            for (int i = 0; i < playerNumber; i++)
            {
                if (name == playerNames[i])
                {
                    for (int j = 0; j < artistNumber; j++)
                    {
                        // Debug.Log(playerUuids[i]);
                        // Debug.Log(artistUuids[j]);
                        if (playerUuids[i] == artistUuids[j])
                        {
                            return true;
                        }
                    }
                    break;
                }
            }
            return false;
        }

        public void updateRankingBoard(
            string[] correctNameRecord, 
            float[] correctTimeRecord, 
            float roundStartTime, 
            int playerNumber, 
            int correctCount,
            string[] playerUuids,
            string[] playerNames,
            string[] artistUuids,
            int artistNumber)
        {
            int artistScore = 0;
            for (int i = 0; i < listLength; i++)
            {
                changesInt[i] = 0;
            }
            for (int i = 0; i < playerNumber; i++)
            {
                for (int j = 0; j < correctCount; j++)
                {
                    if (correctNameRecord[j] == namesString[i])
                    {
                        changesInt[i] = this.roundDuration - (int) (correctTimeRecord[j]);
                        scoresInt[i] += changesInt[i];
                        artistScore += changesInt[i];
                    }
                }
            }
            artistScore /= playerNumber;
            for (int i = 0; i < playerNumber; i++)
            {
                if (isArtist(
                        namesString[i],
                        playerUuids, 
                        playerNames,
                        artistUuids,
                        playerNumber,
                        artistNumber))
                {
                    changesInt[i] = artistScore;
                    scoresInt[i] += changesInt[i];
                }
            }
        }
    }
}