
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ApplicationManager : SingletonMonoBehaviour<ApplicationManager> {
	public Text canvasTextTag;

	private Quaternion prevRotation;
	private GameObject camera;
	private GameObject cameraParent;

	private GameObject fan;
	private GameObject handHours;
	private GameObject handSeconds;
	private GameObject heartMonitor;

	private AudioSource dropSound;
	private AudioSource clockSound;
	private AudioSource heartSound;
	private AudioSource heartDeadSound;
	private AudioSource fanSound;

	private Light light;
	private Light light2;

	private string message = "";

	private float speedH = 2.0f;
	private float speedV = 2.0f;
	private float yaw = 0.0f;
	private float pitch = 0.0f;

	private bool displayInfo = false; 
	private bool firstMessage = true;
	private bool isHealing = false;
	private bool gameStarted = false;

	private float currentSpeed = 100;
	private float currentLife = 100;
	private float currentDelay = 0;

	private int healingProgress = 0;
	private int objectDelay = 8; 
	private int initialDelay = 20;

	Dictionary<string, string> clickableItems = new Dictionary<string, string>();

	private ScreenFaderSphere screenFader;

	void Start() {
		screenFader = GameObject.Find("Sphere_Inv").GetComponent<ScreenFaderSphere> ();

		canvasTextTag = GameObject.Find ("Text").GetComponent<Text> ();
		canvasTextTag.color = Color.clear;

		camera = GameObject.Find ("MainCamera");
		cameraParent = GameObject.Find ("MainCameraParent");

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
		if (!gameStarted) {

			dropSound = GameObject.Find ("DropSound").GetComponent<AudioSource> ();
			clockSound = GameObject.Find ("ClockSound").GetComponent<AudioSource> ();
			heartSound = GameObject.Find ("HeartSound").GetComponent<AudioSource> ();
			heartDeadSound = GameObject.Find ("HeartDeadSound").GetComponent<AudioSource> ();
			fanSound = GameObject.Find ("FanSound").GetComponent<AudioSource> ();

			fan = GameObject.Find ("CeilingFan");
			handHours = GameObject.Find ("HandHours");
			handSeconds = GameObject.Find ("HandSeconds");
			heartMonitor = GameObject.Find ("HeartBeatGraph");

			light = GameObject.Find ("MainLight").GetComponent<Light> ();
			light2 = GameObject.Find ("SideLight").GetComponent<Light> ();
			gameStarted = true;
		} else {
			DeathProgression ();
			AnimateObjects ();
			DetectCameraMovement ();
			RotateCameraMouse ();
			TapOnObject ();
			FadeText ();
		}
	}

	void DeathProgression () {
		if (isHealing) {
			currentLife += 0.02f;
			healingProgress++;
		}if(currentLife > 60) {
			currentLife -= 0.01f;
		}else {
			currentLife -= 0.01f;
			screenFader.fadeToBlack ();
			heartDeadSound.Play();
		}

		if (healingProgress > 500) {
			healingProgress = 0;
			isHealing = false;
		}

		cameraParent.transform.localPosition = new Vector3(-729.82f,-373.55f+Mathf.Abs(currentLife-100)*10/100,0.26f);

		light.intensity = 2.33f*currentLife/100;
		light2.intensity = 0.6f*currentLife/100;
	}

	void AnimateObjects () {
		fan.transform.Rotate (0,0 ,currentSpeed*7);
		handHours.transform.Rotate (0,currentSpeed/100,0);
		handSeconds.transform.Rotate (0,currentSpeed/10,0);
		double heartPosition = heartMonitor.transform.localPosition.x;
		if(heartPosition > 0) {
			heartMonitor.transform.Translate (Vector3.left * ((currentSpeed*200)*Time.deltaTime)/200);
		}else {
			heartMonitor.transform.localPosition = new Vector3(1.7f,0,0);
		}
	}

	void DetectCameraMovement (){
		float angle = Quaternion.Angle(prevRotation, camera.transform.rotation);
		currentSpeed = angle/50+1-Mathf.Abs(currentLife-100)/50;
		prevRotation = camera.transform.rotation;
		dropSound.pitch = currentSpeed;
		fanSound.pitch = currentSpeed;
		clockSound.pitch = currentSpeed;
		heartSound.pitch = currentSpeed;
	}

	void RotateCameraMouse () {
		yaw += speedH * Input.GetAxis("Mouse X");
		pitch -= speedV * Input.GetAxis("Mouse Y");

		camera.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
	}

	void TapOnObject () {
		if (Raycaster.getInstance ().anythingHitByRay ()) {
			GameObject objHitByRay = Raycaster.getInstance ().getObjectHitByRay ();
			if(clickableItems.TryGetValue(objHitByRay.name, out message))
			{
				displayInfo = true;
				if (!isHealing) {
					isHealing = true;
				}
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