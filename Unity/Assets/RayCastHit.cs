using UnityEngine;
using System.Collections;
using Meta;
using Thalmic.Myo;
public class RayCastHit : MonoBehaviour {
	public GameObject metaWorld;
	public ThalmicMyo myo;
	public TapHandler tapHandler;
	public GameObject display;
	private Thalmic.Myo.Pose _lastPose = Pose.Unknown;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		myo.Unlock (UnlockType.Hold);
		if (myo.pose != _lastPose) {
			_lastPose = myo.pose;

			if (myo.pose == Pose.Fist) {
				UnityEngine.Vector3 point = findCollisionPosition();
				if (point != UnityEngine.Vector3.zero)
				{
					tapHandler.handleTap(point.x, point.y);
				}
			}
		}
	}
	UnityEngine.Vector3 findCollisionPosition() {
		Plane plane = new Plane(new UnityEngine.Vector3 (0f, 0f, -1f), new UnityEngine.Vector3 (0f, 0f, .4f));
		Ray ray = new Ray(new UnityEngine.Vector3(0,0,0), metaWorld.transform.forward);
		float distance;
		if (plane.Raycast(ray, out distance)) {
			return distance * metaWorld.transform.forward;
				}
				else {
			return UnityEngine.Vector3.zero;
				}
	}
}
