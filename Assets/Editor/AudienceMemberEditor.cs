using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor (typeof (AudienceMember))]
public class AudienceMemberEditor : Editor {

    public override void OnInspectorGUI() {
        AudienceMember member = target as AudienceMember;
        DrawDefaultInspector();
           
        EditorGUILayout.LabelField("Current Interest", member.CurrentInterest.ToString());
        if (member.Interests != null) {
            EditorGUILayout.LabelField("My Interests");
            foreach (KeyValuePair<string, float> interest in member.Interests) {
                EditorGUILayout.LabelField(interest.Key, interest.Value.ToString());
            }    
        }
    }
}
