/*
 * GravityBody.cs
 * 
 * Author: Bendrix Bailey
 * Project: Small Planet
 * 
 * This script should be attached to any object 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GravityBody : MonoBehaviour {

	GravityAttractor planet;    // this creates a variable to represent the planet attractor

	void Awake(){
        /*
         * This function is called whenever the scene becomes active. 
         * This finds the planet object.
         * It also disables this rigidbody's 2d gravity and freezes its rotation so the player doesnt fall forward
        */
		planet = GameObject.FindGameObjectWithTag ("Planet").GetComponent<GravityAttractor> (); // this finds the planet with the gravity attractor script (THIS WORKS WITH MULTIPLE PLANETS)

		GetComponent<Rigidbody> ().useGravity = false;                                          // tells this objects rigidbody to turn off 2d gravity
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;           // tells this objects rigidbody to stop the player from rolling over
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
	}

    void FixedUpdate(){
        // because this is a rigidbody, fixed update must be used.
        // this function passes this game object as a gravity body to the gravity attractor script so it can do its worst
        planet.Attract(gameObject);
        
	}
}
