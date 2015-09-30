using UnityEngine;
using System.Collections;

public class PlayerMovementNew : MonoBehaviour {
	RaycastHit hit;
	NavMeshAgent agent;
	EventDispatcher eventDisp;

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

				switch (hit.transform.tag) {
				case "terrainHome":
					ArrayList canInterruptBy = new ArrayList();
					canInterruptBy.Add("moveCharacterToClickedDedstination");
					canInterruptBy.Add("goToPlayer");

					ArrayList methodsToCall = new ArrayList();
					methodsToCall.Add("player_playNannyWalking");
					methodsToCall.Add("player_moveCharacterToClickedDestination");
					methodsToCall.Add("player_playNannyIdle");

					ArrayList methodsAfterInterrupt = new ArrayList();
					methodsAfterInterrupt.Add("player_stopPlayerMovement");
					methodsAfterInterrupt.Add("player_playNannyIdle");

					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt);
					break;
				}

			}
		}

		destinationReachedLogic ();

	}

	void moveCharacterToClickedDestination(){
		agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
		hasPath = true;
	}

	void destinationReachedLogic () {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && hasPath) {
			hasPath = false;
			eventFinishedCallback("moveCharacterToClickedDestination");
		}
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
