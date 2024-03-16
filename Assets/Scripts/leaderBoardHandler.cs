using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class leaderBoardHandler : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;

    void Start(){
        DisplayLeaderboard(3);
    }

    private List<int> LoadScores(int diskCount)
    {
        List<int> scores = new List<int>();
        string scoresString = PlayerPrefs.GetString($"Leaderboard_{diskCount}", ""); // Dynamic key based on disk count
        if (!string.IsNullOrEmpty(scoresString))
        {
            string[] scoresArray = scoresString.Split(',');
            foreach (string scoreStr in scoresArray)
            {
                if (int.TryParse(scoreStr, out int score))
                {
                    scores.Add(score);
                }
            }
        }
        scores.Sort((a, b) => a.CompareTo(b)); 
        return scores;
    }


   private void DisplayLeaderboard(int diskCount)
    {
        List<int> scores = LoadScores(diskCount);

        if (scores.Count > 10)
        {
            scores = scores.GetRange(0, 10);
        }

        string leaderboardString = "Leaderboard\n";
        for (int i = 0; i < scores.Count; i++)
        {
            leaderboardString += $"{i + 1}. {scores[i]}\n";
        }
        leaderboardText.text = leaderboardString;
    }

    public void UpdateDisplayedLeaderboard(int diskCount)
    {
        DisplayLeaderboard(diskCount);
    }

}
