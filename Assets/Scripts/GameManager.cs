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

            m_concepts = new Dictionary<string, SpeechConcept>();
            m_scoreKeeper = GetComponent<ScoreKeeper> ();
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
        m_concepts.Clear ();

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_audienceManager = FindObjectOfType<AudienceManager> ();

        LoadLevelData (levelResource);
	}

    void OnCountdownTimerElapsed(Message message) {
        OpenGameOverScreen ();
    }

    void OnButtonPushed(Message message) {
        m_player.Speak (((SpeechConcept)message.data["concept"]));
    }

    void LoadLevelData(string resourceName) {
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

            Audience.RemoveAllAudienceMembers ();
            Audience.GenerateAudienceMembers (10, initialAudienceSpawnContainerOrigin, initialAudienceSpawnContainerSize);
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

    public void OpenGameOverScreen() {
        Time.timeScale = 0f;
        gameOverScreen.gameObject.SetActive (true);
        gameOverScreen.UpdateValues (Score.Score);
    }

    public void RestartGame() {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}
