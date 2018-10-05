using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

[Serializable]
class Sonoff { // this is just for test, waiting the real struct from Melita
	public String isOn = "false";
}

public class mqttTest : MonoBehaviour {
	private MqttClient client;
	// Use this for initialization
	void Start () {
		// create client instance 
		// iot.eclipse.org:1883 sonoff17/out sonoff17/in
		Debug.Log("Start...");
		client = new MqttClient("iot.eclipse.org",1883 , false , null ); 

		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		client.Subscribe(new string[] { "sonoff17/out" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

	}
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		Sonoff sonoff = JsonUtility.FromJson<Sonoff>(System.Text.Encoding.UTF8.GetString(e.Message));
		Debug.Log("Lamp is: " + sonoff.isOn );
	} 

	void OnGUI(){
		if ( GUI.Button (new Rect (20,40,80,20), "Level 1")) {
			Debug.Log("sending...");
			Sonoff sonoff = new Sonoff();
			sonoff.isOn = "true";
			string jsonMsg = JsonUtility.ToJson(sonoff);
			client.Publish("sonoff17/out", System.Text.Encoding.UTF8.GetBytes(jsonMsg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			Debug.Log("sent");
		}
	}
	// Update is called once per frame
	void Update () {



	}
}
