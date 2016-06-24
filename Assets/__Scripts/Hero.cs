using UnityEngine;
using System.Collections;




public class Hero : MonoBehaviour {

	static public Hero		S;
	public float gameRestartDelay = 2f;
	public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;

	[SerializeField]
	private float _shieldLevel = 1;

	//Weapon Fields
	public Weapon[] weapons;

	public bool	_____________________;
	public Bounds bounds;

	//Declare a new delegate type WeaponFireDelegate
	public delegate void WeaponFireDelegate();
	//Create a WeaponFireDelegate field named fireDelegate.
	public WeaponFireDelegate fireDelegate;




	void Awake(){
		S = this;
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}
		


	// Use this for initialization
	void Start () {
		//Reset the weapons to start _Hero with 1 blaster
		ClearWeapons ();
		weapons [0].SetType (WeaponType.blaster);
	}
	
	// Update is called once per frame
	void Update () {
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		
		bounds.center = transform.position;
		
		// constrain to screen
		Vector3 off = Utils.ScreenBoundsCheck(bounds,BoundsTest.onScreen);
		if (off != Vector3.zero) {  // we need to move ship back on screen
			pos -= off;
			transform.position = pos;
		}
		
		// rotate the ship to make it feel more dynamic
		transform.rotation =Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);

		//Use the fireDelegate to fire Weapons
		//First, make sure the Axis("Jump") button is pressed
		//Then ensure that fireDelegate isn't null to avoid an error
		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null) {
			fireDelegate();
		}
	}

	//This variable holds a reference to the last triggering GameObject
	public GameObject lastTriggerGo = null;

	void OnTriggerEnter(Collider other){
		//Find the tag of other.gameObject or its parent GameObjects
		GameObject go = Utils.FindTaggedParent (other.gameObject);
		//If there is a parent with a tag
		if (go != null) {
			//Announce it
			//Make sure it's not the same triggering go as last time
			if (go ==lastTriggerGo){
				return;
			}
			lastTriggerGo = go;
			if(go.tag == "Enemy"){
				shieldLevel--;
				Destroy(go);
			} else if (go.tag == "PowerUp"){
				//if the shield was trigger by a powerup
				AbsorbPowerUp(go);
			} else {
				print ("Triggered: " + go.name);
			}
		} else {
			print ("Triggered: " + other.gameObject.name);
		}
	}

	public float shieldLevel{
		get {
			return(_shieldLevel);
		}
		set {
			_shieldLevel = Mathf.Min(value, 4);
			//If the shield is going to be set to less than zero
			if(value < 0){
				Destroy(this.gameObject);
				//Tell Main.S to restart the game after a delay
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}

	public void AbsorbPowerUp(GameObject go){
		PowerUp pu = go.GetComponent<PowerUp> ();
		switch (pu.type) {
		case WeaponType.shield: //If it's the shield
			shieldLevel++;
			break;

		default://If it's any Weapon PowerUp
			//Check the current weapon type
			if(pu.type == weapons[0].type){
				//then increase the number of weapons of this type
				Weapon w = GetEmptyWeaponSlot(); //Find an available weapon
				if (w != null){
					//Set it to pu.type
					w.SetType(pu.type);
				}
			} else {
				//If this is a different weapon
				ClearWeapons();
				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy (this.gameObject);
	}

	Weapon GetEmptyWeaponSlot() {
		for (int i=0; i < weapons.Length; i++) {
			if (weapons[i].type == WeaponType.none){
				return(weapons[i]);
			}
		}
		return(null);
	}

	void ClearWeapons(){
		foreach (Weapon w in weapons) {
			w.SetType(WeaponType.none);
		}
	}
}