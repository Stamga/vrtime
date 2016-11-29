
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ApplicationManager : SingletonMonoBehaviour<ApplicationManager> {
	public Text canvasTextTag;

	private float currentSpeed = 500;
	private Quaternion prevRotation;
	private GameObject camera;

	private string message = "";
	private bool displayInfo = false; 
	private bool firstMessage = true;
	private float currentDelay = 0;
	private int objectDelay = 3; 
	private int initialDelay = 7;
	Dictionary<string, string> clickableItems = new Dictionary<string, string>();

	void Start() {
		camera = GameObject.Find ("MainCamera");

		canvasTextTag = GameObject.Find ("Text").GetComponent<Text> ();
		canvasTextTag.color = Color.clear;

		prevRotation = camera.transform.rotation;

		clickableItems.Add("ClickableCube", "This is a cube!");
		FadeText ();
		displayInfo = true;
		message = "You are in coma.\n\nFind MEMORIES  to latch onto, to avoid losing conciousness.\n\nMoving takes TIME.";
	}

	void Update () {
		FadeText ();
		TapOnObject ();
		AnimateObjects ();
		DetectCameraMovement ();
	}

	void AnimateObjects () {
		GameObject fan = GameObject.Find ("CeilingFan");
		fan.transform.Rotate (0,0 ,currentSpeed);

		GameObject handHours = GameObject.Find ("HandHours");
		handHours.transform.Rotate (0,currentSpeed/1000,0);

		GameObject handSeconds = GameObject.Find ("HandSeconds");
		handSeconds.transform.Rotate (0,currentSpeed/150,0);

		GameObject heartMonitor = GameObject.Find ("HeartBeatGraph");
		double heartPosition = heartMonitor.transform.localPosition.x;
		if(heartPosition > 0) {
			heartMonitor.transform.Translate (Vector3.left * currentSpeed/200);
		}else {
			heartMonitor.transform.localPosition = new Vector3(1.7f,0,0);
		}
	}

	void DetectCameraMovement (){
		float angle = Quaternion.Angle(prevRotation, camera.transform.rotation);
		currentSpeed = angle*50+200*Time.deltaTime;
		prevRotation = camera.transform.rotation;
	}

	void TapOnObject () {
		if (Raycaster.getInstance ().anythingHitByRay ()) {
			GameObject objHitByRay = Raycaster.getInstance ().getObjectHitByRay ();
			if(clickableItems.TryGetValue(objHitByRay.name, out message))
			{
				displayInfo = true;
			}
			else
			{
				displayInfo = false;
			}
		} else {
			displayInfo = false;
		}
	}

	void FadeText () {
		if(displayInfo) {
			canvasTextTag.text = message;
			canvasTextTag.color = Color.Lerp (canvasTextTag.color, Color.white, 255);
			currentDelay = Time.time;
		}
		else if(firstMessage && Time.time - currentDelay >= initialDelay)  {
			canvasTextTag.color = Color.Lerp (canvasTextTag.color, Color.clear, 10 * Time.deltaTime);
			firstMessage = false;
		}
		else if(!firstMessage && Time.time - currentDelay >= objectDelay)  {
			canvasTextTag.color = Color.Lerp (canvasTextTag.color, Color.clear, 10 * Time.deltaTime);
		}
	}
}