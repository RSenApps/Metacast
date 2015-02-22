using UnityEngine;
using System.Collections;

public class LockToVertical : MonoBehaviour {
	private float initialx;
	private float initialz;
	// Use this for initialization
	void Start () {
		Vector3 pos = transform.position;
		initialx = pos.x;
		initialz = pos.z;
	}

	void Update () {
		Vector3 pos = transform.position;
		pos.x = initialx;
		pos.z = initialz;
		transform.position = pos;

	}
	
}
