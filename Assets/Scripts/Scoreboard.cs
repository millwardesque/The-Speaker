using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {
    public Text scoreLabel;

    void Start() {
        MessageManager.Instance.AddListener ("ScoreUpdate", OnScoreUpdated);
    }

    void OnScoreUpdated(Message message) {
        int score = (int)message.data ["score"];
        scoreLabel.text = score.ToString ();
    }
}
