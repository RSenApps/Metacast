using UnityEngine;
using System.Collections;

public class UpdateScreenshot : MonoBehaviour {
	public string url = "https://metacast.firebaseio.com/image.json";
	IEnumerator Start() {
		string lastEncoded = "";

		while (true) {
						WWW www = new WWW (url);
						yield return www;
						string imageEncoded = www.text.Split ('"') [1].Replace ("\\n", "");
						if (imageEncoded != lastEncoded)
						{
							lastEncoded = imageEncoded;
							byte[] Bytes = System.Convert.FromBase64String (imageEncoded);
							Texture2D tex = new Texture2D (960, 540);
							tex.LoadImage (Bytes);
							renderer.material.mainTexture = tex;
						}
				}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
