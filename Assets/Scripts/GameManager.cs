using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MessageManager.Instance.AddListener ("CountdownTimerElapsed", OnCountdownTimerElapsed);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCountdownTimerElapsed(Message message) {
        Debug.Log ("Countdown expired!");
    }
}
