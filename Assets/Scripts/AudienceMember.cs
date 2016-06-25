using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudienceMember : MonoBehaviour {
    public float minDistanceFromPlayer = 2f;
    public float walkSpeed = 1f;
    public float interestThreshold = 0f;

    Animator[] m_animators;

    Dictionary<string, float> m_interests;
    public Dictionary<string, float> Interests {
        get { return m_interests; }
        set { m_interests = value; }
    }

    public float CurrentInterest {
        get {
            if (m_interests == null) {
                return 0f;
            }

            float overallInterest = 0f;
            int matchingTopics = 0;
            Player player = GameManager.Instance.player;
            foreach (string trait in m_interests.Keys) {
                List<SpeechConcept> concepts = player.FindActiveConceptsWithTrait(trait);

                foreach (SpeechConcept concept in concepts) {
                    matchingTopics ++;
                    overallInterest += m_interests [trait] * concept.value;
                }
            }
            if (matchingTopics == 0) {
                return 0f;
            }
            else {
                return overallInterest / matchingTopics;    
            }
        }
    }

    void Awake() { 
        m_interests = new Dictionary<string, float> ();
    }


    // Use this for initialization
	void Start () {
        m_animators = GetComponentsInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 distanceToPlayer = GameManager.Instance.player.transform.position - transform.position;
        if (distanceToPlayer.magnitude >= minDistanceFromPlayer || !IsInterested ()) {
            Vector3 direction = distanceToPlayer.normalized;
            if (!IsInterested ()) {
                direction *= -1f;
            }
            transform.position += direction * walkSpeed * Time.deltaTime;

            if (direction.y > 0f) {
                TriggerAnimationIfNotActive ("Walk Up");
            }
            else if (direction.y < 0f) {
                TriggerAnimationIfNotActive ("Walk Down");
            }
        }
        else {
            if (IsInterested ()) {
                TriggerAnimationIfNotActive ("Stand Up");
            }
            else {
                TriggerAnimationIfNotActive ("Stand Down");
            }
        }
	}

    void TriggerAnimationIfNotActive(string animationName) {
        for (int i = 0; i < m_animators.Length; ++i) {
            if (!m_animators[i].GetCurrentAnimatorStateInfo(0).IsName(animationName)) {
                m_animators[i].SetTrigger (animationName);
            }    
        }
    }

    public bool IsInterested() {
        return CurrentInterest >= interestThreshold;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Audience Out-of-bounds") {
            Destroy (gameObject);
        }
    }
}
