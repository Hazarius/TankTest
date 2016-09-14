using UnityEngine;
using System.Collections;

public class TowerController : MonoBehaviour {
	public bool 		CanControl;
	public Transform 	Tower;	
	public Transform 	Gun;
	public Transform	BulletSpawner;	
	public Texture2D	Crosshair;	
	private Transform	Target;
	private Vector3		RayTarget;
	public float		TowerRotationSpeed = 2;
	public float		GunRotationSpeed = 2;
	public Vector2		MinMaxGunAngle = new Vector2(-10,10);
	
	private PlayerTankController pc;
	private Vector3		TowerEuler;
	private Vector3		GunEuler;
	private Quaternion	QTowerEuler;
	private Quaternion	QGunEuler;
	private float		TowerAngle;
	private float		GunAngle;
	private Vector3 	CrossPositionOnScreen;
	private Vector3 	CrossPositionInWorld;	
	private RaycastHit	hit;
	private RaycastHit	Crosshit;
	private Transform	me;
	private int			mask;
	
	private float		timer;
	
	void Start () {
		me = transform;
		if (!CanControl){
			Target = GameObject.FindGameObjectWithTag("Player").transform;
		} else {
			pc = GameObject.FindObjectOfType(typeof(PlayerTankController)) as PlayerTankController;
			mask = 1<<8;
			mask = ~mask;
		}
	}
	
	void LateUpdate () {			
			if (CanControl){
				if ((pc)&&(pc.enableControl)){
				
					if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0)),out hit, 1500, mask)){
						RayTarget = hit.point;
					} else RayTarget = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0)).direction * 100;
					
					Vector3 TargetDir = (RayTarget - me.position).normalized;
					
					TowerAngle 		= AngleAroundAxis(me.forward, TargetDir, Vector3.up);
					QTowerEuler		= Quaternion.Euler(Tower.localEulerAngles.x, TowerAngle, Tower.localEulerAngles.z);
					Tower.localRotation = Quaternion.Lerp(Tower.localRotation, QTowerEuler, Time.deltaTime * TowerRotationSpeed);
				
				
					GunAngle		= AngleAroundAxis(BulletSpawner.forward, TargetDir, Tower.TransformDirection(Vector3.right));
					GunAngle		= Mathf.Clamp(GunAngle, MinMaxGunAngle.x, MinMaxGunAngle.y);
				
					GunEuler		= new Vector3(GunEuler.x, Gun.localEulerAngles.y, Gun.localEulerAngles.z);
					GunEuler.x		= Mathf.LerpAngle(GunEuler.x, GunAngle, Time.deltaTime * GunRotationSpeed);
					GunEuler.x		= Mathf.Clamp(GunEuler.x, MinMaxGunAngle.x, MinMaxGunAngle.y );
					Gun.localEulerAngles  = GunEuler;
					
					if (Physics.Raycast(BulletSpawner.position, BulletSpawner.TransformDirection(Vector3.forward),out Crosshit,1500, mask)){
						CrossPositionInWorld = Crosshit.point;
					} else CrossPositionInWorld = BulletSpawner.TransformDirection(Vector3.forward * 100);
					
					CrossPositionOnScreen = Camera.main.WorldToScreenPoint(CrossPositionInWorld);
				}
			} else {
				if (Target){
					Vector3 TargetDir = (Target.position - me.position).normalized;
					TowerAngle 		= AngleAroundAxis(me.forward, TargetDir, Vector3.up);
					GunAngle		= AngleAroundAxis(Tower.forward, TargetDir, Tower.TransformDirection(Vector3.right));	
					
					TowerEuler 		= new Vector3(Tower.localEulerAngles.x, TowerEuler.y, Tower.localEulerAngles.z);
					TowerEuler.y	= Mathf.LerpAngle(TowerEuler.y, TowerAngle, Time.deltaTime * TowerRotationSpeed);
					Tower.localEulerAngles = TowerEuler;	
					
					GunEuler		= new Vector3(GunEuler.x, Gun.localEulerAngles.y, Gun.localEulerAngles.z);
					GunEuler.x		= Mathf.LerpAngle(GunEuler.x, GunAngle, Time.deltaTime * GunRotationSpeed);
					GunEuler.x		= Mathf.Clamp(GunEuler.x, MinMaxGunAngle.x, MinMaxGunAngle.y );
					Gun.localEulerAngles  = GunEuler;
				} else {
					if (timer + Time.deltaTime > 1){
						try {							
							Target = GameObject.FindGameObjectWithTag("Player").transform;
						} catch{
							
						}
						timer = 0;
					} else timer += Time.deltaTime;
				}
			}
	}
	
	public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis) {
		dirA = dirA - Vector3.Project(dirA, axis);
		dirB = dirB - Vector3.Project(dirB, axis);
		float angle = Vector3.Angle(dirA, dirB);
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
	}
	
	static float ClampAngle (float angle, float min, float max) {
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp (angle, min, max);
	}
	
	void OnGUI(){
		if (CanControl){
			
			GUI.Label(new Rect(20,100,300,20),""+TowerAngle);
			GUI.Label(new Rect(20,120,300,20),""+GunAngle);
			GUI.Label(new Rect(20,140,300,20),"World "+CrossPositionInWorld);
			GUI.Label(new Rect(20,160,300,20),"Screen "+CrossPositionOnScreen);
			GUI.Label(new Rect(20,180,300,20),"QGunEuler "+QGunEuler);
			
			GUI.DrawTexture(new Rect(CrossPositionOnScreen.x-32, Screen.height - CrossPositionOnScreen.y-32 ,64,64),Crosshair,ScaleMode.StretchToFill,true);
			GUI.DrawTexture(new Rect(Screen.width/2-32, Screen.height/2-32,64,64),Crosshair,ScaleMode.StretchToFill,true);
		}
	}
}
