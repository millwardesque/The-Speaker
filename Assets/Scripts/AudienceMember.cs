using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudienceMember : MonoBehaviour {
    public float walkSpeed = 1f;
    public float interestThreshold = 0f;
    public Vector2 walkingDirection;

    bool m_isInScoreZone = false;
    AnimatedSprite m_animator;

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

            float angle = Vector2.Angle (walkingDirection, GameManager.Instance.player.transform.position - transform.position);
            if (!m_isInScoreZone && angle > 90f) {
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
        m_animator = GetComponent<AnimatedSprite> ();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 distanceToPlayer = GameManager.Instance.player.transform.position - transform.position;
        Vector3 direction = distanceToPlayer.normalized;

        if (m_isInScoreZone && IsInterested()) {
            if (Mathf.Abs (direction.x) > Mathf.Abs(direction.y)) {
                if (direction.x > 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Stand Right");
                }
                else if (direction.x < 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Stand Left");
                }
            }
            else {
                if (direction.y > 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Stand Up");
                }
                else if (direction.y < 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Stand Down");
                }
            }
        }
        else {
            if (!IsInterested ()) {
                direction = walkingDirection;
            }
            transform.position += direction * walkSpeed * Time.deltaTime;

            if (Mathf.Abs (direction.x) > Mathf.Abs(direction.y)) {
                if (direction.x > 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Walk Right");
                }
                else if (direction.x < 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Walk Left");
                }
            }
            else {
                if (direction.y > 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Walk Up");
                }
                else if (direction.y < 0f) {
                    m_animator.TriggerAnimationIfNotActive ("Walk Down");
                }
            }
        }

        // Sort sub-sprites by Y component to get the order correct.
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer> ();
        for (int i = 0; i < sprites.Length; ++i) {
            sprites[i].sortingOrder = (Mathf.RoundToInt(sprites[i].transform.position.y * 100f) - i) * -1;    
        }
	}

    public bool IsInterested() {
        return CurrentInterest >= interestThreshold;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Audience Out-of-bounds") {
            Destroy (gameObject);
        }
        else if (col.tag == "Score Zone") {
            m_isInScoreZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Score Zone") {
            m_isInScoreZone = false;
        }
    }
}
