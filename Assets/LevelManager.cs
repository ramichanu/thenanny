using UnityEngine;
using System.Collections;

public class LevelManager : EventScript {

	int randomTimeToStart;
	bool isMadLadyInvoke = false;
	// Use this for initialization
	void Start () {
		//Invoke ("startCatAndDogPersecution", 2f);
		//Invoke ("launchDangerMadLady", 2f);
		startLevel1 ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startLevel1() {

		InvokeRepeating ("startCatAndDogIA", 5, 15);
		InvokeRepeating ("startDangerMadLadyIA", 0, 15);
	}

	void startCatAndDogIA() {
		startCatAndDogPersecution ();
		int timeToStopPersecution = Random.Range (5, 10);
		Invoke ("stopCatAndDogPersecution", timeToStopPersecution);
	}

	void startDangerMadLadyIA() {
		if (!isMadLadyInvoke) {
			isMadLadyInvoke = true;
			int randomTime = Random.Range (15, 50);
			Invoke ("dangerMadLadyIA", randomTime);
		}
	}

	void startCatAndDogPersecution() {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		methodsToCall.Add("cat_stopCatRandomMovement");
		methodsToCall.Add("dog_stopDogRandomMovement");
		methodsToCall.Add("cat_startCatRunaway");
		methodsToCall.Add("dog_startDogFollowRunning");

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void stopCatAndDogPersecution (){
		ArrayList canInterruptBy = new ArrayList();

		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("cat_stopCatRunaway");
		methodsToCall.Add("dog_stopDogRandomMovement");
		methodsToCall.Add("cat_startCatRandomMovement");
		methodsToCall.Add("dog_startDogRandomMovement");

		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void cockroachPlague() {

	}

	void madLady(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add ("madLady_enableAgentEvent");
		methodsToCall.Add("madLady_startFollowChildEvent");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void dangerMadLadyIA() {
		isMadLadyInvoke = false;
		GameObject madLady = GameObject.Find ("madLady");

		if (madLady == null) {
			Invoke ("launchDangerMadLady", 2f);
		}
	}

	void launchDangerMadLady(){
		Vector3 portalEnterPosition = getPortalScenaryPosition();
		GameObject madLady = Instantiate(Resources.Load("characters/madLady")) as GameObject;
		madLady.name = "madLady";
		madLady.transform.position = portalEnterPosition;

		this.madLady ();
	}
	
	Vector3 getPortalScenaryPosition(){
		GameObject portalScenary = GameObject.Find ("portalEnter");
		return portalScenary.transform.position;
	}



}
