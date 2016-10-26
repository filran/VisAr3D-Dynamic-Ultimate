using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {

    public float i { set; get; }

	// Use this for initialization
	void Start () {
        this.gameObject.AddComponent<Rigidbody>();
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        this.transform.position = new Vector3(this.transform.position.x + (this.transform.position.x/4), this.transform.position.y, this.transform.position.z);
    }
}
