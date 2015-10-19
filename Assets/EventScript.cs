using UnityEngine;
using System.Collections;

public class EventScript : MonoBehaviour {

	protected string hashEvent;
	protected EventDispatcher eventDisp;

	// Use this for initialization
	void Awake () {
		eventDisp = EventDispatcher.DefaultEventDispatcher;
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void eventFinishedCallback(string methodExecuted){
		Hashtable options = new Hashtable ();
		string methodCalled = transform.name + "_" + methodExecuted;
		options.Add ("methodCalled", methodCalled);
		options.Add ("hash", this.hashEvent);
		NotificationCenter.DefaultCenter.PostNotification(this, "eventIsFinished", options);
	}
	
	protected void executeScript(Notification options){
		
		if(options.data["objectName"].ToString() == transform.name)
		{
			this.hashEvent = options.data["hash"].ToString();
			Invoke(options.data["scriptMethod"].ToString(), 0);
		}
	}
}
