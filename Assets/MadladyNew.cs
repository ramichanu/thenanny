using UnityEngine;
using System.Collections;

public class MadladyNew : EventScript {
	const int ATTACK = 0;
	const int YELLING = 1;
	const int ATTACK_YELLING = 2;

	public NavMeshAgent agent;
	Vector3 startPoint;
	bool runaway = false;
	bool agentHasPath = false;
	bool childReached = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		startPoint = transform.position;

		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("madLady_startFollowChildEvent");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
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
		playAnimation ("madlady_yelling", 0.5f);
	}

	void hitChild(bool andYelling = false){

		int damageToChild = 2;
		
		if (andYelling) {
			playAnimation ("madlady_attack_yelling", 0.7f);
		} else {
			playAnimation ("madlady_attack", 0.7f);
		}
		
		GameObject.Find("child").GetComponent<ChildControllerNew>().hitAndPain(damageToChild);
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
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();		
		methodsToCall.Add ("madLady_stopMadladyMovement");
		methodsToCall.Add ("player_moveCharacterToClickedDestination");
		methodsToCall.Add ("madLady_moveToInitialPosition");
		methodsToCall.Add ("madLady_destroyMadLady");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("kickOut");
	}

	void destroyMadLady() {
		Destroy (gameObject);
		eventFinishedCallback("destroyMadLady");
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


}
