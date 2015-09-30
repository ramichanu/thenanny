using UnityEngine;
using System.Collections;

public class Event : MonoBehaviour {

	public string hash;
	public bool isFinished = false;
	public bool isWaiting = true;
	public string currentEventRunning;

	public string hasInterruptedBy;
	public ArrayList methodsToCall = new ArrayList();
	public ArrayList canInterruptBy = new ArrayList();
	public ArrayList methodsAfterInterrupt = new ArrayList();


	public void init(ArrayList methodsToCall, ArrayList canInterruptBy, ArrayList methodsAfterInterrupt ) {

		this.methodsToCall = methodsToCall;
		this.canInterruptBy = canInterruptBy;
		this.methodsAfterInterrupt = methodsAfterInterrupt;

		hash = getHash ();
	}

	string getHash(){
		string methodsToCallString = string.Join (",", (string[])methodsToCall.ToArray(typeof(string) ));
		string canInterruptByString = string.Join (",", (string[])canInterruptBy.ToArray(typeof(string) ));
		string hash = methodsToCallString + canInterruptByString;
		string hashstring = hash.GetHashCode ().ToString();

		return hashstring;
	}
	

	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "eventIsFinished");
		NotificationCenter.DefaultCenter.AddObserver(this, "enableEventToWaitExecution");
	}

	void Update () {
	
	}

	public void executeScript() {
		this.isWaiting = false;

		string currentMethodToCall = methodsToCall[0] as string;
		string[] currentMethodToCallArray = currentMethodToCall.Split('_');
		string objectName = currentMethodToCallArray[0];
		string scriptMethod = currentMethodToCallArray[1];
		currentEventRunning = currentMethodToCall;

		Hashtable options = new Hashtable ();
		options.Add ("objectName", objectName);
		options.Add ("scriptMethod", scriptMethod);
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

		methodsToCall.Remove(methodCalled);

		if (methodsToCall.Count>0){
			executeScript();
		} else{
			if(hasInterruptedBy != null) {
				Hashtable opt = new Hashtable ();
				opt.Add ("eventHash", hasInterruptedBy);
				NotificationCenter.DefaultCenter.PostNotification(this, "enableEventToWaitExecution", opt);
			}

			this.isFinished = true;
		}
	}

	public void replaceMethodsToMethodsAfterInterrupt() {
		Hashtable options = new Hashtable ();
		options.Add ("methodCalled", currentEventRunning);
		NotificationCenter.DefaultCenter.PostNotification(this, "eventIsFinished", options);

		methodsToCall = methodsAfterInterrupt;

	}

	public void enableEventToWaitExecution(Notification options){
		string eventHash = options.data ["eventHash"].ToString();
		if (eventHash == hash) {
			isWaiting = true;
			isFinished = false;
		}
	}

}
