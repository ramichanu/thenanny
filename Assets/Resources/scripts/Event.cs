using UnityEngine;
using System.Collections;

public class Event : MonoBehaviour {

	EventDispatcher eventDisp;
	public string hash;
	public bool isFinished = false;
	public bool isWaiting = true;
	public bool isRunning = false;
	public string currentEventRunning;

	public string hasInterruptedBy;
	public string hasInterruptedTo;
	public ArrayList methodsToCall = new ArrayList();
	public ArrayList canInterruptBy = new ArrayList();
	public ArrayList methodsAfterInterrupt = new ArrayList();
	public ArrayList methodsDisabledUntilFinishEvent = new ArrayList ();


	public void init(ArrayList methodsToCall, ArrayList canInterruptBy, ArrayList methodsAfterInterrupt, ArrayList methodsDisabledUntilEventFinished ) {
		eventDisp = EventDispatcher.DefaultEventDispatcher;

		this.methodsToCall = methodsToCall;
		this.canInterruptBy = canInterruptBy;
		this.methodsAfterInterrupt = methodsAfterInterrupt;
		this.methodsDisabledUntilFinishEvent = methodsDisabledUntilEventFinished;

		hash = getHash ();
	}

	string getHash(){
		string methodsToCallString = string.Join (",", (string[])methodsToCall.ToArray(typeof(string) ));
		string canInterruptByString = string.Join (",", (string[])canInterruptBy.ToArray(typeof(string) ));
		string hash = methodsToCallString + canInterruptByString;
		//string hashstring = hash.GetHashCode ().ToString();
		string hashstring = hash;

		return hashstring;
	}
	

	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "eventIsFinished");
		NotificationCenter.DefaultCenter.AddObserver(this, "enableEventToWaitExecution");
	}

	void Update () {
	
	}

	public void executeScript() {
		eventDisp.addOrRemoveMethodsDisabled (methodsDisabledUntilFinishEvent, true);

		this.isWaiting = false;
		this.isRunning = true;

		string currentMethodToCall = methodsToCall[0] as string;
		string[] currentMethodToCallArray = currentMethodToCall.Split('_');
		string objectName = currentMethodToCallArray[0];
		string scriptMethod = currentMethodToCallArray[1];
		currentEventRunning = currentMethodToCall;

		Hashtable options = new Hashtable ();
		options.Add ("objectName", objectName);
		options.Add ("scriptMethod", scriptMethod);
		options.Add ("hash", this.hash);

		NotificationCenter.DefaultCenter.PostNotification(this, "executeScript", options);
	}

	public bool canInterrupt(ArrayList functionsName){
		foreach( string functionName in functionsName){
			string[] functionNameArray = functionName.Split('_');
			string methodName = functionNameArray[1];
			if(canInterruptBy.Contains(methodName)){
				hasInterruptedBy = functionName;
				return true;
			}
		}
		return false;
	}

	void eventIsFinished(Notification options){

		string methodCalled = options.data ["methodCalled"].ToString();
		string hashFromScript = options.data ["hash"].ToString();
		this.isRunning = false;
		methodsToCall.Remove(methodCalled);

		if (hashFromScript == this.hash || this.hasInterruptedTo != null) {
			if (methodsToCall.Count>0){
				this.isWaiting = true;
				executeScript();
			} else{
				
				this.isFinished = true;
				this.isWaiting = false;
				this.isRunning = false;
				eventDisp.addOrRemoveMethodsDisabled (methodsDisabledUntilFinishEvent, false);
				
				if(hasInterruptedBy != null) {
					Hashtable opt = new Hashtable ();
					opt.Add ("eventHash", hasInterruptedBy);
					NotificationCenter.DefaultCenter.PostNotification(this, "enableEventToWaitExecution", opt);
				}
				
			}
		}

	}

	public void replaceMethodsToMethodsAfterInterrupt() {
		Hashtable options = new Hashtable ();
		options.Add ("methodCalled", currentEventRunning);
		options.Add ("hash", this.hash);

		methodsToCall = methodsAfterInterrupt;

		NotificationCenter.DefaultCenter.PostNotification(this, "eventIsFinished", options);

	}

	public void enableEventToWaitExecution(Notification options){
		string eventHash = options.data ["eventHash"].ToString();
		if (eventHash == hash) {
			isWaiting = true;
			isFinished = false;
		}
	}

}
