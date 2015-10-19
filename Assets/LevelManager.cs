﻿using UnityEngine;
using System.Collections;

public class LevelManager : EventScript {

	// Use this for initialization
	void Start () {
		Invoke ("startCatAndDogPersecution", 2f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startCatAndDogPersecution() {
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("cat_stopCatRandomMovement");
		methodsToCall.Add("cat_startCatRunaway");
		methodsToCall.Add("dog_stopDogRandomMovement");
		methodsToCall.Add("dog_startDogFollowRunning");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void stopCatAndDogPersecution (){
		ArrayList canInterruptBy = new ArrayList();

		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("cat_stopCatRunaway");
		methodsToCall.Add("dog_stopDogRandomMovement");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void cockroachPlague() {

	}

	void madLady(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("madLady_startFollowChildEvent");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}



}
