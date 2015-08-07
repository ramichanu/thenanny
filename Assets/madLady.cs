using UnityEngine;
using System.Collections;

public class madLady : MonoBehaviour {

	const int FOLLOW_CHILD = 1;
	const int HIT_CHILD = 2;
	const int PUSH_PLAYER = 3;
	const int RUNAWAY = 4;
	const int YELLING = 5;
	const int STOP = 6;
	const int ATTACK_YELLING = 7;


	GameObject childTarget;
	public NavMeshAgent agent;
	RaycastHit hit;
	
	public int state;
	Vector3 startPoint;
	public bool isRunning = false;

	// Use this for initialization
	void Start () {
		childTarget = GameObject.Find ("child");
		agent = GetComponent<NavMeshAgent> ();
		state = FOLLOW_CHILD;
		startPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {
		case FOLLOW_CHILD:
			playAnimation("madlady_walking", 1.5f);
			followChild();
			break;
		case YELLING:
			int b1 = 2;
			break;
		case STOP:
			if (!gameObject.GetComponent<Animation> ().isPlaying) {
				playAnimation("madlady_idle", 0.3f);
			}
			if(!isRunning){
				isRunning = true;
				int seconds = Random.Range (1, 5);
				StartCoroutine(stopFewSecondsAndReturnToFollowChild(seconds));
			}

			break;
		case RUNAWAY:
			playAnimation("madlady_walking", 1.5f);
			if(!isRunning){
				agent.SetDestination(startPoint);
				isRunning = true;
			}
			if(transform.position.x == startPoint.x && transform.position.z == startPoint.z){
				Destroy(gameObject);
			}
			break;
		}
	}

	void followChild(){
		agent.SetDestination (childTarget.transform.position);

	}

	void OnCollisionStay(Collision collision) {

		switch (collision.transform.name) {
		case "child":
			if(state == FOLLOW_CHILD && !isRunning)
			{
				playAnimation("madlady_idle", 0.3f);
				int[] randomChildCollisionActions = new int[] {YELLING, HIT_CHILD, HIT_CHILD, HIT_CHILD, ATTACK_YELLING};
				int actionAgainsChild = randomChildCollisionActions[Random.Range(0, randomChildCollisionActions.Length)];
				switch(actionAgainsChild){
					case YELLING:
						stopMadLady();
						yellingChild();
					break;
					case HIT_CHILD:
						stopMadLady();
						hitChild();
						break;
					case ATTACK_YELLING:
						stopMadLady();
						hitChild(true);
					break;
				}
			}
			break;
		}
		
	}

	void yellingChild(){
		state = YELLING;
		stopChild ();
		playAnimation ("madlady_yelling", 0.5f);
		int seconds = Random.Range (0, 3);
		StartCoroutine (stopFewSecondsThenMeStopAndChildMoves(seconds));
	}
	void hitChild(bool andYelling = false){
		state = HIT_CHILD;
		stopChild();
		int damageToChild = 2;

		if (andYelling) {
			playAnimation ("madlady_attack_yelling", 0.7f);
		} else {
			playAnimation ("madlady_attack", 1f);
		}

		GameObject.Find("child").GetComponent<childController>().hitAndPain(damageToChild);
		StartCoroutine (stopFewSecondsThenMeStopAndChildMoves(1));
	}

	void stopChild(){
		int waitingChildState = 3;
		GameObject.Find("child").GetComponent<childController>().agent.Stop ();
		GameObject.Find("child").GetComponent<childController>().agent.ResetPath ();
		GameObject.Find("child").GetComponent<childController>().state = waitingChildState;
		GameObject.Find("child").GetComponent<childController>().isRandomState = false;
		GameObject.Find("child").GetComponent<childController>().isRunning = true;
	}
	void startChild(){
		int walkState = 0;
		GameObject.Find("child").GetComponent<childController>().state = walkState;
		GameObject.Find("child").GetComponent<childController>().isRunning = false;
	}

	IEnumerator stopFewSecondsThenMeStopAndChildMoves(int seconds){
		isRunning = true;
		yield return new WaitForSeconds(seconds);
		isRunning = false;
		state = STOP;
		startChild ();
	}

	IEnumerator stopFewSecondsAndReturnToFollowChild(int seconds){
		yield return new WaitForSeconds(seconds);
		if(state != RUNAWAY)
		{
			state = FOLLOW_CHILD;
			isRunning = false;
		}
	}

	void stopMadLady(){
		agent.ResetPath ();
	}

	public void playAnimation(string animation, float speed){
			gameObject.GetComponent<Animation>()[animation].speed = speed;
			gameObject.GetComponent<Animation>().Play(animation);

	}
}
