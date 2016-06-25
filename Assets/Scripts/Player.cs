using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public GameObject m_speechBubble;
    public float speechMessageDuration;
    public float conceptDuration = 1f;

    Dictionary<string, float> m_activeConcepts;
    public Dictionary<string, float> ActiveConcepts {
        get { return m_activeConcepts; }
        set { m_activeConcepts = value; }
    }

    float m_speechMessageElapsed;
    bool m_isSpeaking;

    AnimatedSprite m_animator;

    void Awake() {
        m_activeConcepts = new Dictionary<string, float> ();
        m_animator = GetComponent<AnimatedSprite> ();
    }

    void Start() {
        m_activeConcepts.Clear ();
    }

    void Update() {
        if (m_isSpeaking) {
            m_speechMessageElapsed += Time.deltaTime;

            if (m_speechMessageElapsed > speechMessageDuration) {
                StopSpeaking ();    
            }
        }

        // Update any active concepts.
        List<string> expiredConcepts = new List<string> ();
        List<string> concepts = new List<string> (m_activeConcepts.Keys);
        foreach (string concept in concepts) {
            m_activeConcepts [concept] -= Time.deltaTime;
            if (m_activeConcepts[concept] < 0f) {
                expiredConcepts.Add(concept);
            }
        }

        // Clean up the list of expired concepts.
        foreach (string concept in expiredConcepts) {
            m_activeConcepts.Remove (concept);
        }
    }

    public void Speak(SpeechConcept concept) {
        m_speechMessageElapsed = 0f;
        m_speechBubble.SetActive (true);
        m_speechBubble.GetComponentInChildren<Text>().text = concept.speech;
        m_isSpeaking = true;
        m_animator.TriggerAnimationIfNotActive ("Speak Down");
    
        if (!m_activeConcepts.ContainsKey (concept.name)) {
            m_activeConcepts[concept.name] = conceptDuration;
        }
        else {
            m_activeConcepts [concept.name] += conceptDuration;
        }
    }

    public void StopSpeaking() {
        m_speechBubble.GetComponentInChildren<Text> ().text = "";
        m_speechBubble.SetActive (false);
        m_isSpeaking = false;
        m_speechMessageElapsed = 0f;

        m_animator.TriggerAnimationIfNotActive ("Stand Down");
    }

    public List<SpeechConcept> FindActiveConceptsWithTrait(string trait) {
        List<SpeechConcept> matches = new List<SpeechConcept> ();

        foreach (string concept in ActiveConcepts.Keys) {
            if (GameManager.Instance.Concepts [concept].trait == trait) {
                matches.Add (GameManager.Instance.Concepts [concept]);
            }
        }

        return matches;
    }
}
