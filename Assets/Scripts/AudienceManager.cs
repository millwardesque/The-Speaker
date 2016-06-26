using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudienceType {
    public string name;
    public AudienceMember prefab;
    public string primaryTrait;

    public AudienceType (string name, AudienceMember prefab, string primaryTrait) {
        this.name = name;
        this.prefab = prefab;
        this.primaryTrait = primaryTrait;
    }

    public override string ToString() {
        return string.Format("Name: '{0}', Prefab: '{1}', Primary trait: {2}", name, (prefab == null ? "<null>" : prefab.name), primaryTrait);
    }
}

public class AudienceManager : MonoBehaviour {
    public float audienceSpawnTime = 5f;
    public Vector2 leftSpawnContainerOrigin;
    public Vector2 leftSpawnContainerSize;
    public Vector2 rightSpawnContainerOrigin;
    public Vector2 rightSpawnContainerSize;

    // Customizations.
    public GameObject[] hairOptions;
    public GameObject[] eyeOptions;

    public float audienceMoveSpeed = 1.0f;
    float m_audienceSpawnRemaining = 0f;
    int m_spawnCounter = 0;

    Dictionary<string, AudienceType> m_audienceTypes;
    public Dictionary<string, AudienceType> AudienceTypes {
        get { return m_audienceTypes; }
    }

    void Awake() {
        m_audienceTypes = new Dictionary<string, AudienceType> ();
    }

    void Start() {
        m_audienceTypes.Clear ();
        m_audienceSpawnRemaining = audienceSpawnTime;
    }

    void Update() {
        m_audienceSpawnRemaining -= Time.deltaTime;
        if (m_audienceSpawnRemaining <= 0f) {

            bool walkLeft = (Random.Range (0, 2) % 2 == 0);
            if (walkLeft) {
                GenerateAudienceMember (leftSpawnContainerOrigin, leftSpawnContainerSize);    
            }
            else {
                GenerateAudienceMember (rightSpawnContainerOrigin, rightSpawnContainerSize);
            }

            m_audienceSpawnRemaining = audienceSpawnTime;
        }
    }

    public void GenerateAudienceMembers(int numMembers, Vector2 containerOrigin, Vector2 containerSize) {
        for (int i = 0; i < numMembers; i++) {
            GenerateAudienceMember (containerOrigin, containerSize);
        }
    }
        
    public void GenerateAudienceMember(Vector2 containerOrigin, Vector2 containerSize) {
        Dictionary<string, float> interests = GenerateInterests (GameManager.Instance.Traits);
        string name = "";

        // Determine what type of audience member to generate.
        string primaryInterest = "";
        float primaryInterestValue = -1f;
        foreach (KeyValuePair<string, float> interest in interests) {
            if (interest.Value > primaryInterestValue) {
                primaryInterest = interest.Key;
                primaryInterestValue = interest.Value;
            }
        }

        // Create the audience instance.
        AudienceMember newMember = null;
        foreach (AudienceType type in AudienceTypes.Values) {
            if (type.primaryTrait == primaryInterest) {
                newMember = Instantiate<AudienceMember> (type.prefab);
                name = type.name;
                break;
            }
        }
        if (newMember == null) {
            Debug.Log ("Unable to generate audience member: Unable to match primary interest '" + primaryInterest + "' with audience type.");
            return;
        }
        newMember.transform.SetParent (transform, true);
        newMember.name = name + " (" + m_spawnCounter + ")";
        newMember.Interests = interests;
        newMember.walkSpeed *= Random.Range (audienceMoveSpeed * 0.7f, audienceMoveSpeed * 1.3f);

        float scale = Random.Range (0.85f, 1.15f);
        newMember.transform.localScale = new Vector3 (newMember.transform.localScale.x * scale, newMember.transform.localScale.y * scale, 1f);

        // Choose location
        float minX = containerOrigin.x - containerSize.x / 2f;
        float maxX = containerOrigin.x + containerSize.x / 2f;
        float minY = containerOrigin.y - containerSize.y / 2f;
        float maxY = containerOrigin.y + containerSize.y / 2f;
        float x = Random.Range (minX, maxX);
        float y = Random.Range (minY, maxY);
        newMember.transform.position = new Vector3 (x, y, transform.position.z);

        // Choose walking direction
        if (x < GameManager.Instance.player.transform.position.x) {
            newMember.walkingDirection = new Vector2 (1f, 0f);
        }
        else {
            newMember.walkingDirection = new Vector2 (-1f, 0f);
        }

        // Generate eyes.
        if (eyeOptions.Length > 0) {
            int index = Random.Range (0, eyeOptions.Length);
            GameObject newEyes = (GameObject)Instantiate(eyeOptions[index], Vector3.zero, Quaternion.identity);
            newEyes.transform.SetParent (newMember.transform, false);
            newEyes.transform.localScale = Vector3.one;
            newEyes.GetComponent<SpriteRenderer>().sortingLayerName = "Audience";
        }

        // Generate hair
        if (hairOptions.Length > 0) {
            int index = Random.Range (0, hairOptions.Length);
            GameObject newHair = (GameObject)Instantiate(hairOptions[index], Vector3.zero, Quaternion.identity);
            newHair.transform.SetParent (newMember.transform, false);
            newHair.transform.localScale = Vector3.one;
            newHair.GetComponent<SpriteRenderer>().sortingLayerName = "Audience";
        }

        m_spawnCounter++;
    }

    public AudienceMember[] GetAudienceMembers() {
        return GetComponentsInChildren<AudienceMember>();
    }

    public void RemoveAllAudienceMembers() {
        AudienceMember[] members = GetAudienceMembers ();
        for (int i = 0; i < members.Length; ++i) {
            Destroy (members [i].gameObject);
        }
    }

    Dictionary<string, float> GenerateInterests(List<string> traits) {
        int traitsPerSide = traits.Count / 2;
        List<float> traitValues = new List<float> ();

        // Generate an even number of positive and negative traits per side
        for (int i = 0; i < traitsPerSide; ++i) {
            float value = Random.Range (-1f, 0f);
            traitValues.Add (value);
        }
        for (int i = 0; i < traitsPerSide; ++i) {
            float value = Random.Range (0, 1f);
            traitValues.Add (value);
        }
        while (traitValues.Count < traits.Count) {
            float value = Random.Range (-1f, 0f);
            traitValues.Add (value);
        }

        // Shuffle the list of trait values.
        // Taken from: http://stackoverflow.com/a/1262619
        System.Random rng = new System.Random();  
        int n = traitValues.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            float value = traitValues[k];
            traitValues[k] = traitValues[n];  
            traitValues[n] = value;  
        }

        // Assign the values to the interests.
        Dictionary<string, float> interests = new Dictionary<string, float>();
        for (int i = 0; i < traits.Count; ++i) {
            // @TODO Normalize the trait values while assigning them...
            interests [traits [i]] = traitValues [i];
        }
        return interests;
    }
}
