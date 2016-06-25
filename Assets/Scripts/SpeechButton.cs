using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpeechButton : MonoBehaviour {
    SpeechConcept m_concept = null;
    Button m_button;

    void Awake() {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(() => OnClicked());
    }

    void Start() {
        UpdateButton();
    }

    void OnClicked() {
        Dictionary<string, object> messageData = new Dictionary<string, object> ();
        messageData ["concept"] = m_concept;
        MessageManager.Instance.SendMessage (new Message(this, "ButtonPushed", messageData));
        GameObject.Destroy (gameObject);
    }

    public void Initialize(SpeechConcept concept) {
        m_concept = concept;
        UpdateButton();                
    }

    void UpdateButton() {
        if (m_button != null && m_concept != null) {
            m_button.GetComponentInChildren<Text>().text = m_concept.name;
        }
    }
}
