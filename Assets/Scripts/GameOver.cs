using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver : MonoBehaviour {
    public Text scoreLabel;

    public void UpdateValues(int score) {
        scoreLabel.text = score.ToString ();
    }
}
