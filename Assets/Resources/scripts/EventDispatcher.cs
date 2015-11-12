using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EventDispatcher : MonoBehaviour {
	
	public Dictionary<string,GameObject> eventList = new Dictionary<string,GameObject>();
	public ArrayList methodsDisabledUntilFinishEvent = new ArrayList();

	// Use this for initialization
	void Start () {

		//eventSystem ();
		//InvokeRepeating ("eventSystem", 0, 2);
	}
	
	// Update is called once per frame
	void Update () {

		eventSystem ();
	}

	private static EventDispatcher defaultEventDispatcher;
	public static EventDispatcher DefaultEventDispatcher {
		get {
			if (!defaultEventDispatcher) {
				GameObject eventDispatcherObject = new GameObject ("Default Event Dispatcher");
				
				defaultEventDispatcher = eventDispatcherObject.AddComponent<EventDispatcher> ();
			}
			
			return defaultEventDispatcher; 
		}
	}
	
	public void addEvent(ArrayList methodsToCall, ArrayList canInterruptBy, ArrayList methodsAfterInterrupt, ArrayList methodsDisabledUntilEventFinished) {

		GameObject eventObject = new GameObject ();
		eventObject.AddComponent<Event>();
		((Event)eventObject.GetComponent<Event>()).init(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		string eventObjectKey = eventObject.GetComponent<Event>().hash;

		bool areThereMethodsDisabledUntilEventFinished = disabledMethodsContainsThisMethods (methodsToCall);
		List<string> keyList = new List<string>(this.eventList.Keys);
		if (eventList.ContainsKey(eventObjectKey) && !areThereMethodsDisabledUntilEventFinished) {

			GameObject eventListItem = (GameObject)(eventList[eventObjectKey]);
			bool canInterrupt = eventListItem.GetComponent<Event>().canInterrupt(methodsToCall);
			if(canInterrupt){
				Destroy(eventListItem);
				eventList.Remove(eventObjectKey);
				eventList.Add (eventObjectKey, eventObject);
			} else {
				Destroy(eventObject);
			}

		} else if(areThereMethodsDisabledUntilEventFinished) {
			Destroy(eventObject);
		} else {
			interrumptEventsByNewEvent(eventObject, eventObjectKey);
			eventList.Add (eventObjectKey, eventObject);
		}

	}

	bool disabledMethodsContainsThisMethods(ArrayList newMethods){
		foreach (string disabledMethod in methodsDisabledUntilFinishEvent) {
			if(newMethods.Contains(disabledMethod)){
				return true;
			}
		}
		return false;
	}

	void interrumptEventsByNewEvent(GameObject newEvent, string eventObjectKey){
		ArrayList methodsToCall = newEvent.GetComponent<Event> ().methodsToCall;
		for (int i = eventList.Count -1; i>=0; i--) {
			GameObject item = eventList.ElementAt(i).Value as GameObject;
			bool canInterrupt = item.GetComponent<Event>().canInterrupt(methodsToCall);
			//item.GetComponent<Event>().hasInterruptedBy = eventObjectKey;

			if (canInterrupt) {
				item.GetComponent<Event>().replaceMethodsToMethodsAfterInterrupt();
				newEvent.GetComponent<Event>().isWaiting = false;
				newEvent.GetComponent<Event>().hasInterruptedTo = eventObjectKey;
			}
		}
	}

	void eventSystem() {
		if (eventList.Count > 0) {

			for(int i = eventList.Count -1; i>=0; i--)
			{
				GameObject item = eventList.ElementAt(i).Value as GameObject;
				bool isWaiting = item.GetComponent<Event>().isWaiting;
				bool isFinished = item.GetComponent<Event>().isFinished;
				bool isRunning = item.GetComponent<Event>().isRunning;


				if (isWaiting && !isRunning) {
					item.GetComponent<Event>().executeScript ();
				} else if(isFinished) {
					eventList.Remove(item.GetComponent<Event>().hash);
					Destroy(item);
				}
			}


		}
	}

	public void addOrRemoveMethodsDisabled(ArrayList methodsDisabled, bool add = true){
		foreach (string methodDisabled in methodsDisabled) {
			if (!methodsDisabledUntilFinishEvent.Contains (methodDisabled) && add) {
				methodsDisabledUntilFinishEvent.Add(methodDisabled);
			}else if (methodsDisabledUntilFinishEvent.Contains (methodDisabled) && !add){
				methodsDisabledUntilFinishEvent.Remove(methodDisabled);
			}
		}
	}
}
