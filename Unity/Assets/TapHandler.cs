using UnityEngine;
using System.Collections;
using Thalmic.Myo;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


public class TapHandler : MonoBehaviour {
	public GameObject display;
	public ThalmicMyo myo;

	public void handleTap(float x, float y)
	{
		var renderer = display.renderer;
		float width = renderer.bounds.size.x;
		float height = renderer.bounds.size.y;
		float dispx = renderer.bounds.center.x;
		float dispy = renderer.bounds.center.y;
		float relativex = Mathf.Abs(x - (dispx - width/2));
		float relativey = Mathf.Abs(y - (dispy + height/2));
		float percentX = relativex / width;
		float percentY = relativey / height;
		if (percentX > 1 || percentY > 1 || percentX < 0 || percentY < 0) {
			return;
		}
		myo.Vibrate (Thalmic.Myo.VibrationType.Short);

		//WWWForm form = new WWWForm();
		// Upload to a cgi script
		string postData = "\"" + percentX + "," + percentY + "\"";
		/*
		form.AddField ("data", data);
		WWW w = new WWW("http://metacast.firebaseio.com/click", form);
		yield return w;

		*/
		ServicePointManager.ServerCertificateValidationCallback = Validator;
		
		// Create a request using a URL that can receive a post. 
		var request = (HttpWebRequest)WebRequest.Create("https://metacast.firebaseio.com/click.json");
		
		var data = Encoding.ASCII.GetBytes(postData);
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
		
		//var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
	}
	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
	                              SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}
}
