using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

[Serializable]
// In: command
class SonoffCmd {
    public string id = "000";
	public string type = "generic";
	public string cmd = "NONE"; 
}

// Out: feedback
[Serializable]
class SonoffFbk {
	public string id = "000";
	public string type = "generic";
	public string state = "unknown"; 
}


public class mqttTest : MonoBehaviour {
	private MqttClient client;

	private bool refreshState = false;
	private bool lampEnableFeedbackState = false;

	public Light lamp;
	
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
		
		// listen to the state "out" topic
		client.Subscribe(new string[] { "sonoff17/out" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

	}
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		SonoffFbk lampOut = JsonUtility.FromJson<SonoffFbk>(System.Text.Encoding.UTF8.GetString(e.Message));
		Debug.Log("Feedback lamp state: " + lampOut.state );
		lampEnableFeedbackState = string.Compare(lampOut.state,"ON") == 0;
		refreshState = true;
	} 

	void OnGUI(){
		if ( GUI.Button (new Rect (20,40,80,20), "Level 1")) {
			// SonoffIn lampIn = new SonoffIn();
			// lampIn.cmd = "ON";
			// Debug.Log("sending cmd " + lampIn.cmd + "...");
			// string jsonMsg = JsonUtility.ToJson(lampIn);
			// write the new state on the "in" topic
			//client.Publish("sonoff17/in", System.Text.Encoding.UTF8.GetBytes(jsonMsg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			
			// DEBUG FIXME just for debug, send to in topic to close the loop
			SonoffFbk lampOut = new SonoffFbk();
			lampOut.state = "ON";
			Debug.Log("sending cmd " + lampOut.state + "...");
			string jsonMsg = JsonUtility.ToJson(lampOut);
			client.Publish("sonoff17/out", System.Text.Encoding.UTF8.GetBytes(jsonMsg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			
			Debug.Log("sent");
		}
		
	}
	// Update is called once per frame
	void Update () {
		if (refreshState) {
			lamp.enabled = lampEnableFeedbackState;
			Debug.Log("update lamp state: "+lamp.enabled);
			refreshState = false;
		}
	}
}
