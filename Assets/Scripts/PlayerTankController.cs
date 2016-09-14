using UnityEngine;
using System.Collections;

public class PlayerTankController : MonoBehaviour {	
	private float 		MaxHP			= 1000;
	public float		HP				= 1000;
	public float		MaxPower 		= 1000;
	public float		RotatingSpeed 	= 0.5f;
	public bool 		enableControl 	= true;
	public float		ROF 			= 2.2f;
	public float		Firepower		= 200.0f;
	public Transform	BulletSpawner;
	public Transform	BulletPrefab;	
	public Transform	ShotPrefab;
	public Transform	FirePrefab;
	public AudioClip	ShotSound;
	public AudioClip	MachineGunSound;
	public AudioClip	EngineSound;
	public AudioClip	Fire;
	public Texture2D	HPBar;
	private Transform 	me;
	private Vector3		WayDirection;
	private Vector3		AllwaysForward;
	private float		power = 0;	
	
	private bool 		forcing;
	
	private bool		CanShot = false;	
	private float		timer;
	
	void Start () {
		me = transform;
	}
	void Awake(){
		GetComponent<AudioSource>().PlayOneShot(EngineSound);
		GetComponent<AudioSource>().loop = true;
	}
	
	void Hit (float damage){
		if (HP-damage <= 0){
			HP = 0;
			enableControl = false;
			GameObject.Instantiate(FirePrefab,me.position,Quaternion.identity);
			GetComponent<AudioSource>().PlayOneShot(Fire);			
		} else HP -= damage;
	}
	
	void FixedUpdate () {
		forcing = false;
		if (enableControl) {			
			if (Input.GetKey(KeyCode.W)){
				power = 50000;
				me.GetComponent<Rigidbody>().drag = 0;
			}
			if (Input.GetKey(KeyCode.S)){
				power = -50000; 
				me.GetComponent<Rigidbody>().drag = 0;
			}
			if (Input.GetKey(KeyCode.W)){
				power = power + Input.GetAxis("Vertical") * 20;
				power = Mathf.Clamp(power, -MaxPower, MaxPower);
				forcing = true;
			} 
			if (Input.GetKey(KeyCode.S)){
				power = power + Input.GetAxis("Vertical") * 20;
				power = Mathf.Clamp(power, -MaxPower, MaxPower);
				forcing = true;
			}
			if (Input.GetKey(KeyCode.A)){
				me.localEulerAngles = new Vector3(me.localEulerAngles.x,me.localEulerAngles.y - RotatingSpeed, me.localEulerAngles.z);			
			}
			if (Input.GetKey(KeyCode.D)){
				me.localEulerAngles = new Vector3(me.localEulerAngles.x,me.localEulerAngles.y + RotatingSpeed, me.localEulerAngles.z);			
			}
			
			if (!CanShot) {
				if (timer + Time.deltaTime >= ROF){
					timer = 0;
					CanShot = true;	
				} else timer += Time.deltaTime;
			} 	
			if (CanShot){
				if (Input.GetMouseButtonDown(0)){
					Instantiate(ShotPrefab, BulletSpawner.position, BulletSpawner.rotation);
					Transform bullet = Instantiate(BulletPrefab, BulletSpawner.position, BulletSpawner.rotation) as Transform;
					bullet.transform.GetComponent<Rigidbody>().AddForce(BulletSpawner.TransformDirection(Vector3.forward * Firepower),ForceMode.Impulse);
					GetComponent<AudioSource>().PlayOneShot(ShotSound);
					CanShot = false;
				}
			}
			if (Input.GetMouseButtonDown(1)){
				GetComponent<AudioSource>().PlayOneShot(MachineGunSound);
			}
			
		}
		if (!forcing){				
			me.GetComponent<Rigidbody>().drag = 1;
			if (power > 500){
				power -= power * Time.deltaTime * 3;
			} else if (power < -500){
				power -= power * Time.deltaTime * 3;
			} else power = 0;
		}
		
		RaycastHit hit;
		AllwaysForward = me.TransformDirection(Vector3.forward);
		AllwaysForward = new Vector3(AllwaysForward.x, 0, AllwaysForward.z);
		WayDirection = me.TransformDirection(Vector3.forward * AllwaysForward.magnitude * power);	
		if (Physics.Raycast(new Vector3(me.position.x, me.position.y+0.2f, me.position.z), me.TransformDirection(Vector3.down),out hit, 1.0f)){		
			if (hit.transform.tag.Equals("Ground")){				
				me.GetComponent<Rigidbody>().AddForce(WayDirection, ForceMode.Force);		
			}
		}						
	}
	
	void OnGUI(){
		GUI.Label(new Rect(10,10,20,20),"HP:   "+HP);
		GUI.DrawTexture(new Rect (20,10, (200*HP)/MaxHP,30),HPBar,ScaleMode.StretchToFill,true);
		/*
		GUI.Label(new Rect(20,20,250,20),"WayDirection "+WayDirection);
		GUI.Label(new Rect(20,40,200,20),"speed scale "+AllwaysForward.magnitude);
		GUI.Label(new Rect(20,60,200,20),"Way length "+WayDirection.magnitude);
		GUI.Label(new Rect(20,80,200,20),"power "+power);
		*/
	}	
}
