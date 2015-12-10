using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DangerAlertSystem : MonoBehaviour {

	public ArrayList dangerAlerts = new ArrayList();
	GameObject alertDangerSystem;
	bool firstAlert = true;
	GameObject lastAlertDanger = new GameObject();

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
		if (GameObject.FindGameObjectsWithTag ("fire").Length == 0 && GameObject.Find ("dangerAlert_fire") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("fire");
		}

		if (GameObject.FindGameObjectsWithTag ("madLady").Length == 0 && GameObject.Find ("dangerAlert_madLady") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("madLady");
		}

		if (GameObject.FindGameObjectsWithTag ("brokenGlass").Length == 0 && GameObject.Find ("dangerAlert_brokenGlass") != null) {
			GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().removeDangerAlert ("brokenGlass");
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
		
		if (dangerAlerts.Count > 0 && !firstAlert) {
			alertDanger.transform.position = new Vector3(alertDangerSystem.transform.position.x, lastAlertDanger.transform.position.y - 60, alertDangerSystem.transform.position.z);
		}

		lastAlertDanger = alertDanger;
		firstAlert = false;
	}

	public void removeDangerAlert(string type){
		int dangerAlertArrayPosition = dangerAlerts.IndexOf (type);
		dangerAlerts.Remove (type);
		GameObject dangerAlert = GameObject.Find ("dangerAlert_" + type);

		Vector3 dangerAlertToRemovePosition = dangerAlert.transform.position;
		Destroy(dangerAlert);

		sortAlertsAfterRemove (dangerAlertToRemovePosition, dangerAlertArrayPosition);
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
	
}
