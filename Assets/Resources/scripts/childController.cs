using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class childController : MonoBehaviour {
	const int WALK = 0;
	const int STOP = 1;
	const int BURNING = 2;

	int lives;
	public int hunger;

	NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	public Text live;
	GameObject[] danger;
	bool isRunning = false;
	int state = 0;
	bool isRandomState = true;


	// Use this for initialization
	void Start () {
		lives = 50;
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();
		hunger = 0;
		setHungerBarPerSecond (1);

	}
	
	// Update is called once per frame
	void Update () {
		danger = getDangerElements ();

		if (isRandomState) {
			state = Random.Range (0, 2);
		}
		switch (state) {
			case WALK:
			if (!isRunning) {
				childRandomMovement ();
			}
			break;
			case STOP:
			if (!agentHasPath && !isRunning) {
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
		}
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
		GameObject terrain = GameObject.FindWithTag ("terrain");
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
			lives -= 5;
			live.text = lives.ToString();
			StartCoroutine(painEffect());
			StopCoroutine(painEffect());
		}

		if (collision.transform.name == "fire") {
			isRandomState = false;
			state = BURNING;
		}

		if (lives <= 0) {
			goToMenu("mainMenu");
		}	
	}

	void goToMenu(string menuName){
		Application.LoadLevel(menuName);
	}

	IEnumerator painEffect() {
		Material painMaterial = Resources.Load("materials/painEffect", typeof(Material)) as Material;
		Material oldMaterial = GetComponent<Renderer>().material;

		GetComponent<Renderer>().material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		GetComponent<Renderer>().material = oldMaterial;
		yield return new WaitForSeconds(0.2f);
		GetComponent<Renderer>().material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		GetComponent<Renderer>().material = oldMaterial;
	}

	GameObject[] getDangerElements() {
		GameObject[] brokenGlassElements = GameObject.FindGameObjectsWithTag("brokenGlass");
		GameObject[] fireElements = GameObject.FindGameObjectsWithTag("fire");
		GameObject[] dangerElements = new GameObject[brokenGlassElements.Length + fireElements.Length];

		brokenGlassElements.CopyTo(dangerElements, 0);
		fireElements.CopyTo(dangerElements, brokenGlassElements.Length);
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
			goToMenu("mainMenu");
		}
		
	}
	void setBurningChild(int seconds) {
		InvokeRepeating("burningChild", 2, seconds);
	}
	void burningChild(){
		isRunning = true;
		lives -= 1;
		live.text = lives.ToString();
		StartCoroutine(painEffect());
		StopCoroutine(painEffect());
		childRandomMovement ();
	}

}
