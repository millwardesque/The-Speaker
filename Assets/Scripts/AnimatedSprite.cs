using UnityEngine;
using System.Collections;

public class AnimatedSprite : MonoBehaviour {
    Animator[] m_animators;

	// Use this for initialization
	void Start () {
        m_animators = GetComponentsInChildren<Animator> ();
	}

    public void TriggerAnimationIfNotActive(string animationName) {
        string stateName = "Base." + animationName;
        for (int i = 0; i < m_animators.Length; ++i) {
            if (!m_animators[i].GetCurrentAnimatorStateInfo(0).IsName(stateName)) {
                if (name == "Player") {
                    Debug.Log ("Allowed: " + stateName + " against " + m_animators[i].name);
                }
                m_animators[i].SetTrigger (animationName);
            }
            else {
                if (name == "Player") {
                    Debug.Log ("Denied: " + stateName + " against " + m_animators[i].name);
                }
            }
        }
    }
}
