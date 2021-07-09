using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerLocationService : MonoBehaviour {

	public GeoPoint loc = new GeoPoint();
	[HideInInspector]
	public float trueHeading;
	public bool locServiceIsRunning = false;
	public int maxWait = 30; // seconds
	private float locationUpdateInterval = 3f; // seconds
	private double lastLocUpdate = 0.0; //seconds

	private int maxLocationCalls=20;

	public class PosEvent : UnityEvent<GeoPoint>{}
	public PosEvent onGeoPosEvent;

void Start() {
		if (onGeoPosEvent == null)
            onGeoPosEvent = new PosEvent();
	}

	public void StartLocationService() {
		Debug.Log ("Player Loc started.");

		StartCoroutine ("_StartLocationService");
		StartCoroutine ("RunLocationService");
	}

	public void StopLocationService() {
		Debug.Log ("Player Loc stopped.");
		StopCoroutine ("_StartLocationService");
		StopCoroutine ("RunLocationService");
		//StopAllCoroutines();
		Input.location.Stop();
	}

	public IEnumerator _StartLocationService()
	{
		if (!Input.location.isEnabledByUser) {
			Debug.Log ("Locations is not enabled.");
			loc.setLatLon_deg (0.0f, 0.0f); 
			onGeoPosEvent.Invoke(loc);
			locServiceIsRunning = true;
			yield break;
		}

		Input.location.Start();
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		if (maxWait < 1)
		{
			print("Locations services timed out");
			yield break;
		}

		if (Input.location.status == LocationServiceStatus.Failed)
		{
			print("Location services failed");
			yield break;
		} else if (Input.location.status == LocationServiceStatus.Running){
			loc.setLatLon_deg (Input.location.lastData.latitude, Input.location.lastData.longitude);
			//Debug.Log ("Location: " + Input.location.lastData.latitude.ToString ("R") + " " + Input.location.lastData.longitude.ToString ("R") + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			onGeoPosEvent.Invoke(loc);

			locServiceIsRunning = true;
			lastLocUpdate = Input.location.lastData.timestamp;
		} else {
			print ("Unknown Error!");
		}
		Debug.Log (loc.ToString());
	}

	public IEnumerator RunLocationService()
	{
		double lastLocUpdate = 0.0;
		int counterCalls=0;
		while (true) {
			if (lastLocUpdate != Input.location.lastData.timestamp) {
				loc.setLatLon_deg (Input.location.lastData.latitude, Input.location.lastData.longitude);
				trueHeading = Input.compass.trueHeading;
				Debug.Log ("Location: " + Input.location.lastData.latitude.ToString ("R") + " " + Input.location.lastData.longitude.ToString ("R") + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
				//locServiceIsRunning = true;
				lastLocUpdate = Input.location.lastData.timestamp;
			}
			counterCalls++;
			if(counterCalls>maxLocationCalls)
				StopLocationService();
			yield return new WaitForSeconds(locationUpdateInterval);
		}
	}
}