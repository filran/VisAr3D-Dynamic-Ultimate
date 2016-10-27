using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.gameObject.AddComponent<Rigidbody>();
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        this.gameObject.AddComponent<BoxCollider>().size = new Vector3(8,6,0);
        this.gameObject.GetComponent<BoxCollider>().center = new Vector3(0,-.7f,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
