using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

public class Main : MonoBehaviour {
	static public Main S ;


	public GameObject[] prefabEnemies ;
	public float enemySpawnPerSecond = 0.5f ;
	public float enemySpawnPadding = 1.5f ;

	public bool _______________ ;
	public float enemySpawnRate ;



	void Awake () {

		S = this;
		//Set Utils.cambounds
		Utils.SetCameraBounds (this.camera);
		//0.5 enemies/second = enemySpawnRate of 2
		enemySpawnRate = 1f / enemySpawnPerSecond;
		//Invoke call spawnenemy() once after a 2 second delay
		Invoke ("SpawnEnemy", enemySpawnRate);

	}


	void Start(){

		Screen.SetResolution (630, 900, false);

		GameObject scoreGO = GameObject.Find ("ScoreCounter");

	}

	public void SpawnEnemy(){
		//pick a random enemy prefab to instantiate
		int ndx = Random.Range (0,prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies[ndx])as GameObject ;
		//position the Enemy above the screen with a random x positions
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		//call spawnEnemy() again in a couple of seconds
		Invoke ("SpawnEnemy", enemySpawnRate);
	}

	public void DelayedRestart(float delay){
		//Invoke the Restart() method in delay seconds
		Invoke ("Restart", delay);
	}

	public void Restart(){
		//Reload _Scene_0 to restart the game
		Application.LoadLevel ("_Scene_0");
	}

}
