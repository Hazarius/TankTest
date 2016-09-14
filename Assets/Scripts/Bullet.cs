using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public Transform 	BlowEffect;
	public float 		FirePower = 50f;
	public AudioClip	SoundEffect;
	private Transform 	me;
	private float		TTL = 30;
	private float		timer;
	
	void Start(){
		me = transform;
	}
	
	void Update () {
		if (timer+Time.deltaTime > TTL){
			GameObject.Instantiate(BlowEffect, me.position, Quaternion.identity);
			Destroy(me.gameObject);
		} else timer+=Time.deltaTime;
	}
	
	void OnCollisionEnter (Collision col){
		/*if (Mathf.Acos(Vector3.Dot(me.rigidbody.velocity.normalized, -col.contacts[0].normal))*180/Mathf.PI  > 70){
			me.rigidbody.AddForce(Vector3.Reflect(me.rigidbody.velocity, col.contacts[0].normal));
			Debug.Log("Reflect");
		} else {
		*/
			col.transform.SendMessage("Hit",FirePower, SendMessageOptions.DontRequireReceiver);
			GameObject.Instantiate(BlowEffect, col.contacts[0].point, Quaternion.identity);
			GetComponent<AudioSource>().PlayOneShot(SoundEffect, 0.7f);
			if (me) GameObject.Destroy(me.gameObject);
		//}
	}
}
