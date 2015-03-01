using UnityEngine;
using System.Collections;

public class IconUpdateService : MonoBehaviour {
	public UpdateIcon icon0;
	public UpdateIcon icon1;
	public UpdateIcon icon2;
	public UpdateIcon icon3;
	public UpdateIcon icon4;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public IEnumerator updateIcons()
	{

		//hilarious
		yield return new WaitForSeconds (0);

		StartCoroutine(icon0.updateTexture());
		StartCoroutine(icon1.updateTexture());
		StartCoroutine(icon2.updateTexture());
		StartCoroutine(icon3.updateTexture());
		StartCoroutine(icon4.updateTexture());

		yield return new WaitForSeconds (3);
		StartCoroutine(icon0.updateTexture());
		StartCoroutine(icon1.updateTexture());
		StartCoroutine(icon2.updateTexture());
		StartCoroutine(icon3.updateTexture());
		StartCoroutine(icon4.updateTexture());

		//icon4.updateTexture ();

	}
}
