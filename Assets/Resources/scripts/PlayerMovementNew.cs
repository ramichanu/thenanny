using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovementNew : EventScript {
	public RaycastHit hit;
	NavMeshAgent agent;

	public GameObject extinguisher = null;
	public GameObject broom = null;
	public GameObject wrench = null;
	public GameObject insecticide = null;

	CharacterController charCtrl;

	bool hasPath = false;
	public bool isClickEnabled = true;
	public bool hasBabyBottle = false;
	public bool isCarryingChild = false;

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

				if(isCarryingChild)
				{
					switch (hit.transform.tag) {
					case "player":
					case "brokenGlass":
					case "brokenTv":
					case "terrainHome":
						methodsToCall.Add("player_createPlayerMenu");
						break;
					case "terrain":
						canInterruptBy.Add("moveCharacterToClickedDestination");
						canInterruptBy.Add("goToPlayer");
						canInterruptBy.Add("removeBrokenGlass");
						canInterruptBy.Add("removeBrokenJar");
						canInterruptBy.Add("takeBabyBottle");
						canInterruptBy.Add("repair");
						
						methodsToCall.Add("player_playNannyWalking");
						methodsToCall.Add("camera_moveCamToNanny");
						methodsToCall.Add("player_moveCharacterToClickedDestination");
						//methodsToCall.Add("camera_disableCamFollowPlayer");
						methodsToCall.Add("player_playNannyIdle");
						
						methodsAfterInterrupt.Add("player_stopPlayerMovement");
						break;
					}
					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

				} else {

					switch (hit.transform.tag) {
						case "player":
						case "terrain":
						case "brokenGlass":
						case "brokenTv":
						case "terrainHome":
							canInterruptBy.Add("moveCharacterToClickedDestination");
							canInterruptBy.Add("goToPlayer");
							canInterruptBy.Add("removeBrokenGlass");
							canInterruptBy.Add("removeBrokenJar");
							canInterruptBy.Add("takeBabyBottle");
							canInterruptBy.Add("repair");

							methodsToCall.Add("player_playNannyWalking");
							methodsToCall.Add("camera_moveCamToNanny");
							methodsToCall.Add("player_moveCharacterToClickedDestination");
							//methodsToCall.Add("camera_disableCamFollowPlayer");
							methodsToCall.Add("player_playNannyIdle");

							methodsAfterInterrupt.Add("player_stopPlayerMovement");

							eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
							break;

						case "fire":
							methodsToCall.Add("camera_moveCamToNanny");
							methodsToCall.Add("player_startFireExtinguish");
							canInterruptBy.Add("playNannyCockroach");
							canInterruptBy.Add ("removeFire");
							methodsAfterInterrupt.Add("player_stopPlayerMovement");
							
							
							eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
							break;
						case "cockroach":
							if(GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().hit != hit.transform.gameObject
						   	&& !GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().isCockroachAnnoying) {
						   		methodsToCall.Add("camera_moveCamToNanny");
								methodsToCall.Add("cockroach_stopCockroachMovement");
								methodsToCall.Add("player_playNannyWalking");
								methodsToCall.Add("player_moveCharacterToCockroach");
								methodsToCall.Add("cockroachManager_destroyCockroach");
								methodsToCall.Add("player_stopPlayerMovement");
								methodsToCall.Add("player_playNannyIdle");
								
								canInterruptBy.Add ("goToPlayer");
								methodsDisabledUntilEventFinished.Add ("player_moveCharacterToCockroach");
								methodsDisabledUntilEventFinished.Add ("player_playNannyCockroach");
								methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
								eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

							}
							if(GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().isCockroachAnnoying
						   	&& GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().hit == hit.transform.gameObject){
								methodsToCall.Add("cockroachManager_destroyCockroach");
								methodsToCall.Add("player_stopPlayerMovement");
								methodsToCall.Add("player_playNannyIdle");

								canInterruptBy.Add ("goToPlayer");
								methodsDisabledUntilEventFinished.Add ("player_moveCharacterToCockroach");
								methodsDisabledUntilEventFinished.Add ("player_playNannyCockroach");
								methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
								eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
							}
							break;
						case "child":
							methodsToCall.Add("child_createChildMenu");
							methodsToCall.Add("camera_moveCamToNanny");

							methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
							methodsDisabledUntilEventFinished.Add ("player_nannyTakeOut");
							methodsDisabledUntilEventFinished.Add ("child_helpBurning");
							methodsDisabledUntilEventFinished.Add ("child_fireOff");
							methodsDisabledUntilEventFinished.Add ("child_createChildMenu");
							
							eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
							break;
						case "madLady":
							methodsToCall.Add("madLady_createMadladyMenu");
							methodsToCall.Add("camera_moveCamToNanny");

							methodsDisabledUntilEventFinished.Add ("player_nannyTakeOut");
							methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");

							eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);

						break;
						case "babyBottle":
						hit.point = hit.transform.position + transform.forward * 0.1f;
							methodsToCall.Add("camera_moveCamToNanny");
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
			(agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && hasPath && !agent.pathPending)
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

		if (isCarryingChild) {
			playAnimation("nanny_walking_pickup", 3f);
		} else {
			playAnimation("nanny_walking", 3f);
		}
		eventFinishedCallback("playNannyWalking");
	}

	public void playNannyIdle() {
		gameObject.GetComponent<Animation> ().Stop ();

		if (isCarryingChild) {
			playAnimation("nanny_idle_pickup", 0.3f);
		} else {
			playAnimation("nanny_idle", 0.3f);
		}

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
		hasBabyBottle = false;
		GameObject life = GameObject.Find ("lifeHungerBar");
		life.GetComponent<NewLifeAndHunger> ().resetHunger();
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
					methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");
					
					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;

			case "child":
				GameObject canvas = GameObject.Find ("Canvas");
				bool isLastButtonHelpBurning = canvas.GetComponent<gameFunctions>().lastButtonClicked == "helpBurning";
				if(hit.transform.tag == "child" && hit.transform.GetComponent<ChildControllerNew>().isBurning && isLastButtonHelpBurning) {
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
					if (isShortDistance && hit.transform == collision.transform) {
		
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
					if (hit.transform != null) {
							if (hit.transform.tag == "fire" && hit.transform == collision.transform) {
									methodsToCall.Add ("player_playNannyWalking");
									methodsToCall.Add ("player_moveCharacterToClickedDestination");
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

				case "brokenTv":
					if (hit.transform != null) {
							if (hit.transform.tag == "brokenTv" && hit.transform == collision.transform) {
								if (hit.transform.parent.GetComponent<dangerFurni>().dangerDropped == true){
									//methodsToCall.Add ("player_stopPlayerMovement");
									methodsToCall.Add ("player_disableClick");		
									methodsToCall.Add ("player_repair");
									methodsToCall.Add ("child_cancelElectrifying");
									methodsToCall.Add ("child_startChildRandomMovement");
									methodsToCall.Add ("player_playNannyIdle");
									methodsToCall.Add ("player_putTvCorrectly");
									methodsToCall.Add ("player_enableClick");
		
									methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
									//methodsDisabledUntilEventFinished.Add ("madLady_goToPlayer");
									methodsDisabledUntilEventFinished.Add ("player_removeBrokenGlass");
									methodsDisabledUntilEventFinished.Add ("player_removeBrokenJar");
									methodsDisabledUntilEventFinished.Add ("player_startFireExtinguish");
									methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");
		
									eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
								}
							}
					}
					break;
			}
		}
	}

	void repair() {

		StartCoroutine ("repairing");
	}

	IEnumerator repairing() {

		GameObject tv = GameObject.Find ("tv");
		transform.LookAt (tv.transform);

		playAnimation("nanny_repair", 0.7f);
		wrench.SetActive(true);

		yield return new WaitForSeconds(2.5f);
		GameObject tvTable = GameObject.Find ("tvTable");
		tvTable.GetComponent<dangerFurni> ().dangerDropped = false;
		wrench.SetActive(false);
		eventFinishedCallback("repair");
	}

	void putTvCorrectly(){
		GameObject tvTable = GameObject.Find ("tvTable");
		tvTable.GetComponent<dangerFurni> ().dangerDropped = false;
		tvTable.GetComponent<Animation> ().Stop ();
		tvTable.GetComponent<Animation> ().Rewind();
		tvTable.GetComponent<Animation> () ["tv_falling"].time = 1f;
		tvTable.GetComponent<Animation> () ["tv_falling"].speed = -1.3f;
		tvTable.GetComponent<Animation> ().Play ();
		
		ParticleSystem tvElectricity = GameObject.Find("electricity").GetComponent<ParticleSystem>();
		tvElectricity.Stop();
		tvElectricity.Clear();

		eventFinishedCallback("putTvCorrectly");
	}
	void carryChild() {
		playAnimation("nanny_pickup_child", 0.7f);
		GameObject child = GameObject.Find ("child");
		GameObject pickUpChild = GameObject.Find ("pickUpChild");

		isCarryingChild = true;
		child.GetComponent<NavMeshAgent>().enabled = false;
		child.transform.parent = pickUpChild.transform;
		child.transform.position = pickUpChild.transform.position;
		eventFinishedCallback("carryChild");
	}

	void createPlayerMenu() {
		GameObject child = transform.gameObject;
		GameObject.Find ("Canvas").GetComponent<gameFunctions>().createClickMenu(gameObject);
		eventFinishedCallback("createPlayerMenu");
	}

	void putDown() {
		ArrayList canInterruptBy = new ArrayList ();
		ArrayList methodsToCall = new ArrayList ();
		ArrayList methodsAfterInterrupt = new ArrayList ();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList ();

		methodsToCall.Add("player_playNannyWalking");
		methodsToCall.Add("camera_moveCamToNanny");
		methodsToCall.Add("player_moveCharacterToClickedDestination");
		methodsToCall.Add("player_playNannyPutdown");
		methodsToCall.Add("child_startChildRandomMovement");
		methodsToCall.Add ("player_enableClick");

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("putDown");
	}

	void playNannyPutdown() {
		if(isCarryingChild) {
			StartCoroutine ("putBabyOnTheFloor");
		}
	}

	IEnumerator putBabyOnTheFloor() {
		isCarryingChild = false;

		GameObject child = GameObject.Find ("child");
		GameObject characters = GameObject.Find ("Characters");
		//GetComponent<Animation> ().Stop ();
		//GetComponent<Animation> ().Rewind("nanny_pickup_child");
		GetComponent<Animation> () ["nanny_pickup_child"].time = 0.3f;
		GetComponent<Animation> () ["nanny_pickup_child"].wrapMode = WrapMode.Once;
		GetComponent<Animation> () ["nanny_pickup_child"].speed = -1.5f;
		GetComponent<Animation> ().Play ("nanny_pickup_child");

		yield return new WaitForSeconds (0.3f);

		child.GetComponent<NavMeshAgent>().enabled = true;
		child.transform.parent = characters.transform;
		playNannyIdle ();
		StopAllCoroutines ();

		eventFinishedCallback("playNannyPutdown");
	}

}
