using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameManager : MonoBehaviour {    
    public string levelResource;
    public GameObject buttonContainer;
    public SpeechButton buttonPrefab;

    List<string> m_traits = new List<string> ();
    public List<string> Traits {
        get { return m_traits; }
    }

    Dictionary<string, SpeechConcept> m_concepts;
    public Dictionary<string, SpeechConcept> Concepts {
        get { return m_concepts; }
    }

    public static GameManager Instance = null;

    Player m_player;
    public Player player {
        get { return m_player; }
    }

    AudienceManager m_audienceManager;
    public AudienceManager Audience {
        get { return m_audienceManager; }
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;

            m_concepts = new Dictionary<string, SpeechConcept>();
        }
        else {
            GameObject.Destroy (gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        MessageManager.Instance.AddListener ("CountdownTimerElapsed", OnCountdownTimerElapsed);
        MessageManager.Instance.AddListener ("ButtonPushed", OnButtonPushed);
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_audienceManager = FindObjectOfType<AudienceManager> ();
        m_concepts.Clear ();

        LoadLevel (levelResource);
	}

    void OnCountdownTimerElapsed(Message message) {
        Debug.Log ("Countdown expired!");
    }

    void OnButtonPushed(Message message) {
        m_player.Speak (((SpeechConcept)message.data["concept"]));
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
                string trait = concept["trait"];
                float value = concept ["value"].AsFloat;

                if (!m_traits.Contains (trait)) {
                    m_traits.Add (trait);
                }

                SpeechConcept newConcept = new SpeechConcept(name, speech, trait, value);
                m_concepts[name] = newConcept;
                CreateButton(newConcept);
            }

            Audience.GenerateAudienceMembers (10, new Vector2(0f, -2f), new Vector2(6f, 3f));
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
