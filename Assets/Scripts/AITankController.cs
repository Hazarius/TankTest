using UnityEngine;
using System.Collections;

public class AITankController : MonoBehaviour {	
	private float 		MaxHP			= 1000;
	public float 		HP				= 1000;
	public float		MaxPower 		= 920;
	public float		RotatingSpeed 	= 0.5f;	
	public float		ROF 			= 1.7f;	
	public float		Firepower 		= 200.0f;
	public Transform	BulletSpawner;
	public Transform	BulletPrefab;	
	public Transform	FirePrefab;
	
	public AudioClip	ShotSound;
	public AudioClip	MachineGunSound;
	public AudioClip	EngineSound;
	public AudioClip	Fire;
	public Texture2D	HPBar;
	private Transform 	Target;
	public float 		DistanceToTarget;
	private Vector3		TargetDirection;
	private Transform 	me;
	
	private Vector3		WayDirection;
	private Vector3		AllwaysForward;
	public float		power = 0;
	public float		WayAnlge;
	private float		timer;	
	private float		timer2;
	private bool		EnableAI = true;
	private bool		CanShot = false;
	private bool		tryRotate = true;
	private bool		trymove = false;
	
	private string		status;
	
	void Start () {
		me = transform;
		Target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Awake(){
		GetComponent<AudioSource>().PlayOneShot(EngineSound);
		GetComponent<AudioSource>().loop = true;
	}
	
	void Hit (float damage){
		if (HP-damage <=0){
			HP = 0;
			EnableAI = false;
			GameObject.Instantiate(FirePrefab,me.position,Quaternion.identity);
			GetComponent<AudioSource>().PlayOneShot(Fire);			
		} else HP -=damage;
	}
	
	void Update () {
		if (EnableAI) {
			if (Target) {				
				DistanceToTarget = (me.position - Target.position).magnitude;
				Vector3 TargetDir = (Target.position - me.position).normalized;
				if (DistanceToTarget > 40){
					WayAnlge = AngleAroundAxis(me.forward, TargetDir, Vector3.up);
					if ((WayAnlge >= -10)&&(WayAnlge <= 10)){
						status = "Bot is running";
						if (power < MaxPower) power += 15;
						power = Mathf.Clamp(power, 0,MaxPower);
					} else {
						status = "Bot change angle";
						power -= 10;
						power = Mathf.Clamp(power, 0,MaxPower);
						if (WayAnlge < 180) me.localEulerAngles = new Vector3(me.localEulerAngles.x, me.localEulerAngles.y + RotatingSpeed, me.localEulerAngles.z);
						else me.localEulerAngles = new Vector3(me.localEulerAngles.x, me.localEulerAngles.y - RotatingSpeed, me.localEulerAngles.z);
					}
					
				} else {				
					RaycastHit targethit;
					status = "Bot check target";
					if (Physics.Raycast(BulletSpawner.position, BulletSpawner.TransformDirection(Vector3.forward), out targethit, 150.0f)) {		
						if (targethit.transform.tag.Equals("Player")) {	
							power = 0;
							if (!CanShot) {
								if (timer + Time.deltaTime >= ROF){
									timer = 0;
									CanShot = true;	
								} else timer += Time.deltaTime;
							} 	
							if (CanShot) {	
								status = "Bot Fire!";
								Transform bullet = Instantiate(BulletPrefab, BulletSpawner.position, BulletSpawner.rotation) as Transform;
								bullet.transform.GetComponent<Rigidbody>().AddForce(BulletSpawner.TransformDirection(Vector3.forward * 100),ForceMode.Impulse);
								GetComponent<AudioSource>().PlayOneShot(ShotSound);
								CanShot = false;
							}
						} else {
							status = "Bot find another path";
							if (timer2 + Time.deltaTime >= 2){
								trymove = !trymove;
								tryRotate = !tryRotate;
								timer2 = 0;
							} else timer2 += Time.deltaTime;
							if (trymove) power += 10;
							if (tryRotate){
								power = 0;
								me.localEulerAngles = new Vector3(me.localEulerAngles.x, me.localEulerAngles.y + RotatingSpeed/2, me.localEulerAngles.z);	
							} 
						}
					} else {
						status = "Bot find another path";
						if (timer2 + Time.deltaTime >= 5){
							trymove = !trymove;
							tryRotate = !tryRotate;
							timer2 = 0;
						} else timer2 += Time.deltaTime;
						if (trymove) {
							power += 30;
							power = Mathf.Clamp(power, 0,MaxPower);
						}
						if (tryRotate){
							power = 0;
							me.localEulerAngles = new Vector3(me.localEulerAngles.x, me.localEulerAngles.y + RotatingSpeed/2, me.localEulerAngles.z);	
						}
					}
				}
				
				RaycastHit hit;
				AllwaysForward = me.TransformDirection(Vector3.forward);
				AllwaysForward = new Vector3(AllwaysForward.x, 0, AllwaysForward.z);
				WayDirection = me.TransformDirection(Vector3.forward * AllwaysForward.magnitude * power);	
				if (Physics.Raycast(new Vector3(me.position.x, me.position.y + 0.2f, me.position.z), me.TransformDirection(Vector3.down),out hit, 1.0f)){		
					if (hit.transform.tag.Equals("Ground")){				
						me.GetComponent<Rigidbody>().AddForce(WayDirection, ForceMode.Force);		
					}
				}
			} else {
				if (timer + Time.deltaTime > 1){
					try {
						status = "Waiting for target...";
						Target = GameObject.FindGameObjectWithTag("Player").transform;
					} catch{
						
					}
					timer = 0;
				} else timer += Time.deltaTime;
			}
		}
	}
	void OnGUI(){
		if (EnableAI){
			GUI.Label(new Rect(Screen.width - 240,10,20,20),"HP:   "+HP);
			GUI.DrawTexture(new Rect (Screen.width - 220,10, (200*HP)/MaxHP,30),HPBar,ScaleMode.StretchToFill,true);
			GUI.Label(new Rect(Screen.width - 240, 30, 200,20),"Status: "+status);
		} else  GUI.Label(new Rect(Screen.width - 200, 20, 200,20),"Status: AI disabled");
	}
			
	public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis) {
		dirA = dirA - Vector3.Project(dirA, axis);
		dirB = dirB - Vector3.Project(dirB, axis);
		float angle = Vector3.Angle(dirA, dirB);
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
	}
}
