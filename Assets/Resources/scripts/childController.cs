using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class childController : MonoBehaviour {
	const int WALK = 0;
	const int STOP = 1;
	const int BURNING = 2;
	const int WAITING = 3;

	public int initialLives = 50;
	public int lives;
	public int hunger;

	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	public Text live;
	GameObject[] danger;
	public bool isRunning = false;
	public int state = 0;
	public bool isRandomState = true;

	public bool isOutside = false;


	// Use this for initialization
	void Start () {
		lives = initialLives;
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();
		hunger = 0;
		setHungerBarPerSecond (1);

	}
	
	// Update is called once per frame
	void Update () {
		notLivesThenDie ();
		danger = getDangerElements ();

		if (isRandomState) {
			state = Random.Range (0, 2);
		}
		switch (state) {
			case WALK:
			if (!isRunning) {
				playAnimation("child_walking", 3f);
				childRandomMovement ();
			}
			break;
			case STOP:
			if (!agentHasPath && !isRunning) {
				playAnimation("child_idle", 0.6f);
				StartCoroutine(stopFewSeconds());
				StopCoroutine(stopFewSeconds());
			}
			break;
			case BURNING:
			if (!isRunning) {
				childRandomMovement ();
				isRunning = true;
				setBurningChild(2);
			}
			break;
			case WAITING:
			if(!gameObject.GetComponent<Animation>().IsPlaying("child_pain")){
				playAnimation("child_idle", 0.6f);
			}
				
			break;
		}
	}


	public void playAnimation(string animation, float speed){

		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
		
	}

	void childRandomMovement() {
		if (agent.pathStatus == NavMeshPathStatus.PathComplete && !agentHasPath) {
			agentHasPath = true;
		}
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			agentHasPath = false;
			agent.ResetPath();

			int randomNum = Random.Range(0, 3);
			if (randomNum > 0 && danger.Length > 0) {
				setDangerPosition();
			} else {
				setRandomPosition();
			}
		}
	}
	void setRandomPosition() {
		randomPosition = getRandomMeshPosition ();
		agent.SetDestination (randomPosition);
	}
	private Vector3 getRandomMeshPosition () {
		int insideOutsideHomeRandom = Random.Range (1, 10);
		string walkableTerrain = "terrainHome";

		int number = 5;
		bool isStormLightningActive = Camera.main.GetComponent<dangerStrategy> ().isLightningStormEnabled;
		if (isStormLightningActive) {
			number = 1;
		}
		if (insideOutsideHomeRandom > number) {
			walkableTerrain = "terrain";
		}
		GameObject terrain = GameObject.FindWithTag (walkableTerrain);
		float xTerrainMin = terrain.GetComponent<Renderer>().bounds.min.x;
		float xTerrainMax = terrain.GetComponent<Renderer>().bounds.max.x;
		float zTerrainMin = terrain.GetComponent<Renderer>().bounds.min.z;
		float zTerrainMax = terrain.GetComponent<Renderer>().bounds.max.z;
		Vector3 position = new Vector3(Random.Range(xTerrainMin, xTerrainMax), 0, Random.Range(zTerrainMin, zTerrainMax));
		NavMeshHit hit;
		NavMesh.SamplePosition(position, out hit, 10f, 1);
		position = hit.position;
		return position;
	}

	IEnumerator stopFewSeconds(){
		isRunning = true;
		int seconds = Random.Range (2, 7);
		agent.Stop ();
		agentHasPath = false;
		yield return new WaitForSeconds(seconds);
		agent.Resume ();
		isRunning = false;
	}

	void OnTriggerEnter(Collider collision) {
		if (collision.transform.name == "brokenGlass") {
				hitAndPain(5);
		}

		if (collision.transform.tag == "fire") {
			isRandomState = false;
			state = BURNING;
		}
	}

	void goToMenu(string menuName){
		Application.LoadLevel(menuName);
	}

	public IEnumerator painEffect() {
		Material painMaterial = Resources.Load("materials/pain", typeof(Material)) as Material;
		SkinnedMeshRenderer childRenderer = GameObject.Find ("childMesh").GetComponent<SkinnedMeshRenderer> ();
		Material oldMaterial = Resources.Load("materials/baby", typeof(Material)) as Material;

		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.2f);
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.2f);
	}

	GameObject[] getDangerElements() {
		GameObject[] brokenGlassElements = GameObject.FindGameObjectsWithTag("brokenGlass");
		GameObject[] fireElements = null;

		int fireElementsCount = 0;
		if (state != BURNING) {
			fireElements = GameObject.FindGameObjectsWithTag("fire");
			fireElementsCount = fireElements.Length;
		}
		GameObject[] dangerElements = new GameObject[brokenGlassElements.Length + fireElementsCount];

		brokenGlassElements.CopyTo(dangerElements, 0);
		if (state != BURNING) {
			fireElements.CopyTo(dangerElements, brokenGlassElements.Length);
		}

		return dangerElements;
	}
	void setDangerPosition() {
		int dangerPosition = Random.Range (0, danger.Length);
		GameObject dangerFurniDestination = danger [dangerPosition];
		agent.SetDestination (dangerFurniDestination.transform.position);
	}

	
	void setHungerBarPerSecond(int seconds) {
		InvokeRepeating("setHungerBar", 2, seconds);
	}
	
	void setHungerBar(){
		hunger += 1;
		
		GameObject hungerBar = GameObject.Find("hungerBar");
		hungerBar.GetComponent<Image> ().fillAmount = hunger*(float)0.015;
		
		if (hungerBar.GetComponent<Image> ().fillAmount == 1) {
			lives -= 1;
			if(lives >= 0){
				live.text = lives.ToString();
			}
		}	
	}
	void setBurningChild(int seconds) {
		InvokeRepeating("burningChild", 2, seconds);
	}

	public void unsetBurningChild() {
		CancelInvoke("burningChild");
	}

	void burningChild(){
		if (state != BURNING) {
			unsetBurningChild();
		}
		isRunning = true;
		hitAndPain (1);
		if (state != WAITING) {
			childRandomMovement ();
		}

	}
	void notLivesThenDie() {
		if (lives <= 0) {
			goToMenu("mainMenu");
		}	
	}

	public void hitAndPain(int damage){
		if (state != BURNING) {
			playAnimation("child_pain", 0.5f);
		}

		lives -= damage;
		live.text = lives.ToString();
		StartCoroutine(painEffect());
		StopCoroutine(painEffect());
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.transform.tag == "terrain") {
			isOutside = true;
		}

		if (collision.transform.tag == "lightning") {
			hitAndPain(10);
		}
		
	}
	
	void OnCollisionExit(Collision collision) {
		if (collision.transform.tag == "terrain") {
			isOutside = false;
		}
		
	}
}
