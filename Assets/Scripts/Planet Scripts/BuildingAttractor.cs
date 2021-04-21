using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAttractor : MonoBehaviour {

    public GameObject buildingAttractor;
    public float gravity = -9.8f;

    public void Attract(GameObject body)
    {
        /*
         * This is the function the game object body is passed to. 
         * Any object passed to this function, as long as it has a rigidbody, will be attracted with the force of -9.8 towards the center
        */
        Vector3 targetDirection = (body.transform.position - buildingAttractor.transform.position).normalized;                    // sets the direction the body object will be pulled
        Vector3 bodyUp = body.transform.up;                                                                     // defines the up direction of the body object

        body.transform.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.transform.rotation;// rotates the body object so that it's up direction points away from the center
        body.GetComponent<Rigidbody>().AddForce(targetDirection * gravity);                                     // adds the gravity force to the body object
    }
}
