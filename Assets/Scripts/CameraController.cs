using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	private Transform me;
	public Transform target;	
	public float MaxDistance	= 30.0f;
	private float distance		= 10.0f;
	private int WheelAxis;
	public float xSpeed			= 250.0f;
	public float ySpeed			= 120.0f;
	public float yMinLimit 		= -20.0f;
	public float yMaxLimit 		= 80.0f;	
	private float x				= 0.0f;
	private float y				= 0.0f;
	private float defFOV;	
	private float timer;
	
	void Start(){
		me = transform;
		defFOV = me.GetComponent<Camera>().fieldOfView;
		Vector3 Eulerangles = me.eulerAngles;
		x = Eulerangles.x;
		y = Eulerangles.y;
		WheelAxis = 3;
	}
	
	public float GetDistance(){
		return distance;
	}
	
	void LateUpdate () {
		if (target)	{
			x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime; 
			y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime; 		
	 		y = ClampAngle(y, yMinLimit, yMaxLimit);	 		       
	        Quaternion rotation = Quaternion.Euler(y, x, 0);
	        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
			if (Input.GetAxis("Mouse ScrollWheel") < 0.0f) WheelAxis -= 1;
			if (Input.GetAxis("Mouse ScrollWheel") > 0.0f) WheelAxis += 1;
			WheelAxis = Mathf.Clamp(WheelAxis, -4,5);
			switch (WheelAxis){
				case -4:
					distance = 0;
					me.GetComponent<Camera>().fieldOfView = 10;
					break;
				case -3:
					distance = 0;
					me.GetComponent<Camera>().fieldOfView = 20;
					break;
				case -2:
					distance = 0;
					me.GetComponent<Camera>().fieldOfView = 35;
					break;
				case -1:
					distance = 0;
					me.GetComponent<Camera>().fieldOfView = 50;
					break;
				case 0:
					distance = 0;
					me.GetComponent<Camera>().fieldOfView = defFOV;
					break;
				case 1:
					distance = 4;
					break;
				case 2:
					distance = 6;
					break;
				case 3:
					distance = 12;
					break;
				case 4:
					distance = 16;
					break;
				case 5:
					distance = 20;
					break;
				
			}
			distance = Mathf.Clamp(distance, 0, MaxDistance);			
	        transform.rotation = rotation;
	        transform.position = position;			
		} else {
			if (timer + Time.deltaTime >1){
				try {
					target = GameObject.FindGameObjectWithTag("CamPoint").transform;
				} catch{
					
				}
				timer = 0;
			} else timer += Time.deltaTime;
		}
	}
	
	static float ClampAngle (float angle, float min, float max) {
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp (angle, min, max);
	}
}
	