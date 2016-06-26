using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {
    AudioSource m_audioSource;

    public float minCarRespawn = 10f;
    float m_remaining = 0f;

    void Awake() {
        m_audioSource = GetComponent<AudioSource> ();
    }
	
    void Start() { 
        m_remaining = Random.Range (minCarRespawn / 2f, minCarRespawn);
    }

    void Update() { 
        m_remaining -= Time.deltaTime;
        if (m_remaining <= 0f) {
            m_remaining = minCarRespawn + Random.Range (-minCarRespawn / 2f, minCarRespawn / 2f);

            if (!m_audioSource.isPlaying) {
                m_audioSource.Play ();
                m_audioSource.pitch = Random.Range (0.7f, 1.3f);
                m_audioSource.volume = Random.Range (0.3f, 0.8f);
            }
        }
    }
}
