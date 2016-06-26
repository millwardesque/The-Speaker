using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {
    public Text scoreLabel;
    public AudioClip[] soundClips;
    AudioSource m_scoreSound;

    void Awake() {
        m_scoreSound = GetComponent<AudioSource> ();
    }

    void Start() {
        MessageManager.Instance.AddListener ("ScoreUpdate", OnScoreUpdated);
    }

    void OnScoreUpdated(Message message) {
        int score = (int)message.data ["score"];
        scoreLabel.text = score.ToString ();

        if (!m_scoreSound.isPlaying && score > 0) {
            int index = Random.Range (0, soundClips.Length);
            m_scoreSound.clip = soundClips [index];
            m_scoreSound.pitch = Random.Range (0.7f, 1.3f);
            m_scoreSound.volume = Random.Range (0.5f, 0.8f);
            m_scoreSound.Play ();
        }
    }
}
