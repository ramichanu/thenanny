﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChildControllerNew : EventScript {
	public const int IDLE = 0;
	public const int BURNING = 1;
	public const int ELECTRIFYING = 2;
	public const int STARVING = 3;

	public int state = IDLE;
	public NavMeshAgent agent;
	bool agentHasPath;
	public bool isOutside = false;
	Vector3 randomPosition;
	Transform target;
	ArrayList danger;


	// Use this for initialization
	void Start () {
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();

		startChildRandomMovementEvent ();
	}
	
	// Update is called once per frame
	void Update () {
		randomMovement ();
		danger = getDangerElements ();
	}

	void startChildRandomMovementEvent(){
		if (state == ELECTRIFYING) {
			eventFinishedCallback("startChildRandomMovementEvent");
		}
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		canInterruptBy.Add("helpBurning");
		methodsToCall.Add("child_startChildRandomMovement");
		methodsAfterInterrupt.Add("child_stopChildMovement");

		base.eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("startChildRandomMovementEvent");
	}
	
	void startChildRandomMovement(){
		InvokeRepeating("childRandomMovement", 0, 1);
		eventFinishedCallback("startChildRandomMovement");
	}

	void childRandomMovement(){
		if (!agentHasPath) {
			agent.speed = 1.5f;
			playAnimation("child_walking", 2.5f);
			setRandomPosition ();
		}
	}

	void randomMovement() {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			playAnimation("child_idle", 1.5f);
			agent.speed = 1.5f;
			agentHasPath = false;
			agent.ResetPath();
		}
	}

	void setRandomPosition() {
		int random = Random.Range (0, 3);
		randomPosition = getRandomMeshPosition ();
		if (random > 0 && danger.Count > 0) {
			randomPosition = getDangerPosition ();
		}
		agent.SetDestination (randomPosition);
		agentHasPath = true;
	}

	Vector3 getDangerPosition() {
		int dangerPosition = Random.Range (0, danger.Count);
		GameObject dangerFurniDestination = (GameObject)danger [dangerPosition];
		return dangerFurniDestination.transform.position;
	}

	public Vector3 getRandomMeshPosition () {

		string terrainKind = "terrainHome";
		int randomNumberUntil = 5;
		bool isStormLightningActive = GameObject.Find ("lightningStorm") != null;

		if (isStormLightningActive) {
			randomNumberUntil = 1;
		}
		int insideOutsideHomeRandom = Random.Range (1, 10);

		if (insideOutsideHomeRandom > randomNumberUntil && isStormLightningActive) {
			terrainKind = "terrain";
		}

		GameObject terrain = GameObject.FindWithTag (terrainKind);
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

	void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}

	public void hitAndPain(float damage, bool stopChild){

		playAnimation("child_pain", 0.5f);
		
		GameObject life = GameObject.Find ("lifeHungerBar");
		life.GetComponent<NewLifeAndHunger> ().restLife (damage);
		
		StartCoroutine(painEffect(stopChild));
		StopCoroutine(painEffect(stopChild));
	}

	public void hitAndPainElectrify(float damage, bool stopChild){
		
		playAnimation("child_pain", 0.5f);
		
		GameObject life = GameObject.Find ("lifeHungerBar");
		life.GetComponent<NewLifeAndHunger> ().restLife (damage);
		
		StartCoroutine(painEffectElectrify(stopChild));
		StopCoroutine(painEffectElectrify(stopChild));	
	}

	public IEnumerator painEffect(bool stopChild) {
		if (stopChild && state != ELECTRIFYING) {
			stopChildMovement ();
		}

		Material painMaterial = Resources.Load("materials/pain", typeof(Material)) as Material;
		SkinnedMeshRenderer childRenderer = GameObject.Find ("childMesh").GetComponent<SkinnedMeshRenderer> ();
		Material oldMaterial = Resources.Load("materials/baby", typeof(Material)) as Material;
		
		GameObject childIcon = GameObject.Find ("babyIcon");
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon_pain");
		
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon");
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.2f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon_pain");
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.2f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon");
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.2f);

		if (stopChild && state != ELECTRIFYING) {
			startChildRandomMovement ();
		}
	}

	public IEnumerator painEffectElectrify(bool stopChild) {
		if (stopChild && state != ELECTRIFYING) {
			stopChildMovement ();
		}

		Material painMaterial = Resources.Load("materials/baby_electrify", typeof(Material)) as Material;
		SkinnedMeshRenderer childRenderer = GameObject.Find ("childMesh").GetComponent<SkinnedMeshRenderer> ();
		Material oldMaterial = Resources.Load("materials/baby", typeof(Material)) as Material;
		
		GameObject childIcon = GameObject.Find ("babyIcon");
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon_pain");
		
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.1f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon");
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.1f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon_pain");
		childRenderer.material = painMaterial;
		yield return new WaitForSeconds(0.1f);
		childIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/babyIcon");
		childRenderer.material = oldMaterial;
		yield return new WaitForSeconds(0.1f);

		if (stopChild && state != ELECTRIFYING) {
			startChildRandomMovement ();
		}
	}

	ArrayList getDangerElements() {
		ArrayList dangerElements = new ArrayList ();
		GameObject[] brokenGlassElements = GameObject.FindGameObjectsWithTag("brokenGlass");
		GameObject[] fireElements = null;

		GameObject tvTable = GameObject.Find("tvTable");
		GameObject fires = GameObject.Find ("fires");

		if (tvTable != null) {
			if(tvTable.GetComponent<dangerFurni>().dangerDropped == true) {
				GameObject tv = GameObject.Find ("tv");
				dangerElements.Add(tv);
			}
		}

		if (fires != null) {
			fireElements = GameObject.FindGameObjectsWithTag("fire");
			if (fireElements.Length > 0) {
				int fireElementRandom = Random.Range(0, fireElements.Length);
				fireElements = new GameObject[1]{fireElements[fireElementRandom]};
			}
		}

		if (fireElements != null) {
			dangerElements.AddRange (fireElements);
		}

		if (brokenGlassElements != null) {
			//dangerElements.AddRange (brokenGlassElements);
		}

		dangerElements.AddRange (brokenGlassElements);
		return dangerElements;
	}

	void OnTriggerEnter(Collider collision) {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		switch(collision.transform.tag) {
			case "brokenGlass":
				methodsToCall.Add("child_painBrokenGlass");
				eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				break;
			case "fire":
				if(state != BURNING) {
					state = BURNING;
					methodsToCall.Add("child_painFire");
					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
				}
				break;
			case "brokenTv":
				if (collision.transform.parent.GetComponent<dangerFurni>().dangerDropped == true){

					state = ELECTRIFYING;
					methodsToCall.Add("child_painElectrify");
					methodsToCall.Add("child_stopChildMovement");
					methodsToCall.Add("child_stopAgentRandomMovement");

					methodsDisabledUntilEventFinished.Add ("madLady_attackOrYellingChild");

					eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
					
				}
				break;
			case "terrainHome":
				isOutside = false;
			break;

		}
	}

	void painBrokenGlass(){
		hitAndPain(0.07f, false);
		eventFinishedCallback("painBrokenGlass");
	}

	void painFire(){
		ParticleSystem fireChild = transform.FindChild("fireChild").GetComponent<ParticleSystem>();
		if(!fireChild.isPlaying){
			fireChild.Play();
		}
		InvokeRepeating("burningChild", 0, 2);
		eventFinishedCallback("painFire");
	}

	void painElectrify(){
		ParticleSystem electrifyChild = transform.FindChild("electrify").GetComponent<ParticleSystem>();
		if(!electrifyChild.isPlaying){
			electrifyChild.Play();
		}
		InvokeRepeating("electrifyChild", 2, 1);
		eventFinishedCallback("painElectrify");
	}

	void launchElectrifyChildSeconds(){
		StartCoroutine (electrifyChildSeconds(0.5f));

		playAnimation("child_pain", 0.5f);
		
		GameObject life = GameObject.Find ("lifeHungerBar");
		life.GetComponent<NewLifeAndHunger> ().restLife (0.3f);
		
		StartCoroutine(painEffect(true));
		StopCoroutine(painEffect(true));
	}

	IEnumerator electrifyChildSeconds(float seconds){
		ParticleSystem electrifyChild = transform.FindChild("electrify").GetComponent<ParticleSystem>();
		if(!electrifyChild.isPlaying){
			electrifyChild.Play();
		}

		yield return new WaitForSeconds (seconds);

		electrifyChild.Stop();
		electrifyChild.Clear();
	}

	void burningChild(){
		hitAndPain (0.01f, true);	
	}

	void electrifyChild(){
		hitAndPainElectrify (0.01f, false);	
	}

	void stopChildMovement(){
		agent.Stop ();
		agent.ResetPath ();
		agentHasPath = false;
		playAnimation("child_idle", 1.5f);
		CancelInvoke ("childRandomMovement");
		eventFinishedCallback("stopChildMovement");
	}

	void stopAgentRandomMovement() {
		agentHasPath = false;
		CancelInvoke ("childRandomMovement");
		eventFinishedCallback("stopAgentRandomMovement");
	}

	void cancelBurning(){
		CancelInvoke ("burningChild");
		eventFinishedCallback("cancelBurning");
	}

	void helpBurning(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();		

		methodsToCall.Add("player_enableClick");
		methodsToCall.Add("child_stopChildMovement");
		methodsToCall.Add("child_cancelBurning");
		methodsToCall.Add("player_playNannyWalking");
		methodsToCall.Add("player_moveCharacterToClickedDestination");

		canInterruptBy.Add ("fireOff");
		methodsAfterInterrupt.Add("player_stopPlayerMovement");
		methodsAfterInterrupt.Add("child_cancelBurning");
		methodsAfterInterrupt.Add("player_enableClick");

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("helpBurning");
	}

	void helpElectrifying(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		methodsToCall.Add ("player_disableClick");
		methodsToCall.Add ("player_playNannyWalking");
		methodsToCall.Add ("player_moveCharacterToClickedDestination");
		methodsToCall.Add ("child_cancelElectrifying");
		methodsToCall.Add ("child_startChildRandomMovement");
		methodsToCall.Add ("player_playNannyIdle");
		methodsToCall.Add ("player_enableClick");

		methodsAfterInterrupt.Add("player_enableClick");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("helpElectrifying");
	}

	void feed() {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		methodsToCall.Add ("player_disableClick");
		methodsToCall.Add ("child_stopChildMovement");
		methodsToCall.Add ("player_playNannyWalking");
		methodsToCall.Add ("player_moveCharacterToClickedDestination");
		methodsToCall.Add ("player_feedChild");
		methodsToCall.Add ("child_startChildRandomMovement");
		methodsToCall.Add ("player_playNannyIdle");
		methodsToCall.Add ("player_enableClick");

		methodsAfterInterrupt.Add("player_enableClick");
		methodsDisabledUntilEventFinished.Add ("child_createChildMenu");
		methodsDisabledUntilEventFinished.Add ("madLady_createMadladyMenu");

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("feed");
	}

	void createChildMenu() {
		GameObject child = transform.gameObject;
		GameObject.Find ("Canvas").GetComponent<gameFunctions>().createClickMenu(child);
		eventFinishedCallback("createChildMenu");
	}

	void cancelElectrifying() {
		state = IDLE;
		ParticleSystem electrifyChild = transform.FindChild("electrify").GetComponent<ParticleSystem>();
		if(electrifyChild.isPlaying){
			electrifyChild.Stop();
			electrifyChild.Clear();
		}
		CancelInvoke("electrifyChild");
		eventFinishedCallback("cancelElectrifying");
	}

	void fireOff() {
		state = IDLE;
		ParticleSystem fireChild = GameObject.Find("fireChild").GetComponent<ParticleSystem>();
		fireChild.Stop();
		fireChild.Clear();

		eventFinishedCallback("fireOff");
	}

	public void cancelBurningAndElectrifying(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		methodsToCall.Add ("child_cancelElectrifying");
		methodsToCall.Add ("child_fireOff");
		methodsToCall.Add ("child_cancelBurning");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void OnTriggerExit(Collider collision) {
		if (collision.transform.tag == "terrainHome") {
			isOutside = true;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.transform.tag == "lightning") {
			launchElectrifyChildSeconds();
		}
	}

	void goBack(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		methodsToCall.Add ("player_disableClick");
		methodsToCall.Add ("child_stopChildMovement");
		methodsToCall.Add ("player_playNannyWalking");
		methodsToCall.Add ("player_moveCharacterToClickedDestination");
		methodsToCall.Add ("player_playNannyIdle");
		methodsToCall.Add ("child_goBackHome");
		methodsToCall.Add ("player_enableClick");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		eventFinishedCallback("goBack");
	}

	void goBackHome(){
		string terrainKind = "terrainHome";
		
		GameObject terrain = GameObject.FindWithTag (terrainKind);
		float xTerrainMin = terrain.GetComponent<Renderer>().bounds.min.x;
		float xTerrainMax = terrain.GetComponent<Renderer>().bounds.max.x;
		float zTerrainMin = terrain.GetComponent<Renderer>().bounds.min.z;
		float zTerrainMax = terrain.GetComponent<Renderer>().bounds.max.z;
		Vector3 position = new Vector3(Random.Range(xTerrainMin, xTerrainMax), 0, Random.Range(zTerrainMin, zTerrainMax));
		NavMeshHit hit;
		NavMesh.SamplePosition(position, out hit, 10f, 1);
		position = hit.position;

		agent.SetDestination (position);
		agentHasPath = true;

		playAnimation("child_walking", 2.5f);
		eventFinishedCallback("goBackHome");
	}

}
