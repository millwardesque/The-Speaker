using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public GameObject m_speechBubble;
    public float speechMessageDuration;

    float m_speechMessageElapsed;
    bool m_isSpeaking;

    void Update() {
        if (m_isSpeaking) {
            m_speechMessageElapsed += Time.deltaTime;

            if (m_speechMessageElapsed > speechMessageDuration) {
                StopSpeaking ();    
            }
        }   
    }
    public void Speak(string concept) {
        m_speechBubble.SetActive (true);
        m_isSpeaking = true;
    }

    public void StopSpeaking() {
        m_speechBubble.SetActive (false);
        m_isSpeaking = false;
        m_speechMessageElapsed = 0f;
    }
}
