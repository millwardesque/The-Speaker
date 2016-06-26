using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeechManager : MonoBehaviour {
    Dictionary<string, SpeechConcept> m_concepts;
    public Dictionary<string, SpeechConcept> Concepts {
        get { return m_concepts; }
        set {
            m_concepts = value;

            foreach (SpeechConcept concept in m_concepts.Values) {
                if (!m_traitQueue.ContainsKey (concept.trait)) {
                    m_traitQueue.Add (concept.trait, new Queue<SpeechConcept>());
                }
                m_traitQueue [concept.trait].Enqueue (concept);
            }
        }
    }

    Dictionary<string, Queue<SpeechConcept>> m_traitQueue;

    public SpeechConcept GetNextWithTrait(string trait) {
        Debug.Log (string.Format ("Next button trait {0}: {1} available. {2} is next", trait, m_traitQueue[trait].Count, m_traitQueue[trait].Peek().speech));
        SpeechConcept concept = m_traitQueue [trait].Dequeue ();
        m_traitQueue [trait].Enqueue (concept);
        return concept;
    }

    void Awake() {
        m_concepts = new Dictionary<string, SpeechConcept> ();
        m_traitQueue = new Dictionary<string, Queue<SpeechConcept>> ();
    }

    public void Clear() {
        m_concepts.Clear ();
        m_traitQueue.Clear ();
    }
}
