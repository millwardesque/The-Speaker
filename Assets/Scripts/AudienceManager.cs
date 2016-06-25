using UnityEngine;
using System.Collections;

public class AudienceManager : MonoBehaviour {
    public AudienceMember audiencePrefab;
    public float audienceSpawnTime = 5f;
    public Vector2 spawnContainerOrigin;
    public Vector2 spawnContainerSize;
    float m_audienceSpawnRemaining = 0f;
    int m_spawnCounter = 0;

    void Start() {
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
        AudienceMember newMember = Instantiate<AudienceMember> (audiencePrefab);
        newMember.transform.SetParent (transform, true);
        newMember.name = name + " (" + m_spawnCounter + ")";
        newMember.GenerateTraits (GameManager.Instance.Traits);

        float minX = containerOrigin.x - containerSize.x / 2f;
        float maxX = containerOrigin.x + containerSize.x / 2f;
        float minY = containerOrigin.y - containerSize.y / 2f;
        float maxY = containerOrigin.y + containerSize.y / 2f;
        float x = Random.Range (minX, maxX);
        float y = Random.Range (minY, maxY);
        newMember.transform.position = new Vector3 (x, y, transform.position.z);
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
}
