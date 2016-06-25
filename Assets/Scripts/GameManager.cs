using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    Player m_player;

	// Use this for initialization
	void Start () {
        MessageManager.Instance.AddListener ("CountdownTimerElapsed", OnCountdownTimerElapsed);
        MessageManager.Instance.AddListener ("ButtonPushed", OnButtonPushed);
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

    void OnCountdownTimerElapsed(Message message) {
        Debug.Log ("Countdown expired!");
    }

    void OnButtonPushed(Message message) {
        Debug.Log (string.Format("Button pushed: {0}", message.data["concept"]));
        m_player.Speak (message.data["concept"].ToString ());
    }
}
