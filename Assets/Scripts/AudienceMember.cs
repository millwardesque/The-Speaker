using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum AudienceMemberState {
    Disinterested,
    Intrigued,
    Interested,
    Hooked,
    Satisfied
}

public class AudienceMember : MonoBehaviour {
    public float walkSpeed = 1f;
    float intrigueThreshold = 0.75f;
    float interestThreshold = 0.9f;
    public Vector2 walkingDirection;
    public GameObject speechBubble;

    float speechBubbleDuration = 1f;
    float m_speechBubbleElapsed = 0f;

    bool m_isInScoreZone = false;
    AnimatedSprite m_animator;

    AudienceMemberState m_state;
    public AudienceMemberState State {
        get { return m_state; }
        set {
            if (m_state == value) {
                return;
            }

            switch (value) {
            case AudienceMemberState.Disinterested:
                HideSpeechBubble ();
                break;
            case AudienceMemberState.Intrigued:
                LookAtPlayer ();
                ShowSpeechBubble ("?");
                break;
            case AudienceMemberState.Interested:
                LookAtPlayer ();
                ShowSpeechBubble ("!");
                break;
            case AudienceMemberState.Hooked:
                ShowSpeechBubble ("!!");
                LookAtPlayer ();
                break;
            case AudienceMemberState.Satisfied:
                m_speechBubbleElapsed = 0f;
                ShowSpeechBubble ("$", Color.green);
                break;
            default:
                break;
            }

            m_state = value;
        }
    }

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

            float penalty = 0f;

            // Audience members who've passed the player are harder to interest.
            float angle = Vector2.Angle (walkingDirection, GameManager.Instance.player.transform.position - transform.position);
            if (angle > 90f) {
                penalty = m_isInScoreZone ? 0.1f : 0.2f;   // Penalty is less severe for members in the score zone
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
                return overallInterest * (1f - penalty);
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
        if (State == AudienceMemberState.Disinterested) {
            Walk (walkingDirection);
        }
        else if (State == AudienceMemberState.Intrigued) {
            // No-op.
        }
        else if (State == AudienceMemberState.Interested) {
            Vector3 distanceToPlayer = GameManager.Instance.player.transform.position - transform.position;
            Vector3 direction = distanceToPlayer.normalized;
            Walk (direction);
        }
        else if (State == AudienceMemberState.Hooked) {
            // No-op.
        }
        else if (State == AudienceMemberState.Satisfied) {
            Walk (walkingDirection);

            if (m_speechBubbleElapsed < speechBubbleDuration) {
                m_speechBubbleElapsed += Time.deltaTime;

                if (m_speechBubbleElapsed > speechBubbleDuration) {
                    HideSpeechBubble ();
                }
            }
        }

        // Change states if necessary.
        if (State != AudienceMemberState.Satisfied) {
            if (m_isInScoreZone && CurrentInterest > interestThreshold) {
                State = AudienceMemberState.Hooked;
            } else if (CurrentInterest > interestThreshold) {
                State = AudienceMemberState.Interested;
            } else if (CurrentInterest > intrigueThreshold) {
                State = AudienceMemberState.Intrigued;
            } else {
                State = AudienceMemberState.Disinterested;
            }
        }

        // Sort sub-sprites by Y component to get the order correct.
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer> ();
        for (int i = 0; i < sprites.Length; ++i) {
            sprites[i].sortingOrder = (Mathf.RoundToInt(sprites[i].transform.position.y * 100f) - i) * -1;    
        }
	}

    void Walk(Vector3 direction) {
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

    Vector2 DistanceToPlayer() {
        return GameManager.Instance.player.transform.position - transform.position;
    }

    void LookAtPlayer() {
        Vector2 directionToPlayer = DistanceToPlayer ().normalized;
        if (Mathf.Abs (directionToPlayer.x) > Mathf.Abs(directionToPlayer.y)) {
            if (directionToPlayer.x > 0f) {
                m_animator.TriggerAnimationIfNotActive ("Stand Right");
            }
            else if (directionToPlayer.x < 0f) {
                m_animator.TriggerAnimationIfNotActive ("Stand Left");
            }
        }
        else {
            if (directionToPlayer.y > 0f) {
                m_animator.TriggerAnimationIfNotActive ("Stand Up");
            }
            else if (directionToPlayer.y < 0f) {
                m_animator.TriggerAnimationIfNotActive ("Stand Down");
            }
        }
    }

    void ShowSpeechBubble(string text) {
        ShowSpeechBubble (text, Color.blue);
    }
    void ShowSpeechBubble(string text, Color color) {
        speechBubble.SetActive (true);
        speechBubble.GetComponentInChildren<Text> ().text = text;
        speechBubble.GetComponentInChildren<Text> ().color = color;
    }

    void HideSpeechBubble() {
        speechBubble.SetActive (false);
    }
}
