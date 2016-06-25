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
    public Vector2 spawnContainerOrigin;
    public Vector2 spawnContainerSize;
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
            GenerateAudienceMember ("Spawned", spawnContainerOrigin, spawnContainerSize);
            m_audienceSpawnRemaining = audienceSpawnTime;
        }
    }

    public void GenerateAudienceMembers(int numMembers, Vector2 containerOrigin, Vector2 containerSize) {
        for (int i = 0; i < numMembers; i++) {
            GenerateAudienceMember ("Audience", containerOrigin, containerSize);
        }
    }
        
    public void GenerateAudienceMember(string name, Vector2 containerOrigin, Vector2 containerSize) {
        Dictionary<string, float> interests = GenerateInterests (GameManager.Instance.Traits);

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
        newMember.walkSpeed *= Random.Range (0.8f, 1.2f);

        float minX = containerOrigin.x - containerSize.x / 2f;
        float maxX = containerOrigin.x + containerSize.x / 2f;
        float minY = containerOrigin.y - containerSize.y / 2f;
        float maxY = containerOrigin.y + containerSize.y / 2f;
        float x = Random.Range (minX, maxX);
        float y = Random.Range (minY, maxY);
        newMember.transform.position = new Vector3 (x, y, transform.position.z);
        if (x < 0) {
            newMember.walkingDirection = new Vector2 (1f, 0f);
        }
        else {
            newMember.walkingDirection = new Vector2 (-1f, 0f);
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
            float value = Random.Range (-1f, 1f);
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
