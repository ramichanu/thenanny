using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChildControllerNew : EventScript {
	public const int IDLE = 0;
	public const int BURNING = 1;

	public int state = IDLE;
	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	Transform target;
	GameObject[] danger;


	// Use this for initialization
	void Start () {
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();

		startChildRandomMovementEvent ();
	}
	
	// Update is called once per frame
	void Update () {
		randomMovement ();
		danger = getDangerElements ();
	}

	void startChildRandomMovementEvent(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		canInterruptBy.Add("helpBurning");
		methodsToCall.Add("child_startChildRandomMovement");
		methodsAfterInterrupt.Add("child_stopChildMovement");

		base.eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("startChildRandomMovementEvent");
	}
	
	void startChildRandomMovement(){
		InvokeRepeating("childRandomMovement", 0, 1);
		eventFinishedCallback("startChildRandomMovement");
	}

	void childRandomMovement(){
		if (!agentHasPath) {
			agent.speed = 1.5f;
			playAnimation("child_walking", 2.5f);
			setRandomPosition ();
		}
	}

	void randomMovement() {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			playAnimation("child_idle", 1.5f);
			agent.speed = 1.5f;
			agentHasPath = false;
			agent.ResetPath();
		}
	}

	void setRandomPosition() {
		int random = Random.Range (0, 3);
		randomPosition = getRandomMeshPosition ();
		if (random > 0 && danger.Length > 0) {
			randomPosition = getDangerPosition ();
		}
		agent.SetDestination (randomPosition);
		agentHasPath = true;
	}

	Vector3 getDangerPosition() {
		int dangerPosition = Random.Range (0, danger.Length);
		GameObject dangerFurniDestination = danger [dangerPosition];
		return dangerFurniDestination.transform.position;
	}

	GameObject[] getDangerElements() {
		GameObject[] brokenGlassElements = GameObject.FindGameObjectsWithTag("brokenGlass");
		GameObject[] fireElements = null;
		
		GameObject fires = GameObject.Find ("fires");
		int fireElementsCount = 0;
		if (fires != null) {
			fireElementsCount = 1;
			fireElements = GameObject.FindGameObjectsWithTag("fire");
			if (fireElements.Length > 0) {
				int fireElementRandom = Random.Range(0, fireElements.Length);
				fireElements = new GameObject[1]{fireElements[fireElementRandom]};
			}
		}
		GameObject[] dangerElements = new GameObject[brokenGlassElements.Length + fireElementsCount];
		
		brokenGlassElements.CopyTo(dangerElements, 0);
		if (fires != null) {
			fireElements.CopyTo(dangerElements, brokenGlassElements.Length);
		}
		
		return dangerElements;
	}
	
	public Vector3 getRandomMeshPosition () {
		GameObject terrain = GameObject.FindWithTag ("terrainHome");
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

	void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}

	public void hitAndPain(int damage){

		playAnimation("child_pain", 0.5f);
		
		GameObject life = GameObject.Find ("lifeAndHunger");
		life.GetComponent<LifeAndHunger> ().restPercentLife (damage);
		
		StartCoroutine(painEffect());
		StopCoroutine(painEffect());
		
		
	}

	public IEnumerator painEffect() {
		Material painMaterial = Resources.Load("materials/pain", typeof(Material)) as Material;
		SkinnedMeshRenderer childRenderer = GameObject.Find ("childMesh").GetComponent<SkinnedMeshRenderer> ();
		Material oldMaterial = Resources.Load("materials/baby", typeof(Material)) as Material;
		
		GameObject childIcon = GameObject.Find ("childIcon");
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("hub/child_icon_pain");
		
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("hub/child_icon");
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.2f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("hub/child_icon_pain");
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("hub/child_icon");
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.2f);
	}

	void OnTriggerEnter(Collider collision) {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		switch(collision.transform.tag) {
			case "brokenGlass":
				methodsToCall.Add("child_painBrokenGlass");
				eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				break;
			case "fire":
				state = BURNING;
				methodsToCall.Add("child_painFire");
				eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				break;
		}
	}

	void painBrokenGlass(){
		hitAndPain(5);
		eventFinishedCallback("painBrokenGlass");
	}

	void painFire(){
		ParticleSystem fireChild = transform.FindChild("fireChild").GetComponent<ParticleSystem>();
		if(!fireChild.isPlaying){
			fireChild.Play();
		}
		InvokeRepeating("burningChild", 2, 2);
		eventFinishedCallback("painFire");
	}

	void burningChild(){
		hitAndPain (2);	
	}

	void stopChildMovement(){
		agent.Stop ();
		agent.ResetPath ();
		agentHasPath = false;
		playAnimation("child_idle", 1.5f);
		CancelInvoke ("childRandomMovement");
		eventFinishedCallback("stopChildMovement");
	}

	void cancelBurning(){
		CancelInvoke ("burningChild");
		eventFinishedCallback("cancelBurning");
	}

	void helpBurning(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();		

		methodsToCall.Add("child_cancelBurning");
		methodsToCall.Add("child_stopChildMovement");
		methodsToCall.Add("player_moveCharacterToClickedDestination");

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("helpBurning");
	}

	void fireOff() {
		state = IDLE;
		ParticleSystem fireChild = GameObject.Find("fireChild").GetComponent<ParticleSystem>();
		fireChild.Stop();
		fireChild.Clear();

		eventFinishedCallback("fireOff");
	}


}
