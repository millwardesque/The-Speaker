using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameManager : MonoBehaviour {    
    public string levelResource;
    public GameObject buttonContainer;
    public SpeechButton buttonPrefab;
    public Vector2 initialAudienceSpawnContainerOrigin;
    public Vector2 initialAudienceSpawnContainerSize;
    public GameOver gameOverScreen;

    Dictionary<string, SpeechConcept> conceptsByTrait;

    bool m_levelLoaded = false;

    List<string> m_traits = new List<string> ();
    public List<string> Traits {
        get { return m_traits; }
    }

    SpeechManager m_speechManager;
    public SpeechManager Speeches {
        get { return m_speechManager; }
    }

    public static GameManager Instance = null;

    Player m_player;
    public Player player {
        get { return m_player; }
    }

    ScoreKeeper m_scoreKeeper;
    public ScoreKeeper Score {
        get { return m_scoreKeeper; }
    }

    AudienceManager m_audienceManager;
    public AudienceManager Audience {
        get { return m_audienceManager; }
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            GameObject.Destroy (gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        MessageManager.Instance.AddListener ("CountdownTimerElapsed", OnCountdownTimerElapsed);
        MessageManager.Instance.AddListener ("ButtonPushed", OnButtonPushed);

        Time.timeScale = 1f;
        m_levelLoaded = false;

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_audienceManager = FindObjectOfType<AudienceManager> ();
        m_speechManager = GetComponent<SpeechManager>();
        m_speechManager.Clear ();
        m_scoreKeeper = FindObjectOfType<ScoreKeeper> ();
	}

    void Update() {
        if (!m_levelLoaded) {
            m_levelLoaded = true;
            LoadLevelData (levelResource);
        }
    }

    void OnCountdownTimerElapsed(Message message) {
        OpenGameOverScreen ();
    }

    void OnButtonPushed(Message message) {
        SpeechConcept concept = (SpeechConcept)message.data ["concept"];
        int buttonIndex = (int)message.data ["index"];
        m_player.Speak (concept);
        CreateButton(Speeches.GetNextWithTrait (concept.trait), buttonIndex);
    }

    void LoadLevelData(string resourceName) {
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourceName);
        if (jsonAsset != null) {
            string fileContents = jsonAsset.text;
            var N = JSON.Parse(fileContents);

            float levelDuration = N ["level_duration"].AsFloat;
            FindObjectOfType<CountdownTimer> ().duration = levelDuration;
            FindObjectOfType<CountdownTimer> ().ResetTimer ();

            float conceptDuration = N ["concept_duration"].AsFloat;
            player.conceptDuration = conceptDuration;

            float audienceSpawnTime = N ["audience_spawn_time"].AsFloat;
            Audience.audienceSpawnTime = audienceSpawnTime;

            // Load the concepts.
            Dictionary<string, SpeechConcept> concepts = new Dictionary<string, SpeechConcept>();
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
                concepts[name] = newConcept;
            }
            Speeches.Concepts = concepts;

            // Create buttons for the starting traits
            foreach (string trait in m_traits) {
                CreateButton (Speeches.GetNextWithTrait (trait));
            }

            // Load the audience types.
            Audience.AudienceTypes.Clear ();
            var audienceTypeArray = N["audience_types"].AsArray;
            foreach (JSONNode type in audienceTypeArray) {
                string name = type["name"];
                string prefabName = type["prefab_name"];
                string primaryTrait = type["primary_trait"];
                AudienceMember typePrefab = Resources.Load<AudienceMember>("Levels/" + prefabName);
                if (typePrefab == null) {
                    Debug.Log("Unable to load type prefab '" + prefabName + "'");
                }
                Audience.AudienceTypes.Add (name, new AudienceType(name, typePrefab, primaryTrait));
            }

            int initialAudienceSize = N ["initial_audience_size"].AsInt;
            Audience.RemoveAllAudienceMembers ();
            Audience.GenerateAudienceMembers (initialAudienceSize, initialAudienceSpawnContainerOrigin, initialAudienceSpawnContainerSize);
        }
        else {
            Debug.LogError("Unable to load level data from JSON at '" + resourceName + "': There was an error opening the file.");
        }
    }

    void CreateButton(SpeechConcept concept, int index = -1) {
        SpeechButton newButton = Instantiate<SpeechButton>(buttonPrefab);
        newButton.Initialize(concept);
        newButton.transform.SetParent (buttonContainer.transform, false);

        if (index != -1) {
            newButton.transform.SetSiblingIndex (index);
        }
    }

    public void OpenGameOverScreen() {
        Time.timeScale = 0f;
        gameOverScreen.gameObject.SetActive (true);
        gameOverScreen.UpdateValues (Score.Score);
    }

    public void RestartGame() {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
