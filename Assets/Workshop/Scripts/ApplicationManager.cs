
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ApplicationManager : SingletonMonoBehaviour<ApplicationManager> {
	public Text canvasTextTag;

	private float currentSpeed = 100;
	private float currentLife = 100;
	private Quaternion prevRotation;
	private GameObject camera;
	private GameObject cameraParent;

	private GameObject fan;
	private GameObject handHours;
	private GameObject handSeconds;
	private GameObject heartMonitor;

	private string message = "";
	private bool displayInfo = false; 
	private bool firstMessage = true;
	private float currentDelay = 0;
	private int objectDelay = 5; 
	private int initialDelay = 15;
	Dictionary<string, string> clickableItems = new Dictionary<string, string>();

	private ScreenFaderSphere screenFader;

	void Start() {
		screenFader = GameObject.Find("Sphere_Inv").GetComponent<ScreenFaderSphere> ();

		GameObject fan = GameObject.Find ("CeilingFan");
		GameObject handHours = GameObject.Find ("HandHours");
		GameObject handSeconds = GameObject.Find ("HandSeconds");
		GameObject heartMonitor = GameObject.Find ("HeartBeatGraph");

		camera = GameObject.Find ("MainCamera");
		cameraParent = GameObject.Find ("MainCameraParent");

		canvasTextTag = GameObject.Find ("Text").GetComponent<Text> ();
		canvasTextTag.color = Color.clear;

		prevRotation = camera.transform.rotation;

		clickableItems.Add("ClickableDrawing", "Your son’s first drawing. You have cherished this image for as long as you can remember.");
		clickableItems.Add("ClickablePhoto", "Your beautiful family. They have brought you so much joy in your life and have been there whenever you needed them. ");
		clickableItems.Add("ClickableScarf", "Your sister’s scarf. You gave her this scarf as a gift when she graduated university.  She loves you and how you supported her through her difficult time in school.");
		clickableItems.Add("ClickableRing", "Your wedding band. Shouldn’t it be on your finger?  No, wait- they have to remove all jewelry before surgery. You and your spouse are so in love, even after all these years. ");
		clickableItems.Add("ClickableMri", "A scan of your brain. To think just a blink of the eye ago it was perfectly fine. Now it is uncertain whether it will allow you to keep living.");
		FadeText ();
		displayInfo = true;
		message = "Your head hit the ground. The doctors took you into surgery right away. They managed to stop the bleeding in your brain but you never woke up. You slipped into a coma.\n\n The doctors think it is only a matter of time before you pass. Your family is in deep agony. Some have hope you will survive, others are starting to accept you will never be with them again. Maybe if you keep holding on and find memories to give you strength to keep going, you will wake from the coma. ";
	}

	void Update () {
		FadeText ();
		TapOnObject ();
		AnimateObjects ();
		DeathProgression ();
		DetectCameraMovement ();
	}

	void DeathProgression () {
		if(currentLife>50) {
			currentLife -= 0.01f;
		}else {
			currentLife -= 0.01f;
			screenFader.fadeToBlack ();
		}
		cameraParent.transform.localPosition = new Vector3(-729.82f,-373.55f+Mathf.Abs(currentLife-100)*10/100,0.26f);

		Light light = GameObject.Find ("MainLight").GetComponent<Light> ();
		Light light2 = GameObject.Find ("MainLight").GetComponent<Light> ();
		light.intensity = currentLife / 25 * 2;
		light2.intensity = currentLife / 50;
	}

	void AnimateObjects () {
		fan.transform.Rotate (0,0 ,currentSpeed);
		handHours.transform.Rotate (0,currentSpeed/1000,0);
		handSeconds.transform.Rotate (0,currentSpeed/150,0);
		double heartPosition = heartMonitor.transform.localPosition.x;
		if(heartPosition > 0) {
			heartMonitor.transform.Translate (Vector3.left * currentSpeed/200);
		}else {
			heartMonitor.transform.localPosition = new Vector3(1.7f,0,0);
		}
	}

	void DetectCameraMovement (){
		float angle = Quaternion.Angle(prevRotation, camera.transform.rotation);
		currentSpeed = angle*25+200*Time.deltaTime-Mathf.Abs(currentLife-100)/10;
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