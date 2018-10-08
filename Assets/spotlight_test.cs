using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spotlight_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("Ciao");
		GetComponent<Light>().intensity = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
