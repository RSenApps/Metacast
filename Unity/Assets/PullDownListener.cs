using UnityEngine;
using System.Collections;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text;
using Meta;

public class PullDownListener : MetaBehaviour {
	public int iconIndex;
	public IconUpdateService iconUpdater;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}


	IEnumerator OnRelease()
	{
		Vector3 pos = transform.position;
		if (pos.y < .1) {
			pos.y = .15f;
			transform.position = pos;

			WWW www = new WWW ("https://metacast.firebaseio.com/pkg" + iconIndex + ".json");
			yield return www;
			string pkg = www.text;
			
			
			ServicePointManager.ServerCertificateValidationCallback = Validator;
			
			// Create a request using a URL that can receive a post. 
			var request = (HttpWebRequest)WebRequest.Create("https://metacast.firebaseio.com/app.json");
			
			var data = Encoding.ASCII.GetBytes(pkg);
			request.UserAgent = "test.net";
			request.Accept = "application/json";
			request.Method = "PUT";
			request.ContentType = "raw";
			request.ContentLength = data.Length;
			
			using (var stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}
			var response = (HttpWebResponse)request.GetResponse();
			pos.y = .15f;
			transform.position = pos;
			StartCoroutine(iconUpdater.updateIcons());

		}
	}
	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
	                              SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}
}
