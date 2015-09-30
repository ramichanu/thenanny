using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour {

	const int FIRE_EXTINGUISH = 1;
	const int COCKROACH_ATTACK = 2;

	public bool babbyBottle;
	public bool isExtinguishFire = true;
	public GameObject currentFireToRemove;
	public string lastButtonClick = null;
	public GameObject characterMenu = null;
	bool isNotRefreshingDestination = false;
	bool noFire = false;

	NavMeshAgent agent;
	bool agentHasPath;
	RaycastHit hit;
	int state = 0;
	GameObject closestFire = null;
	public GameObject extinguisher = null;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agentHasPath = false;
		babbyBottle = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetMouseButton (0)) {
			if(isNotRefreshingDestination == false)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				int layerMask = 1 << 8;
				layerMask = ~layerMask;
				bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

				if(Input.touchCount > 0)
				{
					isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
				}

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && !isOverUI){

					if(state == COCKROACH_ATTACK){
						switch(hit.transform.tag) {
							case "cockroach":
							destroyCockroach(hit.transform.gameObject);
							break;
						}
					} else {
						switch(hit.transform.tag) {
						case "terrain":
						case "terrainHome":
						case "brokenGlass":
						case "player":
						case "brokenJar":
						case "brokenTv":
						case "cockroach":
						case "babyBottle":
							StopAllCoroutines();
							agent.ResetPath();
							getExtinguisher(false);
							noFire = true;
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							break;
						case "fire":
							noFire = false;
							isExtinguishFire = false;
							state = FIRE_EXTINGUISH;
							closestFire = hit.transform.gameObject;
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							break;
						case "child":
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							createChildClickMenu();
							break;
						case "madLady":
							agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
							createMadLadyClickMenu();
							break;
						}
					}
				}
			}
		}
		destinationReachedLogic ();

		switch (state) {
			case FIRE_EXTINGUISH:

			break;
			case COCKROACH_ATTACK:
			playAnimation("nanny_cockroach", 1f);
			break;
		}
	}

	IEnumerator fireExtinguish(GameObject fire){
		if (noFire == false && !isExtinguishFire) {
			ArrayList fireObjects = GameObject.FindGameObjectWithTag("fires").GetComponent<fires>().fireObjects;
			
			int fireToRemovePos = fireObjects.IndexOf(fire);
			if (fireToRemovePos != -1 && !isExtinguishFire) {

				
				isExtinguishFire = true;
				var targetRotation = Quaternion.LookRotation(fire.transform.position - transform.position);
				
				transform.LookAt(fire.transform.position);
				yield return new WaitForSeconds(1.5f);
				fireObjects.RemoveAt(fireToRemovePos);
				getExtinguisher(false);
				Destroy(fire);
				playAnimation("nanny_idle", 0.3f);
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
			} else if(!isExtinguishFire) {
				if (fireObjects.Count > 0) {
					getExtinguisher(true);
					fire = findClosestFire
						(
							fire.GetComponent<fire> ().parent.GetComponent<fires> ().fireObjects
							);
					currentFireToRemove = fire;
					agent.SetDestination(fire.transform.position);
				} else {
					getExtinguisher(false);
					playAnimation("nanny_idle", 0.3f);
					state = 0;
				}
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
			playAnimation("nanny_walking", 3f);
			agentHasPath = true;
		}
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {

			playAnimation("nanny_idle", 0.3f);

			//Debug.Log ("You have reached your destination");
			agentHasPath = false;
			if (hit.transform != null) {
				if (hit.transform.tag == "objectToInteract") {
					Destroy(hit.transform.gameObject);
				}
			}
		}
	}

	public void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}
	
	void OnTriggerStay(Collider collision) {

		switch (collision.transform.name) {
		case "brokenGlass":
		case "tv":
		case "brokenJar":
			if(hit.transform != null) {
				if (hit.transform.name == "brokenGlass") {
					collision.gameObject.GetComponent<dangerItem>().parent.GetComponent<dangerFurni>().dangerDropped = false;
					Destroy(collision.transform.gameObject);
				}else if(hit.transform.name == "brokenJar"){
					Destroy(collision.transform.gameObject);
				}else if(hit.transform.name == "tv"){

					collision.gameObject.GetComponent<dangerItem>().parent.GetComponent<dangerFurni>().dangerDropped = false;

					AnimationState tvFalling = GameObject.Find ("tvTable").GetComponent<Animation>()["tv_falling"];
					tvFalling.enabled = true;
					tvFalling.time = 0;
					tvFalling.speed = 0;
					GameObject.Find ("tvTable").GetComponent<Animation>().Play();

					ParticleSystem electricity =  GameObject.Find("electricity").GetComponent<ParticleSystem>();
					electricity.Stop ();
				}
			}
			break;
		case "babyBottle":
			if (hit.transform.tag == "babyBottle") {
				babbyBottle = true;

				Sprite withBabyBottle =  Resources.Load <Sprite>("imgs/hub/babyBottle");
				if (withBabyBottle){
					GameObject.Find ("babyBottleIcon").GetComponent<Image>().sprite = withBabyBottle;
				} else {
					Debug.LogError("Sprite not found", this);
				};
				GameObject.Find ("babyBottleIcon").GetComponent<Image>().sprite = withBabyBottle;
			}
			break;
		case "cockroach":
			if(state != COCKROACH_ATTACK)
			{
				StopAllCoroutines();
				agent.ResetPath();
				collision.gameObject.GetComponent<Cockroach>().startCockroachAttack(transform.position);
				state = COCKROACH_ATTACK;
			}

			break;
			
		}

		switch (collision.transform.tag) {
			case "fire":
				if(!isExtinguishFire){
					
					StartCoroutine(fireExtinguish(collision.transform.gameObject));	
			} else if(noFire == false) {
					agent.ResetPath();
					playAnimation("nanny_fireoff", 0.6f);

					getExtinguisher(true);
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
				StopAllCoroutines();
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
			lastButtonClick = buttonType;

			GameObject canvas = GameObject.Find ("Canvas");
			canvas.GetComponent<gameFunctions> ().pauseGame ();

		});

		button.transform.position = new Vector2 (position.x, position.y);

	}

	void createMadLadyClickMenu(){
		if(hit.transform != null){
			ArrayList buttonOptions = new ArrayList();
			if (hit.transform.tag == "madLady") {
				buttonOptions.Add("attackMadLady");
			}	
			Vector3 buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);
			
			if(buttonOptions.Count > 0){
				//to stop madLady
				int stopMadLadyState = -1;
				StopAllCoroutines();
				GameObject.Find("madLady").GetComponent<madLady>().agent.Stop ();
				GameObject.Find("madLady").GetComponent<madLady>().agent.ResetPath ();
				GameObject.Find("madLady").GetComponent<madLady>().state = stopMadLadyState;
				GameObject.Find("madLady").GetComponent<madLady>().isRunning = false;
				
				isNotRefreshingDestination = true;
				GameObject canvas = GameObject.Find ("Canvas");
				canvas.GetComponent<gameFunctions> ().pauseGame ();
				
				characterMenu = new GameObject();
				characterMenu.transform.name = "characterMenu";
				characterMenu.transform.SetParent(canvas.transform, false);
			}
			foreach(string buttonOption in buttonOptions)
			{
				showMadLadyMenuByPositionAndButtonType(buttonPos, buttonOption);
				buttonPos.y += 30;
				
			}
			
			
		}
	}

	void showMadLadyMenuByPositionAndButtonType(Vector3 position, string buttonType) {
		
		OrderedDictionary buttonStringType = new OrderedDictionary ();
		buttonStringType.Add("attackMadLady", "¡Fuera!");
		
		GameObject button = Instantiate(Resources.Load("hub/button")) as GameObject;
		string buttonText = (string)buttonStringType[buttonType];
		button.name = buttonType;
		button.transform.FindChild("Text").GetComponent<Text>().text = buttonText;
		button.transform.SetParent (characterMenu.transform, false); 
		
		button.GetComponent<Button> ().onClick.AddListener (() => {
			Destroy (GameObject.Find ("characterMenu"));
			GameObject.Find("madLady").GetComponent<madLady>().StopAllCoroutines();
			lastButtonClick = buttonType;
			
			GameObject canvas = GameObject.Find ("Canvas");
			canvas.GetComponent<gameFunctions> ().pauseGame ();
			
		});
		
		button.transform.position = new Vector2 (position.x, position.y);
		
	}

	


	void OnCollisionStay(Collision collision) {
		switch (collision.transform.name) {
		case "child":
			if(hit.transform != null){
				if (hit.transform.tag == "child" && lastButtonClick != null) {
					switch(lastButtonClick){
						case "feed":
						case "helpBurning":
				
						isNotRefreshingDestination = false;
						GameObject.Find ("child").GetComponent<childController>().isRunning = false;
						GameObject.Find ("child").GetComponent<childController>().isRandomState = true;
						Invoke(lastButtonClick, 0);
						lastButtonClick = null;
							break;
					}
				}
			}
			break;

		case "madLady":
			if(hit.transform != null){
				if (hit.transform.tag == "madLady" && lastButtonClick != null) {
					switch(lastButtonClick){
					case "attackMadLady":
						
						isNotRefreshingDestination = false;
						int madLadyStateFollowChild = 1;
						GameObject.Find ("madLady").GetComponent<madLady>().isRunning = false;
						GameObject.Find ("madLady").GetComponent<madLady>().state = madLadyStateFollowChild;
						Invoke(lastButtonClick, 0);
						lastButtonClick = null;
						break;
					}
				}

			}
			isNotRefreshingDestination = false;
			break;

		case "lightning":

			break;
		}


	}

	void feed(){
		babbyBottle = false;

		Sprite withBabyBottle =  Resources.Load <Sprite>("imgs/hub/babyBottleNOT");
		if (withBabyBottle){
			GameObject.Find ("babyBottleIcon").GetComponent<Image>().sprite = withBabyBottle;
		} else {
			Debug.LogError("Sprite not found", this);
		};
		GameObject.Find ("babyBottleIcon").GetComponent<Image>().sprite = withBabyBottle;

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
		ParticleSystem fireChild = GameObject.Find("fireChild").GetComponent<ParticleSystem>();
		fireChild.Stop();
		fireChild.Clear();

	}

	void attackMadLady(){
		int runawayState = 4;
		GameObject.Find("madLady").GetComponent<madLady>().state = runawayState;
	}

	void destroyCockroach(GameObject cockroach){
		Destroy (cockroach);
		playAnimation("nanny_idle", 0.3f);
		state = 0;
	}

	void getExtinguisher(bool showExtinguisher){
		extinguisher.SetActive(showExtinguisher);
	}
	
}
