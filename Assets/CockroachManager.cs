using UnityEngine;
using System.Collections;

public class CockroachManager : EventScript {

	public GameObject hit;
	public bool isCockroachAnnoying = false;
	// Use this for initialization
	void Start () {
		GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem>().addDangerAlerts("cockroach");
		InvokeRepeating ("instanciateCockroach", 0f, 15f);
		NotificationCenter.DefaultCenter.AddObserver(this, "cockroachReachsTarget");
		//Invoke ("instanciateCockroach", 2f);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Vector3 getRandomHomePosition() {
		GameObject terrain = GameObject.FindWithTag ("terrainHome");
		float xTerrainMin = terrain.GetComponent<Renderer>().bounds.min.x;
		float xTerrainMax = terrain.GetComponent<Renderer>().bounds.max.x;
		float zTerrainMin = terrain.GetComponent<Renderer>().bounds.min.z;
		float zTerrainMax = terrain.GetComponent<Renderer>().bounds.max.z;
		Vector3 position = new Vector3(Random.Range(xTerrainMin, xTerrainMax), 0, Random.Range(zTerrainMin, zTerrainMax));
		NavMeshHit hit2;
		NavMesh.SamplePosition(position, out hit2, 10f, 1);
		position = hit2.position;
		return position;
	}
	
	void instanciateCockroach(){
		Vector3 randomPosition = getRandomHomePosition ();
		GameObject cockroach = Instantiate(Resources.Load("scenary/cockroach")) as GameObject;
		cockroach.name = "cockroach";
		cockroach.transform.position = randomPosition;
	}

	
	public void destroyCockroach(){

		StartCoroutine ("playInsecticideAndDestroyCockroach");

	}

	IEnumerator playInsecticideAndDestroyCockroach(){
		GameObject.Find ("player").GetComponent<PlayerMovementNew>().insecticide.SetActive(true);
		GameObject.Find ("player").GetComponent<PlayerMovementNew>().playAnimation("nanny_kill_cockroach", 0.7f);

		yield return new WaitForSeconds(3f);

		GameObject.Find ("player").GetComponent<PlayerMovementNew>().insecticide.SetActive(false);

		Destroy (hit);
		StopAllCoroutines ();
		isCockroachAnnoying = false;

		GameObject[] cockroachs = GameObject.FindGameObjectsWithTag ("cockroach");
		if (cockroachs.Length == 1) {
			CancelInvoke ("instanciateCockroach");
			Destroy (gameObject);
		}

		eventFinishedCallback("destroyCockroach");
	}

	public void gotoPlayerEvent() {
		if (!isCockroachAnnoying) {
				ArrayList canInterruptBy = new ArrayList ();
				ArrayList methodsAfterInterrupt = new ArrayList ();
				ArrayList methodsDisabledUntilEventFinished = new ArrayList ();
				ArrayList methodsToCall = new ArrayList ();
	
				methodsToCall.Add("player_playNannyCockroachFinishEvent");
				methodsToCall.Add ("cockroachManager_goToPlayer");
				methodsToCall.Add ("player_playNannyCockroach");	

				canInterruptBy.Add ("destroyCockroach");
				methodsAfterInterrupt.Add("player_eventToClose");
	
				methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
				methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");
				methodsDisabledUntilEventFinished.Add ("child_createChildMenu");
				methodsDisabledUntilEventFinished.Add ("child_repair");
				methodsDisabledUntilEventFinished.Add ("cockroachManager_goToPlayer");

				eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		}
	}

	void goToPlayer(){

		isCockroachAnnoying = true;
		hit.GetComponent<CockroachNew> ().goToPlayer ();
	}

	protected void cockroachReachsTarget(Notification options){
		
		eventFinishedCallback("goToPlayer");
	}


}
