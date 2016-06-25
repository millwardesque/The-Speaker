﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudienceMember : MonoBehaviour {
    public float minDistanceFromPlayer = 2f;
    public float walkSpeed = 1f;
    public float interestThreshold = 0f;
    public Vector2 walkingDirection;

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
        if (distanceToPlayer.magnitude >= minDistanceFromPlayer || !IsInterested ()) {
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
        else {
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
