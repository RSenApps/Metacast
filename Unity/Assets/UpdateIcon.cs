using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class UpdateIcon : MonoBehaviour {
	public string url = "https://metacast.firebaseio.com/image.json";

	string lastEncoded = "";
	IEnumerator Start() {
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
	// Update is called once per frame
	void Update () {
		
	}
	public IEnumerator updateTexture()
	{
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
	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
	                              SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}
}
