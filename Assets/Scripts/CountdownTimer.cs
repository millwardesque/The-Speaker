using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class CountdownTimer : MonoBehaviour {
    public float duration = 60f;

    Text m_countdownLabel;
    float m_remaining;
    bool m_hasElapsed = false;

    void Awake() {
        m_countdownLabel = GetComponent<Text> ();
        if (m_countdownLabel == null) {
            Debug.Log (string.Format("{0}: Countdown label isn't assigned", name));
        }
    }

	// Use this for initialization
	void Start () {
        ResetTimer ();
	}
	
	// Update is called once per frame
	void Update () {
        if (!m_hasElapsed) {
            int previousSeconds = Mathf.FloorToInt (m_remaining);
            m_remaining -= Time.deltaTime;
            int newSeconds = Mathf.FloorToInt (m_remaining);

            if (previousSeconds != newSeconds && newSeconds >= 0) {
                m_countdownLabel.text = newSeconds.ToString ();
            }

            if (m_remaining <= 0f) {
                OnElapsed ();
            }
        }
	}

    void OnElapsed() {
        m_hasElapsed = true;
        MessageManager.Instance.SendMessage (new Message(this, "CountdownTimerElapsed", null));
    }

    public void ResetTimer() {
        m_remaining = duration;
        m_hasElapsed = false;
    }
}
