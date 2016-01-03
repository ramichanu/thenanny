using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class lightningStorm : MonoBehaviour {

	GameObject tree;
	Vector3 pointa = Vector3.zero;
	Vector3 pointb = Vector3.zero;
	Vector3 pointc = Vector3.zero;

	public GameObject flash = null;

	// Use this for initialization
	void Start () {
		GameObject.Find ("weather").GetComponent<Weather> ().raining (true);
		GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem>().addDangerAlerts("storm");
		GameObject directionalLight = GameObject.Find ("directionalLight");
		//GameObject playerLight = GameObject.Find ("playerLight");

		directionalLight.GetComponent<Light> ().intensity = 0.2f;
		//playerLight.GetComponent<Light> ().enabled = true;
		InvokeRepeating("launchLightning", 1, 1f);
		InvokeRepeating ("startLaunchFlashStorm", 1, 7f);
	}

	// Update is called once per frame
	void Update () {
	
	}

	public class VertexConnection
	{
		public List<int> connections = new List<int>();
	}
	void launchLightning() {
		GameObject[] lightnings = GameObject.FindGameObjectsWithTag("lightning");
		if (lightnings.Length < 1) {
			Mesh mesh = (Mesh)GameObject.Find ("lightningTerrain").GetComponent<MeshFilter> ().mesh;
			mesh.RecalculateBounds ();
			Vector3[] vertices = mesh.vertices;
			GameObject lightning = Instantiate (Resources.Load ("scenary/lightning"), Vector3.zero, Quaternion.identity) as GameObject;
			Vector3 randomVertice = vertices [Random.Range (0, vertices.Length)];
			lightning.transform.name = "lightning" + Random.Range (1, 50);
			bool playerOutside = isPlayerOutside ();
			bool isRayHitsChild = Random.Range (1, 20) == 5;
			if (playerOutside && isRayHitsChild) {
				lightning.transform.position = GameObject.Find ("child").transform.TransformPoint (randomVertice);

			} else {
				lightning.transform.position = GameObject.Find ("lightningTerrain").transform.TransformPoint (randomVertice);
			}


			lightning.transform.SetParent (transform);
			StartCoroutine (removeLightning (lightning));

		}
	}

	IEnumerator removeLightning(GameObject lightning) {
		yield return new WaitForSeconds (0.5f);
		//Debug.Log("NO Rayito");
		Destroy (lightning);
	}

	bool isPlayerOutside(){
		GameObject child = GameObject.Find("child");
		return child.GetComponent<ChildControllerNew> ().isOutside;
	}

	void OnDestroy() {
		CancelInvoke ();
		flash = GameObject.Find ("flashLightning");
		GameObject.Find ("weather").GetComponent<Weather> ().raining (false);
		flash.GetComponent<Light> ().intensity = 0f;
	}

	void startLaunchFlashStorm(){
		int secondsDelay = Random.Range (2, 15);
		Invoke ("launchFlashStorm", secondsDelay);
	}

	void launchFlashStorm(){
		StartCoroutine (flashStorm());
	}

	IEnumerator flashStorm(){

		StartCoroutine(lightningFlash (0.1f));
		yield return new WaitForSeconds (0.1f);
		StartCoroutine(lightningFlash (1f));
	}

	IEnumerator lightningFlash(float fadeTime){
		flash = GameObject.Find ("flashLightning");
		float time = 0f;
		float fadeStart = 8f;
		float fadeEnd = 0f; 

		while (time < fadeTime) {
			time += Time.deltaTime;
			flash.GetComponent<Light> ().intensity = Mathf.Lerp(fadeStart, fadeEnd, time / fadeTime);
			yield return null;
		}
	}
}
