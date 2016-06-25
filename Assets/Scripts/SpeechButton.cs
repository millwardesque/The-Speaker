using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpeechButton : MonoBehaviour {
    public string concept;

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => OnClicked());
    }

    void OnClicked() {
        Dictionary<string, object> messageData = new Dictionary<string, object> ();
        messageData ["concept"] = concept;
        MessageManager.Instance.SendMessage (new Message(this, "ButtonPushed", messageData));
    }
}
