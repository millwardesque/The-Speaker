using UnityEngine;
using System.Collections;

public class AudienceManager : MonoBehaviour {
    public AudienceMember audiencePrefab;

    public void GenerateAudienceMembers(int numMembers, Vector2 containerOrigin, Vector2 containerSize) {
        for (int i = 0; i < numMembers; i++) {
            GenerateAudienceMember ("Audience #" + i, containerOrigin, containerSize);
        }
    }
    public void GenerateAudienceMember(string name, Vector2 containerOrigin, Vector2 containerSize) {
        AudienceMember newMember = Instantiate<AudienceMember> (audiencePrefab);
        newMember.transform.SetParent (transform, true);
        newMember.name = name;
        newMember.GenerateTraits (GameManager.Instance.Traits);

        float minX = containerOrigin.x - containerSize.x / 2f;
        float maxX = containerOrigin.x + containerSize.x / 2f;
        float minY = containerOrigin.y - containerSize.y / 2f;
        float maxY = containerOrigin.y + containerSize.y / 2f;
        float x = Random.Range (minX, maxX);
        float y = Random.Range (minY, maxY);
        newMember.transform.position = new Vector3 (x, y, transform.position.z);
    }
}
