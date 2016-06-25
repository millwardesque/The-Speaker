using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameManager : MonoBehaviour {
    Player m_player;
    public string levelResource;
    List<SpeechConcept> m_concepts;
    public GameObject buttonContainer;
    public SpeechButton buttonPrefab;

	// Use this for initialization
	void Start () {
        MessageManager.Instance.AddListener ("CountdownTimerElapsed", OnCountdownTimerElapsed);
        MessageManager.Instance.AddListener ("ButtonPushed", OnButtonPushed);
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_concepts = new List<SpeechConcept>();

        LoadLevel (levelResource);
	}

    void OnCountdownTimerElapsed(Message message) {
        Debug.Log ("Countdown expired!");
    }

    void OnButtonPushed(Message message) {
        m_player.Speak (((SpeechConcept)message.data["concept"]).speech);
    }

    void LoadLevel(string resourceName) {
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourceName);
        if (jsonAsset != null) {
            string fileContents = jsonAsset.text;
            var N = JSON.Parse(fileContents);
            var conceptArray = N["concepts"].AsArray;
            foreach (JSONNode concept in conceptArray) {
                string name = concept["name"];
                string speech = concept["speech"];

                Debug.Log (string.Format ("{0}: {1}", name, speech));

                SpeechConcept newConcept = new SpeechConcept(name, speech);
                m_concepts.Add(newConcept);
                CreateButton(newConcept);
            }
        }
        else {
            Debug.LogError("Unable to load level data from JSON at '" + resourceName + "': There was an error opening the file.");
        }
    }

    void CreateButton(SpeechConcept concept) {
        SpeechButton newButton = Instantiate<SpeechButton>(buttonPrefab);
        newButton.Initialize(concept);
        newButton.transform.SetParent(buttonContainer.transform, false);
    }
}
