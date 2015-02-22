using UnityEngine;
using System.Collections;

public class MusicPlayingMonitor : MonoBehaviour {
	public GameObject uiControls;
	public IEnumerator Start() {
		while (true) {
						WWW www = new WWW ("https://metacast.firebaseio.com/music.json");
						yield return www;
						uiControls.SetActiveRecursively (www.text == "true");
						yield return new WaitForSeconds (2);
				}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
