
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ApplicationManager : SingletonMonoBehaviour<ApplicationManager> {

	public Text canvasTextTag;

	private float currentSpeed = 500;

	private string message = "";
	private bool displayInfo = false; 
	private bool firstMessage = true;
	private float currentDelay = 0;
	private int objectDelay = 3; 
	private int initialDelay = 7;
	Dictionary<string, string> clickableItems = new Dictionary<string, string>();

	void Start() {
		canvasTextTag = GameObject.Find ("Text").GetComponent<Text> ();
		canvasTextTag.color = Color.clear;

		clickableItems.Add("ClickableCube", "This is a cube!");
		FadeText ();
		displayInfo = true;
		message = "You are in coma.\n\nFind MEMORIES  to latch onto, to avoid losing conciousness.\n\nMoving takes TIME.";
	}

	void Update () {
		FadeText ();
		TapOnObject ();
		AnimateObjects ();
	}

	void AnimateObjects () {
		GameObject cube = GameObject.Find ("CeilingFan");
		cube.transform.Rotate (0,0 ,currentSpeed * Time.deltaTime);
	}

	void DetectCameraMovement (){
		GameObject camera = GameObject.Find ("MainCamera");
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