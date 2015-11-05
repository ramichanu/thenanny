using UnityEngine;
using System.Collections;

public class PlayerMovementNew : EventScript {
	RaycastHit hit;
	NavMeshAgent agent;

	public GameObject extinguisher = null;
	public GameObject broom = null;

	bool hasPath = false;
	public bool hasBabyBottle = false;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}
	

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
			if(isOverUI) {
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			int layerMask = 1 << 8;
			layerMask = ~layerMask;


			if (Input.touchCount > 0) {
				isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
			}

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask) && !isOverUI) {
				ArrayList canInterruptBy = new ArrayList();
				ArrayList methodsToCall = new ArrayList();
				ArrayList methodsAfterInterrupt = new ArrayList();
				ArrayList methodsDisabledUntilEventFinished = new ArrayList();
				Debug.Log (hit.transform.tag);
				switch (hit.transform.tag) {
					case "player":
					case "terrain":
					case "brokenGlass":
					case "terrainHome":

						canInterruptBy.Add("moveCharacterToClickedDestination");
						canInterruptBy.Add("goToPlayer");
						canInterruptBy.Add("removeBrokenGlass");
						canInterruptBy.Add("removeBrokenJar");

						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("player_moveCharacterToClickedDestination");
						methodsToCall.Add("player_playNannyIdle");

						methodsAfterInterrupt.Add("player_stopPlayerMovement");

						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;

					case "fire":
						
						methodsToCall.Add("player_startFireExtinguish");
						canInterruptBy.Add("playNannyCockroach");
						methodsAfterInterrupt.Add("player_stopPlayerMovement");
						
						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;
					case "cockroach":

						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("player_moveCharacterToCockroach");
						methodsToCall.Add("cockroach_destroyCockroach");
						methodsToCall.Add("player_stopPlayerMovement");
						methodsToCall.Add("player_playNannyIdle");
						methodsDisabledUntilEventFinished.Add ("cockroach_goToPlayer");
						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;
					case "child":
						GameObject child = hit.transform.gameObject;
						if(child.GetComponent<ChildControllerNew>().state == ChildControllerNew.BURNING) {
							GameObject.Find ("Canvas").GetComponent<gameFunctions>().createClickMenu(child);
						}
						break;
					case "madLady":
						methodsToCall.Add("madLady_createMadladyMenu");
						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

					break;
				}
			}
		}

		destinationReachedLogic ();

	}

	void startFireExtinguish(){
		startRemoveFire ();
		eventFinishedCallback("startFireExtinguish");
	}

	void fireExtinguish(){
		removeFire ();
	}
	void startRemoveFire(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		
		methodsToCall.Add("player_playNannyWalking");
		methodsToCall.Add("player_moveCharacterToClickedDestination");

		canInterruptBy.Add("playNannyCockroach");
		canInterruptBy.Add("moveCharacterToClickedDestination");
		methodsAfterInterrupt.Add("player_stopPlayerMovement");
		
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}
	GameObject selectFireToExtinguish(){
		if (hit.transform == null) {
			GameObject[] fires = GameObject.FindGameObjectsWithTag("fire");
			if (fires.Length == 0) {
				return null;
			}else{
				GameObject closestFire;
				closestFire = findClosestFire
					(
						fires
						);
				return closestFire;
			}
		}else{
			return hit.transform.gameObject;
		}
	}

	GameObject findClosestFire(GameObject[] fireObjects) {
		ArrayList gos = new ArrayList();
		gos.AddRange (fireObjects);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			if(!go.GetComponent<fire>().isFireEnabled)
			{
				continue;
			}
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}
	void removeFire(){
		StartCoroutine ("removingFire");
	}

	IEnumerator removingFire(){
		GameObject fire = selectFireToExtinguish ();
		if (fire == null || fire.transform.tag == "fire") {
			yield return new WaitForSeconds(0f); 
		}
		Vector3 position = fire.transform.position - transform.position;
		var newRoot = Quaternion.LookRotation (position);
		transform.rotation = Quaternion.Lerp (transform.rotation, newRoot, 0.1f);

		playAnimation("nanny_fireoff", 0.6f);
		extinguisher.SetActive(true);
		yield return new WaitForSeconds(0.7f);
		extinguisher.SetActive(false);
		if (fire == null || fire.transform.tag == "fire") {
			Destroy (fire);
		}
		eventFinishedCallback("removeFire");
		//startRemoveFire ();

	}

	IEnumerator executeExtinguisher() {
		playAnimation("nanny_fireoff", 0.6f);
		extinguisher.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		extinguisher.SetActive(false);
		eventFinishedCallback("executeExtinguisherEvent");
	}

	void executeExtinguisherEvent(){
		StartCoroutine ("executeExtinguisher");
	}

	void moveCharacterToClickedDestination(){
		agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
		hasPath = true;
	}

	void moveCharacterToCockroach(){
		moveCharacterToClickedDestination ();
	}

	void destinationReachedLogic () {

		if (
			(agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && hasPath)
			|| (Vector3.Distance (transform.position, agent.destination) == 0.4f)
		    ) {
			hasPath = false;
			eventFinishedCallback("moveCharacterToClickedDestination");
			eventFinishedCallback("moveCharacterToCockroach");
		}
	}

	void removeBrokenGlass () {
		if(hit.transform != null) {
			hit.transform.gameObject.GetComponent<dangerItem>().parent.GetComponent<dangerFurni>().dangerDropped = false;
			StartCoroutine ("sweepingAnimationAndRemoveBrokenGlass");
		}
	}

	IEnumerator sweepingAnimationAndRemoveBrokenGlass(){
		if (hit.transform != null) {

			playAnimation("nanny_sweeping", 1.5f);
			broom.SetActive(true);
			yield return new WaitForSeconds(0.7f);
			
			broom.SetActive(false);

			if(hit.transform.name == "brokenGlass") {
				Destroy(hit.transform.gameObject);
				eventFinishedCallback("removeBrokenGlass");
			} else if (hit.transform.name == "brokenJar") {
				Destroy(hit.transform.gameObject);
				eventFinishedCallback("removeBrokenJar");
			}
		
		}
	}

	void nannyTakeOut(){
		StartCoroutine ("playTakeOut");
	}

	IEnumerator playTakeOut(){
		playAnimation("nanny_takeout", 0.5f);
		transform.LookAt (hit.transform);
		yield return new WaitForSeconds(0.7f);
		eventFinishedCallback("nannyTakeOut");
	}

	void removeBrokenJar () {
		StartCoroutine ("sweepingAnimationAndRemoveBrokenGlass");;
	}

	public void playNannyWalking() {
		gameObject.GetComponent<Animation> ().Stop ();
		playAnimation("nanny_walking", 3f);
		eventFinishedCallback("playNannyWalking");
	}

	public void playNannyIdle() {
		gameObject.GetComponent<Animation> ().Stop ();
		playAnimation("nanny_idle", 0.3f);
		eventFinishedCallback("playNannyIdle");
	}

	public void playNannyCockroach() {
		playAnimation("nanny_cockroach", 1f);
		eventFinishedCallback("playNannyCockroach");
	}

	public void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}


	void stopPlayerMovement(){
		agent.Stop ();
		agent.ResetPath ();
		hasPath = false;
		eventFinishedCallback("stopPlayerMovement");
	}

	void helpBurning() {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		methodsToCall.Add("child_cancelBurning");
		methodsToCall.Add("child_stopChildMovement");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void OnCollisionStay(Collision collision) {
		if (hit.transform != null) {
			ArrayList canInterruptBy = new ArrayList();
			ArrayList methodsToCall = new ArrayList();
			ArrayList methodsAfterInterrupt = new ArrayList();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList();
			
			switch (collision.transform.name) {
			case "madLady":
				if(hit.transform.tag == "madLady") {
					methodsToCall.Add ("player_stopPlayerMovement");
					methodsToCall.Add ("player_nannyTakeOut");
					methodsToCall.Add ("player_playNannyIdle");
					methodsToCall.Add ("madLady_moveToInitialPosition");
					methodsToCall.Add ("madLady_destroyMadLady");
					
					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;

			case "child":
				if(hit.transform.tag == "child") {
					methodsToCall.Add ("player_stopPlayerMovement");
					methodsToCall.Add("player_executeExtinguisherEvent");
					methodsToCall.Add("player_playNannyIdle");
					methodsToCall.Add("child_fireOff");
					methodsToCall.Add("child_startChildRandomMovementEvent");

					canInterruptBy.Add("moveCharacterToClickedDestination");
					canInterruptBy.Add("goToPlayer");
					canInterruptBy.Add("removeBrokenGlass");
					canInterruptBy.Add("removeBrokenJar");

					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;
			}
		}
	}

	void OnTriggerStay(Collider collision) {

		if (collision.transform != null) {
			ArrayList canInterruptBy = new ArrayList ();
			ArrayList methodsToCall = new ArrayList ();
			ArrayList methodsAfterInterrupt = new ArrayList ();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList ();

			switch (collision.transform.tag) {
			case "brokenGlass":
					if (hit.transform.name == "brokenGlass" && hit.transform.name == "brokenGlass") {
							methodsToCall.Add ("player_stopPlayerMovement");
							methodsToCall.Add ("player_playNannyIdle");
							methodsToCall.Add ("player_removeBrokenGlass");
					} else if (hit.transform.name == "brokenJar" && hit.transform.name == "brokenJar") {
							methodsToCall.Add ("player_stopPlayerMovement");
							methodsToCall.Add ("player_playNannyIdle");
							methodsToCall.Add ("player_removeBrokenJar");
					}
					methodsToCall.Add ("player_playNannyIdle");

					canInterruptBy.Add ("moveCharacterToClickedDestination");
					canInterruptBy.Add ("goToPlayer");

					eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

					break;

			case "fire":
				if (hit.transform.tag == "fire") {
					methodsToCall.Add ("player_removeFire");
					methodsToCall.Add ("player_playNannyIdle");

					canInterruptBy.Add ("moveCharacterToClickedDestination");
					canInterruptBy.Add ("goToPlayer");
					canInterruptBy.Add ("removeBrokenGlass");
					canInterruptBy.Add ("removeBrokenJar");

					eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

				}
				break;
			}
		}
	}

}
