/*
 * SphericalFPSController.cs
 * 
 * Author: Bendrix Bailey
 * Project: Small Planet
 * 
 * This script handles the player movement and jumping of the character.
 * This is all original code because the pre-packaged fps controller doesnt work with spherical worlds
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalFPSController : MonoBehaviour {

	public float mouseSensitivityX = 250f;  // sets mouse sensitivity x
	public float mouseSensitivityY = 250f;  // sets mouse sensitivity y
    public float maxVelocityChange = 10.0f;
    public float walkSpeed = 5f;            // sets the walk speed of the player
	public float sprintSpeed = 8f;          // sets the sprint speed of the player
	public float jumpForce = 10f;          // sets the jump force of the player
	public LayerMask groundedMask;          // allows the developer to choose what counts as ground

	public GameObject camera;               // passes the camera for movement purposes
	public bool grounded;                   // variable for jumping control

	float verticalLookRot;                  // vertical camera rotation control
	Vector3 moveAmount;                     // private move variable
	Vector3 smoothMoveVelocity;             // private variable to smooth movement
	private float moveSpeed;                // speed to keep track of movement
    private float verticalAxis;
    private Rigidbody rigidbody;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.interpolation = RigidbodyInterpolation.None;
    }

    void Update () {
        /*
         * This function is called every time the game updates.
         * This handles all updates EXCEPT for rigidbody updates. Those are handled in the FixedUpdate function
         * This function takes the generic mousex and mousey inputs and horizontal and vertical and jump inputs and moves the camera or player accordingly
        */
        //== Camera Movement ====
        

		grounded = false;                                                       // the player is no longer grounded because of the above jump

		Ray ray = new Ray (transform.position, -transform.up);                  // creates a ray that shoots down towards the planet
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1 + 0.3f, groundedMask)) {           // if the raycaset of the certain length hits the ground
			grounded = true;                                                    // the player is grounded
		}

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);             // rotates the entire player the horizontal mouse input
        verticalLookRot += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;                           // sets the camera vert rot to the vertical mouse input
        verticalLookRot = Mathf.Clamp(verticalLookRot, -90, 90);                                                    // clamps the camera rotation to only vertical
        camera.transform.localEulerAngles = Vector3.left * verticalLookRot;                                         // applies the rotation and the clamp to the camera transform

    }

	void FixedUpdate(){
        /*
        * FixedUpdate handles the actual moving of the rigidbody.
        * Force can be added in regular update, but moving the actual position in the update funciton will cause jittering
        */
        if (grounded) {
            if (grounded)
            {

                if (Input.GetKey(KeyCode.LeftShift))
                {                                 // if the player has pressed the sprint key
                    moveSpeed = sprintSpeed;                                            // sets the move speed to the sprint speed
                }
                else
                {                                                                // if the key is up
                    moveSpeed = walkSpeed;                                              // the move speed is the regular walk speed
                }

                // Calculate how fast we should be moving
                Vector3 forward = Vector3.Cross(transform.up, -camera.transform.right).normalized;
                Vector3 right = Vector3.Cross(transform.up, camera.transform.forward).normalized;
                Vector3 targetVelocity = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")) * moveSpeed;

                Vector3 velocity = transform.InverseTransformDirection(rigidbody.velocity);
                velocity.y = 0;
                velocity = transform.TransformDirection(velocity);
                Vector3 velocityChange = transform.InverseTransformDirection(targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                velocityChange = transform.TransformDirection(velocityChange);

                rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                if (Input.GetButton("Jump"))
                {
                    rigidbody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
                }
            }
        }
        //transform.position = Vector3.Lerp(transform.position, GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount), Time.deltaTime * 5);
        //transform.position = transform.position + transform.TransformDirection(moveAmount);
	}
}
