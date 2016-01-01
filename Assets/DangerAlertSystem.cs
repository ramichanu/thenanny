using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DangerAlertSystem : EventScript {

	public ArrayList dangerAlerts = new ArrayList();
	GameObject alertDangerSystem;
	bool firstAlert = true;
	bool needsReturnToOriginalPosition = true;
	GameObject lastAlertDanger = new GameObject();
	string alertClicked = "";

	// Use this for initializatio
	void Start () {
		alertDangerSystem = GameObject.Find("AlertDangerSystem");
		/*dangerAlerts.Add("fire");

		dangerAlerts.Add("fire2");
		printDangerAlerts ();*/
	}
	
	// Update is called once per frame
	void Update () {
		removeAlertsAuto ();
	}

	void removeAlertsAuto() {

		GameObject tv = GameObject.Find ("tvTable");

		if (GameObject.FindGameObjectsWithTag ("fire").Length == 0 && GameObject.Find ("dangerAlert_fire") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("fire");
		}

		if (tv.GetComponent<dangerFurni> ().dangerDropped == false) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("electricity");
		}

		if (GameObject.FindGameObjectsWithTag ("madLady").Length == 0 && GameObject.Find ("dangerAlert_madLady") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("madLady");
		}

		if (GameObject.FindGameObjectsWithTag ("brokenGlass").Length == 0 && GameObject.Find ("dangerAlert_brokenGlass") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("brokenGlass");
		}

		if (GameObject.Find("cockroachManager") == null && GameObject.Find ("dangerAlert_cockroach") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("cockroach");
		}
	}

	public void addDangerAlerts(string type) {
		if (GameObject.Find ("dangerAlert_" + type) == null) {
			dangerAlerts.Add (type);
			printDangerAlert (type);
		}
	}

	void printDangerAlert(string type) {
		GameObject alertDanger = Instantiate(Resources.Load("hub/alertDanger")) as GameObject;
		string sprite = type + "_alert_icon";
		alertDanger.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/" + sprite);
		alertDanger.transform.position = alertDangerSystem.transform.position;
		alertDanger.transform.SetParent (alertDangerSystem.transform);
		alertDanger.transform.name = "dangerAlert_" + type;
		
		if (dangerAlerts.Count > 1 && !firstAlert) {
			alertDanger.transform.position = new Vector3(alertDangerSystem.transform.position.x, lastAlertDanger.transform.position.y - 60, alertDangerSystem.transform.position.z);
		}

		lastAlertDanger = alertDanger;
		firstAlert = false;

		alertDanger.GetComponent<Button> ().onClick.AddListener (() => {
			alertClicked = type;

			ArrayList canInterruptBy = new ArrayList();
			
			ArrayList methodsToCall = new ArrayList();
			methodsToCall.Add("AlertDangerSystem_moveCameraToAlertClicked");
			
			ArrayList methodsAfterInterrupt = new ArrayList();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList();
			
			eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);		
		});
	}

	void moveCameraToAlertClicked() {
		GameObject target = new GameObject();
		switch(alertClicked){
		case "fire":
			GameObject[] fires = GameObject.FindGameObjectsWithTag("fire");
			target = fires[0];
			break;
		case "electricity":
			GameObject[] brokenTv = GameObject.FindGameObjectsWithTag("brokenTv");
			target = brokenTv[0];
			break;
		case "brokenGlass":
			GameObject[] brokenGlasses = GameObject.FindGameObjectsWithTag("brokenGlass");
			target = brokenGlasses[0];
			break;
		case "madLady":
			target = GameObject.Find ("madLady");
			break;
		case "cockroach":
			GameObject[] cockroach = GameObject.FindGameObjectsWithTag("cockroach");
			target = cockroach[0];
			break;
		}
		
		moveCameraToTarget(target);
	}

	public void removeDangerAlert(string type){
		int dangerAlertArrayPosition = dangerAlerts.IndexOf (type);
		dangerAlerts.Remove (type);
		GameObject dangerAlert = GameObject.Find ("dangerAlert_" + type);

		if (dangerAlert != null) {
			Vector3 dangerAlertToRemovePosition = dangerAlert.transform.position;
			Destroy(dangerAlert);
			
			sortAlertsAfterRemove (dangerAlertToRemovePosition, dangerAlertArrayPosition);
		}

	}

	void sortAlertsAfterRemove(Vector3 dangerRemovedPosition, int fromDangerAlertMoving) {
		Vector3 lastObjectPositionSorted = dangerRemovedPosition;
		string dangerToSortName = "dangerAlert_" + dangerAlerts[0];
		GameObject dangerAlertToSort = GameObject.Find (dangerToSortName);
		lastAlertDanger = dangerAlertToSort;

		for (int i = fromDangerAlertMoving; i<= dangerAlerts.Count-1; i++) {
			dangerToSortName = "dangerAlert_" + dangerAlerts[i];
			dangerAlertToSort = GameObject.Find (dangerToSortName);
			StartCoroutine(moveToTargetSlowly(dangerAlertToSort, lastObjectPositionSorted));

			lastObjectPositionSorted = dangerAlertToSort.transform.position;
			lastAlertDanger = dangerAlertToSort;
		}
	}

	IEnumerator moveToTargetSlowly(GameObject objectToMove, Vector3 targetPosition){
		float t = 0f;
		Vector3 initialPosition = objectToMove.transform.position;
		while(t < 1)
		{
			t += Time.deltaTime / 0.5f;
			objectToMove.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
			yield return null;
		}
	}
	
	public void moveCameraToTarget(GameObject target) {
		Transform originalTarget = Camera.main.GetComponent<CameraScript> ().target;
		Camera.main.GetComponent<CameraScript> ().target = null;
		StartCoroutine(moveCameraToTargetSlowly(Camera.main.gameObject, target.transform.tag, originalTarget));
	}

	IEnumerator moveCameraToTargetSlowly(GameObject objectToMove, string targetGameObjectName, Transform originalTarget){
		float t = 0f;
		Vector3 initialPosition = objectToMove.transform.position;
		while(t < 5 && objectToMove.transform.position != originalTarget.position)
		{
			Transform targetTransform = GameObject.FindWithTag (targetGameObjectName).gameObject.transform;
			t += Time.deltaTime / 0.5f;
			objectToMove.transform.position = Vector3.Lerp(initialPosition, targetTransform.position, t);
			yield return null;

		}

		if (needsReturnToOriginalPosition) {
			needsReturnToOriginalPosition = false;
			StopCoroutine("moveCameraToTargetSlowly");
			StartCoroutine(moveCameraToTargetSlowly(objectToMove, originalTarget.tag, originalTarget.transform));
		} else {
			needsReturnToOriginalPosition = true;
			Camera.main.GetComponent<CameraScript> ().target = originalTarget.transform;
			eventFinishedCallback("moveCameraToAlertClicked");
		}

	}
}
