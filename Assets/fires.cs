using UnityEngine;
using System.Collections;

public class fires : MonoBehaviour {

	public ArrayList fireObjects = new ArrayList();
	bool isCurrentFireAdded = false;
	bool removeLastFire = false;
	bool fireProcessing;
	int indexPosition = 1;
	int fireCount = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	public void startFireBehaviour()
	{
		InvokeRepeating("launchFire", 2, 1f);
		InvokeRepeating("removeFirstFireItem", 5, 4f);
	}
	void launchFire(){
		StartCoroutine(fireBehaviour());
	}
	IEnumerator fireBehaviour()
	{
		Vector3 newFirePosition;
		fireProcessing = false;

		if (!fireProcessing && fireObjects.Count > 0) {

			int randomFireElement = Random.Range(0, fireObjects.Count);
			GameObject randomFireObject = (GameObject)fireObjects[randomFireElement];
			if(randomFireObject == null)
			{
				return false;
			}
			Vector3 rendererSize = randomFireObject.GetComponent<Renderer> ().bounds.size;
			Vector3[] positionsAroundFire = {
				new Vector3
				(
					randomFireObject.transform.position.x + rendererSize.x,
					randomFireObject.transform.position.y,
					randomFireObject.transform.position.z
					),
				new Vector3
				(
					randomFireObject.transform.position.x - rendererSize.x,
					randomFireObject.transform.position.y,
					randomFireObject.transform.position.z
					),
				new Vector3
				(
					randomFireObject.transform.position.x,
					randomFireObject.transform.position.y,
					randomFireObject.transform.position.z + rendererSize.z
					),
				new Vector3
				(
					randomFireObject.transform.position.x,
					randomFireObject.transform.position.y,
					randomFireObject.transform.position.z - rendererSize.z
					)
			};

			positionsAroundFire = randomizeArray(positionsAroundFire);
			foreach(Vector3 positionAroundFire in positionsAroundFire) {
				newFirePosition = positionAroundFire;
				StartCoroutine(addFire(newFirePosition));
				if(isCurrentFireAdded)
				{
					break;
				}
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	public IEnumerator addFire(Vector3 position) {
		fireProcessing = true;
		fireCount += 1;
		GameObject fire = Instantiate(Resources.Load("scenary/fire")) as GameObject;
		fire.name = "fire" + fireCount;
		fire.transform.position = position;
		fire.GetComponent<fire>().parent = transform.gameObject;
		fire.transform.SetParent(transform);


		yield return new WaitForSeconds(0.5f);

		if (fire.GetComponent<fire> ().isCollidingWithObstacle == true || isThereFireInThisPosition(fire) == true) {
			fire.GetComponent<MeshRenderer> ().enabled = false;
			Destroy(fire);
			isCurrentFireAdded = false;
		} else {
			fire.GetComponent<MeshRenderer> ().enabled = true;
			fire.GetComponent<fire>().isFireEnabled = true;
			fireObjects.Add(fire);
			isCurrentFireAdded = true;
		}
		fireProcessing = false;
	}

	bool isThereFireInThisPosition(GameObject currentFire) {
		GameObject[] objs = GameObject.FindGameObjectsWithTag("fire");
		GameObject myObject = null;

		foreach (GameObject go in objs) {
			if (go.transform.position == currentFire.transform.position && currentFire != go) {
				return true;
			}
		}
		return false;
	}
	public void removeFirstFireItem(){
		if (fireObjects.Count > 1) {
			GameObject fireToDestroy = (GameObject)fireObjects[0];
			fireObjects.RemoveAt(0);
			Destroy(fireToDestroy);
		}else if (fireObjects.Count == 0){
			GameObject heater = GameObject.Find("heater");
			heater.GetComponent<dangerFurni>().dangerDropped = false;
			if (GameObject.FindGameObjectsWithTag ("fire").Length == 0) {
				CancelInvoke("removeFirstFireItem");
				Destroy (GameObject.Find ("fires"));
			}
		}
	}
	Vector3[] randomizeArray(Vector3[] arrayItems)
	{
		for (int t = 0; t < arrayItems.Length; t++ )
		{
			Vector3 tmp = arrayItems[t];
			int r = Random.Range(t, arrayItems.Length);
			arrayItems[t] = arrayItems[r];
			arrayItems[r] = tmp;
		}

		return arrayItems;
	}

}
