using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class lightningStorm : MonoBehaviour {

	GameObject tree;
	Vector3 pointa = Vector3.zero;
	Vector3 pointb = Vector3.zero;
	Vector3 pointc = Vector3.zero;

	// Use this for initialization
	void Start () {
		GameObject directionalLight = GameObject.Find ("directionalLight");
		GameObject playerLight = GameObject.Find ("playerLight");

		directionalLight.GetComponent<Light> ().intensity = 0.2f;
		playerLight.GetComponent<Light> ().enabled = true;
		InvokeRepeating("launchLightning", 1, 1f);
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
			bool playerFuera = isPlayerOutside ();
			if (isPlayerOutside () && Random.Range (1, 15) == 5) {
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
		Debug.Log("NO Rayito");
		Destroy (lightning);
	}

	bool isPlayerOutside(){
		GameObject player = GameObject.Find("child");
		return player.GetComponent<childController> ().isOutside;
	}
}
