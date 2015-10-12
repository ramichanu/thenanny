using UnityEngine;
using System.Collections;

public class PlayerMovementNew : MonoBehaviour {
	RaycastHit hit;
	NavMeshAgent agent;
	EventDispatcher eventDisp;

	public GameObject extinguisher = null;

	bool hasPath = false;

	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");
		agent = GetComponent<NavMeshAgent> ();
		eventDisp = EventDispatcher.DefaultEventDispatcher;
	}
	

	void Update () {
		if (Input.GetMouseButtonDown (0)) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			int layerMask = 1 << 8;
			layerMask = ~layerMask;
			bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();

			if (Input.touchCount > 0) {
					isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
			}

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask) && !isOverUI) {
				ArrayList canInterruptBy = new ArrayList();
				ArrayList methodsToCall = new ArrayList();
				ArrayList methodsAfterInterrupt = new ArrayList();
				ArrayList methodsDisabledUntilEventFinished = new ArrayList();

				switch (hit.transform.tag) {
				case "terrainHome":

					canInterruptBy.Add("moveCharacterToClickedDedstination");
					canInterruptBy.Add("goToPlayer");

					methodsToCall.Add("player_playNannyWalking");
					methodsToCall.Add("player_moveCharacterToClickedDestination");
					methodsToCall.Add("player_playNannyIdle");

					methodsAfterInterrupt.Add("player_stopPlayerMovement");
					methodsAfterInterrupt.Add("player_playNannyIdle");

					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
					break;
				case "brokenGlass":

					methodsToCall.Add("player_playNannyWalking");
					methodsToCall.Add("player_moveCharacterToClickedDestination");
					methodsToCall.Add("player_removeBrokenGlass");
					methodsToCall.Add("player_stopPlayerMovement");
					
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
		methodsToCall.Add("player_removeFire");
		methodsToCall.Add("player_playNannyIdle");

		canInterruptBy.Add("playNannyCockroach");
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
		if (fire == null) {
			yield return new WaitForSeconds(0f); 
		}
		playAnimation("nanny_fireoff", 0.6f);
		extinguisher.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		extinguisher.SetActive(false);
		Destroy(fire);
		eventFinishedCallback("removeFire");
		//startRemoveFire ();

	}

	void moveCharacterToClickedDestination(){
		agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
		hasPath = true;
	}

	void moveCharacterToCockroach(){
		moveCharacterToClickedDestination ();
	}

	void destinationReachedLogic () {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && hasPath) {
			hasPath = false;
			eventFinishedCallback("moveCharacterToClickedDestination");
			eventFinishedCallback("moveCharacterToCockroach");
		}
	}

	void removeBrokenGlass () {
		hit.transform.gameObject.GetComponent<dangerItem>().parent.GetComponent<dangerFurni>().dangerDropped = false;
		Destroy(hit.transform.gameObject);
		eventFinishedCallback("removeBrokenGlass");
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
		playNannyIdle ();
		eventFinishedCallback("stopPlayerMovement");
	}



	void eventFinishedCallback(string methodExecuted){
		Hashtable options = new Hashtable ();
		string methodCalled = transform.name + "_" + methodExecuted;
		options.Add ("methodCalled", methodCalled);
		NotificationCenter.DefaultCenter.PostNotification(this, "eventIsFinished", options);
	}
	void executeScript(Notification options){

		if(options.data["objectName"].ToString() == transform.name)
		{
			Invoke(options.data["scriptMethod"].ToString(), 0);
		}
	}

}
