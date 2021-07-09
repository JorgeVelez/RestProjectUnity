using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float multiplier = 50f;
	//public float multiplier2 = 30f;
	
	void Update () {
		//transform.Rotate(new Vector3(.2f, 0, 0) * Time.deltaTime * multiplier);
		//transform.Rotate(new Vector3(0, .6f, 0) * Time.deltaTime * multiplier2);
		transform.Rotate(Vector3.fwd*Time.deltaTime * multiplier);
	}
}
