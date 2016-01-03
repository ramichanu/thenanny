using UnityEngine;
using System.Collections;

public class MadladyNew : EventScript {
	const int ATTACK = 0;
	const int YELLING = 1;
	const int ATTACK_YELLING = 2;
	public const int FOLLOW_CHILD = 3;
	public const int WAITING_TO_TAKEOUT = 4;

	public NavMeshAgent agent;
	public int state = FOLLOW_CHILD;
	Vector3 startPoint;
	bool runaway = false;
	bool agentHasPath = false;
	bool childReached = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		startPoint = transform.position;
		state = FOLLOW_CHILD;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startFollowChildEvent(){
		InvokeRepeating("startFollowChild", 0, 0.3f);
		eventFinishedCallback("startFollowChildEvent");
	}
	void startFollowChild(){
		if (!agentHasPath) {
			agentHasPath = true;
			childReached = false;

			ArrayList canInterruptBy = new ArrayList ();
			ArrayList methodsAfterInterrupt = new ArrayList ();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList ();
			ArrayList methodsToCall = new ArrayList ();
			methodsToCall.Add ("madLady_followChild");
			methodsToCall.Add ("child_stopChildMovement");
			methodsToCall.Add ("madLady_stopMadladyMovement");
			methodsToCall.Add ("madLady_attackOrYellingChild");
			methodsToCall.Add ("madLady_stopFewSecondsAndFinish");
			methodsToCall.Add ("child_startChildRandomMovementEvent");
			methodsToCall.Add ("madLady_stopFewSecondsAndFinish");
			methodsToCall.Add ("madLady_closeLogicToCreateAnotherMadladyEvent");
			canInterruptBy.Add ("attackOrYellingChild");
			canInterruptBy.Add ("destroyMadLady");
			methodsAfterInterrupt.Add ("madLady_stopMadladyMovement");
			methodsAfterInterrupt.Add ("child_startChildRandomMovementEvent");


			eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		}
	}

	void followChild(){

		InvokeRepeating("follow", 0, 0.1f);
	}

	void follow(){
		if (!childReached) {
			agent.speed = 2.2f;
			playAnimation("madlady_walking", 1.5f);
			GameObject child = GameObject.Find ("child");
			agent.SetDestination (child.transform.position);
		}
	}
	void stopMadladyMovement(){
		playAnimation("madlady_idle", 0.3f);
		agent.Stop ();
		agent.ResetPath ();
		CancelInvoke ("follow");
		eventFinishedCallback("stopMadladyMovement");
	}


	void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}

	void playAnimationQueued(string animation, float speed){

		AnimationState animationQueued = gameObject.GetComponent<Animation>().PlayQueued(animation);
		animationQueued.speed = 0.3f;
	}

	void attackOrYellingChild(){
		int[] randomChildCollisionActions = new int[] {YELLING, ATTACK, ATTACK, ATTACK, ATTACK_YELLING};
		int actionAgainsChild = randomChildCollisionActions[Random.Range(0, randomChildCollisionActions.Length)];
		switch(actionAgainsChild){
		case YELLING:
			yellingChild();
			break;
		case ATTACK:
			hitChild();
			break;
		case ATTACK_YELLING:
			hitChild(true);
			break;
		}

		eventFinishedCallback ("attackOrYellingChild");

	}

	void yellingChild(){
		playAnimation ("madlady_yelling", 1f);
	}

	void hitChild(bool andYelling = false){

		float damageToChild = 0.07f;
		
		if (andYelling) {
			playAnimation ("madlady_attack_yelling", 0.7f);
		} else {
			playAnimation ("madlady_attack", 0.7f);
		}

		GameObject child = GameObject.Find ("child");
		child.GetComponent<ChildControllerNew>().hitAndPain(damageToChild, false);
		child.GetComponent<ChildControllerNew> ().cancelBurningAndElectrifying ();
	}

	void OnCollisionStay(Collision collision) {
		
		switch (collision.transform.name) {
		case "child":
			if(!childReached) {
				childReached = true;
				eventFinishedCallback("followChild");
			}
			break;
		}
		
	}

	void stopFewSecondsAndFinish(){
		StartCoroutine (delayFewSecondsAndFinish());
	}

	IEnumerator delayFewSecondsAndFinish(){
		playAnimationQueued("madlady_idle", 0.3f);
		int seconds = Random.Range (2, 4);
		yield return new WaitForSeconds(seconds);
		eventFinishedCallback("stopFewSecondsAndFinish");
	}

	void closeLogicToCreateAnotherMadladyEvent(){
		agentHasPath = false;

		eventFinishedCallback("closeLogicToCreateAnotherMadladyEvent");
	}

	void kickOut(){
		eventFinishedCallback("kickOut");
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();		
		methodsToCall.Add ("madLady_stopMadladyMovement");
		methodsToCall.Add ("madLady_setStateWaitingToKickOut");
		methodsToCall.Add ("player_playNannyWalking");
		methodsToCall.Add ("player_moveCharacterToClickedDestination");

		canInterruptBy.Add ("moveCharacterToClickedDestination");
		canInterruptBy.Add ("nannyTakeOut");

		//methodsAfterInterrupt.Add("madLady_followChild");
		//methodsAfterInterrupt.Add ("madLady_stopMadladyMovement");
		//methodsAfterInterrupt.Add ("player_stopPlayerMovement");

		//methodsDisabledUntilEventFinished.Add ("followChild");
		methodsDisabledUntilEventFinished.Add ("child_createChildMenu");
		methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");
		methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void destroyMadLady() {
		eventFinishedCallback("destroyMadLady");

		Destroy (gameObject);
	}

	void moveToInitialPosition(){
		if (!runaway) {
			runaway = true;
			playAnimation("madlady_walking", 1.5f);
			agent.ResetPath ();
			agent.SetDestination(startPoint);
			
			InvokeRepeating ("checkHasReturnedToStartPoint", 0, 0.1f);

		}
	}

	void checkHasReturnedToStartPoint() {
		if(transform.position.x == startPoint.x && transform.position.z == startPoint.z){
			eventFinishedCallback("moveToInitialPosition");
		}
	}

	void createMadladyMenu(){
		GameObject madlady = transform.gameObject;
		GameObject.Find ("Canvas").GetComponent<gameFunctions>().createClickMenu(madlady);
		eventFinishedCallback("createMadladyMenu");
	}

	void enableAgentEvent() {
		StartCoroutine ("enableAgent");

	}

	void setStateWaitingToKickOut(){
		state = WAITING_TO_TAKEOUT;
		eventFinishedCallback("setStateWaitingToKickOut");
	}

	void setStateFollowChild(){
		state = FOLLOW_CHILD;
		eventFinishedCallback("setStateFollowChild");
	}

	IEnumerator enableAgent() {
		yield return new WaitForSeconds(1f);
		agent.enabled = true;
		eventFinishedCallback("enableAgentEvent");
	}


}
