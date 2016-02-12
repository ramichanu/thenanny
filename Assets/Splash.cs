using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnParticleCollision(ParticleSystem other) {
		/*Rigidbody body = other.GetComponent<Rigidbody>();
		if (body) {
			Vector3 direction = other.transform.position - transform.position;
			direction = direction.normalized;
			body.AddForce(direction * 5);
		}*/
		Debug.Log ("particle");
	}
}
