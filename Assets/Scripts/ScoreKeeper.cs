using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreKeeper : MonoBehaviour {
    public int pointsPerInterestedAudienceMember = 10;
    public float scoreUpdateFrequency = 1f;
    float m_timeUntilUpdate = 0f;

    int m_score;
    public int Score {
        get { return m_score; }
        set {
            m_score = value;
            Dictionary<string, object> messageData = new Dictionary<string, object> ();
            messageData ["score"] = m_score;
            MessageManager.Instance.SendMessage (new Message(this, "ScoreUpdate", messageData));
        }
    }

    public static ScoreKeeper Instance = null;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            GameObject.Destroy (this);
        }
    }

    void Start() {
        m_timeUntilUpdate = scoreUpdateFrequency;
        Score = 0;
    }

    void Update() {
        m_timeUntilUpdate -= Time.deltaTime;
        while (m_timeUntilUpdate <= 0f) {
            UpdateScore ();
            m_timeUntilUpdate += scoreUpdateFrequency;
        }
    }

    void UpdateScore() {
        AudienceMember[] audience = GameManager.Instance.Audience.GetAudienceMembers ();
        foreach (AudienceMember member in audience) {
            if (member.IsInterested()) {
                Score += pointsPerInterestedAudienceMember;
            }
        }
    }
}
