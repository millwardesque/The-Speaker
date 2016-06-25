using UnityEngine;
using System.Collections;

public class SpeechConcept {
    public string name;
    public string speech;
    public string trait;
    public float value;

    public SpeechConcept(string name, string speech, string trait, float value) {
        this.name = name;
        this.speech = speech;
        this.trait = trait;
        this.value = value;
    }
}
