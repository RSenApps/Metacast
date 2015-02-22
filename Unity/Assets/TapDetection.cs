using UnityEngine;
using System.Collections;
using Meta;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


public class TapDetection : MetaBehaviour {
	public GameObject phoneDisplay;
	public ThalmicMyo myo;
	public TapHandler handler;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	

	void OnTouchEnter() {
		handler.handleTap (Meta.Hands.right.pointer.position.x, Meta.Hands.right.pointer.position.y);

	}

}
