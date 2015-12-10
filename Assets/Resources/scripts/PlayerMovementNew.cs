using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovementNew : EventScript {
	public RaycastHit hit;
	NavMeshAgent agent;

	public GameObject extinguisher = null;
	public GameObject broom = null;

	CharacterController charCtrl;

	bool hasPath = false;
	public bool isClickEnabled = true;
	public bool hasBabyBottle = false;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		charCtrl = GetComponent<CharacterController>();
	}
	

	void Update () {
		if (Input.GetMouseButtonDown (0) && isClickEnabled) {
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
						canInterruptBy.Add("takeBabyBottle");

						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("player_moveCharacterToClickedDestination");
						methodsToCall.Add("player_playNannyIdle");

						methodsAfterInterrupt.Add("player_stopPlayerMovement");

						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;

					case "fire":
						
						methodsToCall.Add("player_startFireExtinguish");
						canInterruptBy.Add("playNannyCockroach");
						canInterruptBy.Add ("removeFire");
						methodsAfterInterrupt.Add("player_stopPlayerMovement");
						
						
						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;
					case "cockroach":
						GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().hit = hit.transform.gameObject;
						methodsToCall.Add("cockroach_stopCockroachMovement");
						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("player_moveCharacterToCockroach");
						methodsToCall.Add("cockroachManager_destroyCockroach");
						methodsToCall.Add("player_stopPlayerMovement");
						methodsToCall.Add("player_playNannyIdle");

						canInterruptBy.Add ("goToPlayer");
						methodsDisabledUntilEventFinished.Add ("player_moveCharacterToCockroach");
						methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;
					case "child":
						methodsToCall.Add("child_createChildMenu");

						methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
						methodsDisabledUntilEventFinished.Add ("player_nannyTakeOut");
						methodsDisabledUntilEventFinished.Add ("child_helpBurning");
						methodsDisabledUntilEventFinished.Add ("child_fireOff");
						methodsDisabledUntilEventFinished.Add ("child_createChildMenu");
						
						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						break;
					case "madLady":
						methodsToCall.Add("madLady_createMadladyMenu");
						methodsDisabledUntilEventFinished.Add ("player_nannyTakeOut");
						methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");

						eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

					break;
					case "babyBottle":
					hit.point = hit.transform.position + transform.forward * 0.1f;
						canInterruptBy.Add("moveCharacterToClickedDestination");
						canInterruptBy.Add("goToPlayer");
						canInterruptBy.Add("removeBrokenGlass");
						canInterruptBy.Add("removeBrokenJar");
						canInterruptBy.Add("takeBabyBottle");
						
						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("player_moveCharacterToClickedDestination");
						methodsToCall.Add("player_playNannyIdle");
						methodsToCall.Add("player_playNannyTakeBabyBottle");
						methodsToCall.Add ("player_takeBabyBottle");
						methodsToCall.Add ("player_playNannyIdle");
						
						methodsAfterInterrupt.Add("player_stopPlayerMovement");
						
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
		canInterruptBy.Add("removeFire");
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
		eventFinishedCallback("removeFire");
		if (fire == null || fire.transform.tag == "fire") {
			Destroy (fire);
		}

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

	void enableClick() {
		isClickEnabled = true;
		eventFinishedCallback("enableClick");
	}

	void disableClick() {
		isClickEnabled = false;
		eventFinishedCallback("disableClick");
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
			StartCoroutine (sweepingAnimationAndRemoveBrokenGlass(hit.transform.gameObject));
		}
	}

	IEnumerator sweepingAnimationAndRemoveBrokenGlass(GameObject hit){
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
		StartCoroutine (sweepingAnimationAndRemoveBrokenGlass(hit.transform.gameObject));
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

	public void playNannyTakeBabyBottle() {
		Vector3 targetPostition = new Vector3( hit.transform.position.x, 
		                                      this.transform.position.y, 
		                                      hit.transform.position.z ) ;
		this.transform.LookAt( targetPostition ) ;
		gameObject.GetComponent<Animation> ().Stop ();
		playAnimation("nanny_takeBabyBottle", 1f);
		eventFinishedCallback("playNannyTakeBabyBottle");
	}

	public void playNannyCockroach() {
		playAnimation("nanny_cockroach", 1f);
		//eventFinishedCallback("playNannyCockroach");
	}
	public void playNannyCockroachFinishEvent() {
		playAnimation("nanny_cockroach", 1f);
		eventFinishedCallback("playNannyCockroachFinishEvent");
	}

	public void eventToClose() {
		eventFinishedCallback("eventToClose");
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

	/*void helpBurning() {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		methodsToCall.Add("child_cancelBurning");
		methodsToCall.Add("child_stopChildMovement");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}*/



	void feedChild() {
		GameObject babyBottleIcon = GameObject.Find ("babyBottleIcon");
		babyBottleIcon.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("imgs/hub/babyBottleNOT");
		playNannyCockroach ();
		hasBabyBottle = false;
		GameObject life = GameObject.Find ("lifeAndHunger");
		life.GetComponent<LifeAndHunger> ().resetHunger();
		eventFinishedCallback("feedChild");
	}

	void takeBabyBottle() {
		GameObject babyBottleIcon = GameObject.Find ("babyBottleIcon");
		babyBottleIcon.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("imgs/hub/babyBottle");

		hasBabyBottle = true;
		eventFinishedCallback("takeBabyBottle");
	}

	void OnCollisionStay(Collision collision) {
		if (hit.transform != null) {
			ArrayList canInterruptBy = new ArrayList();
			ArrayList methodsToCall = new ArrayList();
			ArrayList methodsAfterInterrupt = new ArrayList();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList();
			
			switch (collision.transform.name) {
			case "madLady":
				int madLadyState = collision.transform.gameObject.GetComponent<MadladyNew>().state;
				if(hit.transform.tag == "madLady" && madLadyState == MadladyNew.WAITING_TO_TAKEOUT) {
					methodsToCall.Add ("player_stopPlayerMovement");
					methodsToCall.Add ("player_nannyTakeOut");
					methodsToCall.Add ("player_enableClick");
					methodsToCall.Add ("player_playNannyIdle");
					methodsToCall.Add ("madLady_moveToInitialPosition");
					methodsToCall.Add ("madLady_destroyMadLady");

					methodsDisabledUntilEventFinished.Add ("madLady_stopMadladyMovement");
					methodsDisabledUntilEventFinished.Add ("createMadladyMenu");
					
					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;

			case "child":
				if(hit.transform.tag == "child" && hit.transform.GetComponent<ChildControllerNew>().state == ChildControllerNew.BURNING) {
					methodsToCall.Add ("player_stopPlayerMovement");
					methodsToCall.Add("player_executeExtinguisherEvent");
					methodsToCall.Add("player_playNannyIdle");
					methodsToCall.Add("child_fireOff");
					methodsToCall.Add("child_startChildRandomMovementEvent");

					canInterruptBy.Add("moveCharacterToClickedDestination");
					canInterruptBy.Add("goToPlayer");
					canInterruptBy.Add("removeBrokenGlass");
					canInterruptBy.Add("removeBrokenJar");
					canInterruptBy.Add("createMadladyMenu");

					methodsAfterInterrupt.Add("child_startChildRandomMovementEvent");

					methodsDisabledUntilEventFinished.Add ("madLady_followChild");
					methodsDisabledUntilEventFinished.Add ("child_createChildMenu");

					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;
			}
		}
	}

	void OnTriggerStay(Collider collision) {
		bool isShortDistance = Vector3.Distance (collision.transform.position, transform.position) < 0.5f;
		if (collision.transform != null) {
			ArrayList canInterruptBy = new ArrayList ();
			ArrayList methodsToCall = new ArrayList ();
			ArrayList methodsAfterInterrupt = new ArrayList ();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList ();

			switch (collision.transform.tag) {
			case "brokenGlass":
				if(isShortDistance && hit.transform == collision.transform) {
					
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
					
					methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
					methodsDisabledUntilEventFinished.Add ("madLady_goToPlayer");
					methodsDisabledUntilEventFinished.Add ("player_removeBrokenGlass");
					methodsDisabledUntilEventFinished.Add ("player_removeBrokenJar");
					methodsDisabledUntilEventFinished.Add ("player_startFireExtinguish");
					methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");
					
					eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;

			case "fire":
				if(hit.transform != null) {
					if (hit.transform.tag == "fire" && hit.transform == collision.transform) {
						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("player_moveCharacterToClickedDestination");
						methodsToCall.Add ("player_removeFire");
						methodsToCall.Add ("player_playNannyIdle");
						
						methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
						//methodsDisabledUntilEventFinished.Add ("madLady_goToPlayer");
						methodsDisabledUntilEventFinished.Add ("player_removeBrokenGlass");
						methodsDisabledUntilEventFinished.Add ("player_removeBrokenJar");
						methodsDisabledUntilEventFinished.Add ("player_startFireExtinguish");
						methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");

						
						eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
						
					}
				}
				break;
			
			}
		}
	}


}
