using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreKeeper : MonoBehaviour {
    public int pointsPerInterestedAudienceMember = 10;
    public float scoreUpdateFrequency = 1f;
    float m_timeUntilUpdate = 0f;
    List<AudienceMember> m_inRangeScorers;

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

            m_inRangeScorers = new List<AudienceMember> ();
        }
        else {
            GameObject.Destroy (this);
        }
    }

    void Start() {
        m_timeUntilUpdate = scoreUpdateFrequency;
        Score = 0;
        m_inRangeScorers.Clear ();
    }

    void Update() {
        m_timeUntilUpdate -= Time.deltaTime;
        while (m_timeUntilUpdate <= 0f) {
            UpdateScore ();
            m_timeUntilUpdate += scoreUpdateFrequency;
        }
    }

    void UpdateScore() {
        foreach (AudienceMember member in m_inRangeScorers) {
            if (member.State == AudienceMemberState.Hooked) {
                Score += pointsPerInterestedAudienceMember;
                member.State = AudienceMemberState.Satisfied;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        AudienceMember member = col.GetComponent<AudienceMember>();
        if (member != null && !m_inRangeScorers.Contains (member) ) {
            m_inRangeScorers.Add (member);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        AudienceMember member = col.GetComponent<AudienceMember>();
        if (member != null && m_inRangeScorers.Contains (member) ) {
            m_inRangeScorers.Remove (member);
        }
    }
}
