using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public Transform PlayerTankPrefab;
	public Transform AITankPrefab;
	public Transform PlayerSpawner;
	public Transform AISpawner;
	private Transform PlayerTank;
	private Transform AI;
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)){
			Cursor.visible = true;
		}
	}
	
	void OnGUI(){
		GUI.Box(new Rect (0,0, 230,250),"");
		if (GUI.Button(new Rect(25,35,180,30),"Start new game")){
			if (PlayerTank) Destroy(PlayerTank.gameObject);
			if (AI) Destroy(AI.gameObject);
			PlayerTank = GameObject.Instantiate(PlayerTankPrefab, PlayerSpawner.position, Quaternion.identity) as Transform;
			AI = GameObject.Instantiate(AITankPrefab, AISpawner.position, Quaternion.identity) as Transform;
			Cursor.visible = false;
		}
	}
}
