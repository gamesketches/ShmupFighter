﻿using UnityEngine;
using System.Collections;

public enum BulletType {Knife, Shield, Boomerang};

public class BulletLogic : MonoBehaviour {

	public BulletType type;
	int damage;
	float velocity;
	public float lifetime;
	public float velocityMultiplier = 10f;
	public float indirectCorrectionSpeed = 5f;
    public float indirectHomingTime = 50.5f; // how long it updates its target 
    public float indirectHomingLimit = 10.0f; // how long it flies towards target    
    delegate void BulletFunction();
    BulletFunction bulletFunction;
	private Vector2 travelVector;
	private Transform target;
	private Vector3 targetPosition;
	private float headingTime, shieldOscillationTime;
    private Sprite sprite;
    private int animFrame;
    private bool reflectiveShot;
    private string playerPathString;
    new AudioSource audio;
    new SpriteRenderer renderer;
	private new Sprite[] animation; 
	AnimationCurve shieldVelocity;
	public BulletDepot theDepot;

    GameObject myCollidersObj;
    PolygonCollider2D[] myPolys;
    PolygonCollider2D myPoly;
    private float speedUp = 0;
    const float SPEEDCAP = 5.0f;


    private bool debug_on = false; 
    // Use this for initialization
    void Start () {
        //reflectiveShot = false;
		renderer = GetComponentInChildren<SpriteRenderer>();
		audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("audio/soundEffects/rpsBulletCancel");
		Keyframe[] keyFrames = new Keyframe[10];
		float modulation = 0.1f;
		for(int i = 0; i < keyFrames.Length; i++) {
			keyFrames[i] = new Keyframe(0.1f * i, modulation);
			modulation *= -1;
		}
		shieldVelocity = new AnimationCurve(keyFrames);

    }
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if(lifetime <= 0f) {
			//theDepot.AddObject(gameObject);
			killBullet();
            //Debug.Log("Added depot code 1");
            return;
        }
        bulletFunction();
		gameObject.transform.position += new Vector3(travelVector.x, travelVector.y) * Time.deltaTime;
  	}

	public void Initialize(BulletType bulletType, int bulletDamage, float Velocity, float size,
													float Lifetime, Color bulletColor, int playerNum,
													Character character){
		type = bulletType;
		damage = bulletDamage;
		reflectiveShot = false;
		transform.localScale = new Vector3(size, size, size);
		gameObject.tag = bulletType.ToString();
		velocity = Velocity * velocityMultiplier;
		playerPathString = string.Concat(character.ToString(), playerNum == 0 ? "" : "Alt");
		if(velocity == 0) {
			velocity = 5f;
		}
		Vector3 tempVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
		travelVector = new Vector2(tempVector.x, tempVector.y);
		lifetime = Lifetime;
		gameObject.layer = 8 + playerNum;

        myCollidersObj = GameObject.Find("BulletColliders");
        myPolys = myCollidersObj.GetComponents<PolygonCollider2D>();



        switch (type) {
			case BulletType.Boomerang:
				if(character == Character.Bastet) { 
					bulletFunction = StraightLogic;
					transform.Rotate(new Vector3(0f, 0f, -90f));
					travelVector *= -1;
					}
				else if(character == Character.Loholt) {
					bulletFunction = StraightLogic;
					reflectiveShot = true;
					travelVector *= -1;
					Lifetime = 100;
                    // add custom collider, 
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;
                    myPoly = gameObject.GetComponent<PolygonCollider2D>();
                    myPoly.points = myPolys[2].points;
                    myPoly.isTrigger = true;
                    myPoly.enabled = true;
                }
                else bulletFunction = IndirectLogic;

                // add custom collider, 
                if (character == Character.Orpheus)
                {
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;

                    myPoly = gameObject.GetComponent<PolygonCollider2D>();
                    myPoly.points = myPolys[3].points;
                    myPoly.isTrigger = true;
                    myPoly.enabled = true;
                }

                // add custom collider, 
                if (character == Character.Hiruko)
                {
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;

                    myPoly = gameObject.GetComponent<PolygonCollider2D>();
                    myPoly.points = myPolys[4].points;
                    myPoly.isTrigger = true;
                    myPoly.enabled = true;
                }

                // add custom collider, 
                if (character == Character.Bastet)
                {
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;

                    myPoly = gameObject.GetComponent<PolygonCollider2D>();
                    myPoly.points = myPolys[5].points;
                    myPoly.isTrigger = true;
                    myPoly.enabled = true;
                }

                sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", playerPathString, "/Boomerang"));
				headingTime = 0f;
    
                //indirectHomingLimit = 10f; // how long it tracks
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")){
					if(player.layer != gameObject.layer) {
						target = player.transform;
						}
				}

                break;
			case BulletType.Knife:
                sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", playerPathString, "/Knife"));
                // TODO: change this
                transform.Rotate(new Vector3(0f, 0f, -90f));
                //transform.rotation = new Vector3(0f, 0f, -90.0f);
				bulletFunction = StraightLogic;
                if (character == Character.Loholt) // add custom collider
                {   
                    //Destroy(gameObject.GetComponent<Collider2D>());
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;
                    myPoly = gameObject.GetComponent<PolygonCollider2D>();
                    myPoly.points = myCollidersObj.GetComponent<PolygonCollider2D>().points;
                    myPoly.isTrigger = true;
                    myPoly.enabled = true;

                }
                //Debug.Log("in here");
                break;
			// Shield situation
			default:
			    sprite = Resources.Load<Sprite>(string.Concat("sprites/weapons/", playerPathString, "/Shield"));
				    bulletFunction = SlowShotLogic;
			    GetComponent<CircleCollider2D>().radius = 0.5f;
			    lifetime = Lifetime / 1.5f;
			    //lifetime = Lifetime / 0.25f;
			    tempVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
			    travelVector = new Vector2(tempVector.x, tempVector.y);
                //create polygon colider
                if (character == Character.Loholt) // add custom collider, trying it on loholt
                {
                    gameObject.GetComponent<CircleCollider2D>().enabled = false;
                    myPoly = gameObject.GetComponent<PolygonCollider2D>();
                    myPoly.points = myPolys[1].points;
                    myPoly.isTrigger = true;
                    myPoly.enabled = true;
                }
                break;
		}
        //renderer.sprite = sprite;
        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == "Boundary" || other.gameObject.tag == "ComplexBoundary") {
			if(reflectiveShot) {
                speedUp += 1.0f;
                if (speedUp < SPEEDCAP) //quick way to cap max speed
                {
                    travelVector *= 1.25f;
                }
                lifetime *= 1.2f; //small juice to lifetime for hitting the wall
                if (other.gameObject.tag == "ComplexBoundary") {
					RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) - travelVector, travelVector * 2);
                	if(hit != null) {
                		travelVector = Vector2.Reflect(travelVector, hit.normal);
               	 }
                }
				else {
                if (other.transform.position.y > 0) //top
                {
                    travelVector.Set(travelVector.x, -travelVector.y);
                    //Debug.Log("hell 1");
                    }
                    else if (other.transform.position.y < 0) //bottom
                {
                    travelVector.Set(travelVector.x, -travelVector.y);
                    //Debug.Log("hell 2");

                }
                else if (other.transform.position.x < 0) //left
                {
                    travelVector.Set(-travelVector.x, travelVector.y);
                    //Debug.Log("hell 3");
                }
                else //right
                {
					travelVector.Set(-travelVector.x, travelVector.y);
                    //Debug.Log("hell 4" + " travelvector" + travelVector );
                    }
                    return;
                }
        	}
			if(lifetime < 5) {
				killBullet();
				//theDepot.AddObject(gameObject);
                //Debug.Log("Added depot code 2");
            }
            return;
		}
		if(other.gameObject.layer != gameObject.layer) {
			if(other.gameObject.tag == "Player") {
					other.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), other.transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
                    sparks.transform.localScale = new Vector3(damage * 2, damage * 2, damage * 2);
                    //if (debug_on) Debug.Log("hit spark 0");
                    if (GameObject.FindGameObjectsWithTag("SoundEffects").Length < 5) {
					GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
					temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("damageHit");
					}
					killBullet();
					//theDepot.AddObject(gameObject);
                    //Debug.Log("Added depot code 3");
                return;
			}
			else if(other.gameObject.tag == "Reticle") {
				return;
			}
			BulletType opposingType = (BulletType)System.Enum.Parse(typeof(BulletType), other.gameObject.tag);


			if(opposingType == type){
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("identicalBulletCancel");
				GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
                sparks.transform.localScale = new Vector3(10f, 10f, 10f);
                sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
				theDepot.AddObject(other.gameObject);
				killBullet();
				//theDepot.AddObject(gameObject);
                //Debug.Log("Added depot code 4");

            }
            else if ((int)type == System.Enum.GetValues(typeof(BulletType)).Length - 1)
            {
                return;
            }
            else if((int)opposingType == System.Enum.GetValues(typeof(BulletType)).Length - 1 && (int)type == 0) {
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("rpsBulletCancel");
				//theDepot.AddObject(gameObject);
                Debug.Log("Added depot code 5");
                //GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
                //sparks.transform.localScale = new Vector3(10f, 10f, 10f);
                //Debug.Log("hit spark 2");
                //sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
				killBullet();	
			}
			else {
				GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
                //scaling hitsparkst test -ski
				sparks.transform.localScale = new Vector3(10f, 10f, 10f);
                sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/hitSparks/hitspark");
                //dynamic hitspark color test -ski
                if (debug_on) Debug.Log("hit spark 1");
                //if(gameObject.tag == "Reticle" && )
                GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("rpsBulletCancel");
				GameObject destroyedObject = opposingType > type ? other.gameObject : gameObject;
				//theDepot.AddObject(destroyedObject);
				// THis sucks
				destroyedObject.GetComponent<BulletLogic>().killBullet();
                //Debug.Log("Added depot code 6");
            }
        }
	}

	void IndirectLogic(){
		//Debug.Log("indirect logic");
		renderer.transform.Rotate(0, 0, 2);
		if(headingTime < indirectHomingTime) {
			targetPosition = target.position;
		}

        //Debug.Log("indirect logic: velocity = " + velocity);

        // Might be better to handle this shit as a rotation
        // i changed this to homing limit to accomdate offscreen shots - ski
        //if(headingTime < 1.0f) {
        if (headingTime < indirectHomingLimit) {
		    Vector3 startVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
		    Vector3 temp = Vector3.Lerp(startVector, targetPosition - gameObject.transform.position, 
			    headingTime);
		    travelVector.x = temp.x;
		    travelVector.y = temp.y;
		    headingTime += indirectCorrectionSpeed / (indirectCorrectionSpeed * 60);
		    //headingTime += indirectCorrectionSpeed / 60.0f  ;
        }
    }

    void StraightLogic(){
		//travelVector = new Vector2(velocity, 0f);
	}

	void SlowShotLogic(){
		renderer.transform.position += Vector3.one * shieldVelocity.Evaluate(shieldOscillationTime) * 0.33f;
		shieldOscillationTime += Time.deltaTime;
		//slowing shots in motion
        //travelVector *= 0.975f; //ski
        //Debug.Log("sloweing. shieldOscillationTime = " + shieldOscillationTime);
        
        if (shieldOscillationTime > 0.5f) {
            travelVector *= 0.6f; //ski

            shieldOscillationTime = 0;
			renderer.transform.localPosition = Vector3.zero;
		}
	}

	public void killBullet() {
		GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/BulletDeathObject"), gameObject.transform.position, Quaternion.identity);
		temp.GetComponent<BulletDeathBehavior>().PlayAnimation(playerPathString, type);
		transform.rotation = Quaternion.identity;
		renderer.transform.rotation = Quaternion.identity;
		theDepot.AddObject(gameObject);

	}
}
