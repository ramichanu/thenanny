using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour {
	const int FIRE_EXTINGUISH = 1;
	public bool babbyBottle;
	public bool isExtinguishFire = false;
	public GameObject currentFireToRemove;
	public string lastChildButtonClick = null;
	public GameObject characterMenu = null;
	bool isNotRefreshingDestination = false;

	NavMeshAgent agent;
	bool agentHasPath;
	RaycastHit hit;
	int state = 0;
	GameObject closestFire = null;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agentHasPath = false;
		babbyBottle = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			if(isNotRefreshingDestination == false)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit)){
						isExtinguishFire = true;
						switch(hit.transform.tag) {
						case "terrain":
						case "brokenGlass":
						case "babyBottle":
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							break;
						case "fire":
							isExtinguishFire = false;
							state = FIRE_EXTINGUISH;
							closestFire = hit.transform.gameObject;
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							break;
						case "child":
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							createChildClickMenu();
							break;
						}

				}
			}
		}
		destinationReachedLogic ();

		switch (state) {
			case FIRE_EXTINGUISH:

			break;
		}
	}

	IEnumerator fireExtinguish(GameObject fire){
		ArrayList fireObjects = GameObject.FindGameObjectWithTag("fires").GetComponent<fires>().fireObjects;

		int fireToRemovePos = fireObjects.IndexOf(fire);
		if (fireToRemovePos != -1) {
			fireObjects.RemoveAt(fireToRemovePos);
			Destroy(fire);
			isExtinguishFire = true;
			yield return new WaitForSeconds(0.3f);
			isExtinguishFire = false;
			fire = findClosestFire
				(
					GameObject.FindGameObjectWithTag("fires").GetComponent<fires>().fireObjects
					);
			currentFireToRemove = fire;

			fireObjects = GameObject.FindGameObjectWithTag("fires").GetComponent<fires>().fireObjects;
			if(fire != null)
			{
				agent.SetDestination(fire.transform.position);
			}
		} else {
			if (fireObjects.Count > 0) {
				fire = findClosestFire
					(
						fire.GetComponent<fire> ().parent.GetComponent<fires> ().fireObjects
						);
				currentFireToRemove = fire;
				agent.SetDestination(fire.transform.position);
			} else {
				state = 0;
			}
		}
	}
	
	GameObject findClosestFire(ArrayList fireObjects) {
		ArrayList gos = fireObjects;
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			if(!go.GetComponent<fire>().isFireEnabled)
			{
				continue;
			}
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	
	void destinationReachedLogic () {
		if (agent.hasPath && !agentHasPath) {
			agentHasPath = true;
		}
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			//Debug.Log ("You have reached your destination");
			agentHasPath = false;
			if (hit.transform != null) {
				if (hit.transform.tag == "objectToInteract") {
					Destroy(hit.transform.gameObject);
				}
			}
		}
	}
	
	void OnTriggerStay(Collider collision) {

		switch (collision.transform.name) {
		case "brokenGlass":
			if (hit.transform.tag == "brokenGlass") {
				collision.gameObject.GetComponent<dangerItem>().parent.GetComponent<dangerFurni>().dangerDropped = false;
				Destroy(collision.transform.gameObject);
			}
			
			break;
		case "babyBottle":
			if (hit.transform.tag == "babyBottle") {
				babbyBottle = true;
			}
			break;
			
		}

		switch (collision.transform.tag) {
			case "fire":
				if(!isExtinguishFire){
					StartCoroutine(fireExtinguish(collision.transform.gameObject));	
				}
			break;

		}
	}

	void createChildClickMenu()
	{
		if(hit.transform != null){
			int burningState = 2;
			ArrayList buttonOptions = new ArrayList();
			if (hit.transform.tag == "child" && hit.transform.GetComponent<childController>().state == burningState) {
				buttonOptions.Add("helpBurning");
			}
			if(hit.transform.tag == "child" && babbyBottle){
				buttonOptions.Add("feed");
			}
			
			Vector3 buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);

			if(buttonOptions.Count > 0){
				//to stop child
				int waitingChildState = 3;
				GameObject.Find("child").GetComponent<childController>().agent.Stop ();
				GameObject.Find("child").GetComponent<childController>().agent.ResetPath ();
				GameObject.Find("child").GetComponent<childController>().state = waitingChildState;
				GameObject.Find("child").GetComponent<childController>().isRandomState = false;
				GameObject.Find("child").GetComponent<childController>().isRunning = true;

				isNotRefreshingDestination = true;
				GameObject canvas = GameObject.Find ("Canvas");
				canvas.GetComponent<gameFunctions> ().pauseGame ();

				characterMenu = new GameObject();
				characterMenu.transform.name = "characterMenu";
				characterMenu.transform.SetParent(canvas.transform, false);
			}
			foreach(string buttonOption in buttonOptions)
			{
				showChildMenuByPositionAndButtonType(buttonPos, buttonOption);
				buttonPos.y += 30;
				
			}


		}
	}
	void showChildMenuByPositionAndButtonType(Vector3 position, string buttonType) {

		OrderedDictionary buttonStringType = new OrderedDictionary ();
		buttonStringType.Add("feed", "Dar biberon");
		buttonStringType.Add("helpBurning", "Echar agua");

		GameObject button = Instantiate(Resources.Load("hub/button")) as GameObject;
		string buttonText = (string)buttonStringType[buttonType];
		button.name = buttonType;
		button.transform.FindChild("Text").GetComponent<Text>().text = buttonText;
		button.transform.SetParent (characterMenu.transform, false); 

		button.GetComponent<Button> ().onClick.AddListener (() => {
			Destroy (GameObject.Find ("characterMenu"));
			lastChildButtonClick = buttonType;

			GameObject canvas = GameObject.Find ("Canvas");
			canvas.GetComponent<gameFunctions> ().pauseGame ();

		});

		button.transform.position = new Vector2 (position.x, position.y);

	}

	void OnCollisionStay(Collision collision) {
		switch (collision.transform.name) {
		case "child":
			if(hit.transform != null){
				if (hit.transform.tag == "child" && lastChildButtonClick != null) {
					switch(lastChildButtonClick){
						case "feed":
						case "helpBurning":
				
						isNotRefreshingDestination = false;
						GameObject.Find ("child").GetComponent<childController>().isRunning = false;
						GameObject.Find ("child").GetComponent<childController>().isRandomState = true;
						Invoke(lastChildButtonClick, 0);
						lastChildButtonClick = null;
							break;
					}
				}
			}
			break;
		}

	}

	void feed(){
		babbyBottle = false;
		GameObject hungerBar = GameObject.Find("hungerBar");
		hungerBar.GetComponent<Image> ().fillAmount = 0;
		hit.transform.gameObject.GetComponent<childController>().hunger = 0;

	}

	void helpBurning(){
		int childWalkState = 0;
		GameObject.Find ("child").GetComponent<childController> ().unsetBurningChild ();
		GameObject.Find ("child").GetComponent<childController> ().state = childWalkState;
		GameObject.Find ("child").GetComponent<childController> ().isRunning = false;
		GameObject.Find ("child").GetComponent<childController> ().isRandomState = false;
	}

}
