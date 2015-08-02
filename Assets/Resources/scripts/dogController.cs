using UnityEngine;
using System.Collections;

public class dogController : MonoBehaviour {
	const int WALK = 0;
	const int STOP = 1;
	const int FOLLOW = 2;

	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	public bool isRunning = false;
	public Transform target;
	public int state = -1;
	
	void Start () {
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();

	}
	
	void Update () {
		if (!isRunning) {
			state = Random.Range (0, 2);
		}
		switch (state) {
		case WALK:
			if (!isRunning) {
				randomMovement ();
			}
			break;
		case STOP:
			if (!agentHasPath && !isRunning) {
				StartCoroutine(stopFewSeconds());
				StopCoroutine(stopFewSeconds());
			}
			break;
		case FOLLOW:
			agent.Resume ();
			isRunning = false;
			int huidaState = 2;

			if(target.GetComponent<catController>().state == huidaState)
			{
				playAnimation("dog_running", 1.5f);

				agent.SetDestination(target.position);
				isRunning = true;
			} else {
				playAnimation("dog_idle", 1.5f);
				isRunning = false;
			}

			break;
		}
	}
	
	void randomMovement() {
		if (agent.pathStatus == NavMeshPathStatus.PathComplete && !agentHasPath) {
			playAnimation("dog_walking", 1.5f);
			agentHasPath = true;
		}
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			playAnimation("dog_idle", 0.6f);
			agentHasPath = false;
			agent.ResetPath();
			setRandomPosition();
		}
	}
	
	void setRandomPosition() {
		randomPosition = getRandomMeshPosition ();
		agent.SetDestination (randomPosition);
	}

	void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
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
	
	IEnumerator stopFewSeconds(){
		isRunning = true;
		int seconds = Random.Range (2, 5);
		agent.Stop ();
		agentHasPath = false;
		yield return new WaitForSeconds(seconds);
		agent.Resume ();
		isRunning = false;
	}
}
