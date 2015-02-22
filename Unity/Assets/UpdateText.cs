using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UpdateText : MonoBehaviour {
	// Use this for initialization
	public Text text;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Time: " + Time.time;
	}
}
