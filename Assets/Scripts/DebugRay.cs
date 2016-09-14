using UnityEngine;
using System.Collections;

public class DebugRay : MonoBehaviour {
	public Vector3 Direction = new Vector3(0,0,1);
	public int Length = 100;
	public bool Draw = true;
	public Color MainColor;
	
	void Update () {
	if (Draw)
		Debug.DrawRay(transform.position, transform.TransformDirection(Direction * Length), MainColor);
	}
}
