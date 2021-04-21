using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour {
	public Transform camera;
	public float headBobFrequency = 1.5f;
	public float headBobSwayAngle = 0.5f;
	public float headBobHeight = 0.3f;
	public float headBobSideMovement = 0.05f;
	public float headBobSprintSideMovement = 0.08f;
	public float headBobSpeedMultiplier = 0.3f;
	public float bobStrideSpeedLengthen = 0.3f;
	public float jumpLandMove = 3f;
	public float jumpLandTilt = 60f;

	//Add in sounds later
	public AudioClip[] footstepSounds;
	public AudioClip jumpSound;
	public AudioClip landSound;
    private AudioSource footstepAudio;

	SphericalFPSController player;
	Vector3 originalLocalPosition;
	float nextStepTime = 0.5f;
	float headBobCycle = 0.0f;
	float headBobFade = 0.0f;

	float springPosition = 0.0f;
	float springVelocity = 0.0f;
	float springElastic = 1.1f;
	float springDampen = 0.8f;
	float springVelocityThreshold = 0.05f;
	float springPositionThreshold = 0.05f;
	float defaultHeadSideBob;

	Vector3 previousPosition;
	Vector3 previousVelocity = Vector3.zero;

	Rigidbody body;

	bool previousGrounded;

	void Start(){
        footstepAudio = GetComponent<AudioSource>();
		defaultHeadSideBob = headBobSideMovement;
		originalLocalPosition = camera.localPosition;
		player = GetComponent<SphericalFPSController> ();
		body = GetComponent<Rigidbody> ();
		previousPosition = body.position;
	}

	void FixedUpdate(){
		Vector3 velocity = (body.position - previousPosition) / Time.deltaTime;
		Vector3 velocityChange = velocity - previousVelocity;
		previousPosition = body.position;
		previousVelocity = velocity;

		springVelocity -= velocityChange.y;
		springVelocity -= springPosition * springElastic;
		springVelocity *= springDampen;

		springPosition += springVelocity * Time.deltaTime;
		springPosition = Mathf.Clamp (springPosition, -0.3f, 0.3f);

		if (Mathf.Abs (springVelocity) < springVelocityThreshold && Mathf.Abs (springPosition) < springPositionThreshold) {
			springVelocity = 0;
			springPosition = 0;
		}
	

		float flatVelocity = new Vector3 (velocity.x, 0.0f, velocity.z).magnitude;

		float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);

		headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);

		float bobFactor = Mathf.Sin (headBobCycle * Mathf.PI * 2);
		float bobSwayFactor = Mathf.Sin (Mathf.PI * (2 * headBobCycle + 0.5f));

		bobFactor = 1 - (bobFactor * 0.5f + 1);
		bobFactor *= bobFactor;

		if (new Vector3 (velocity.x, 0.0f, velocity.z).magnitude < 0.1f) {
			headBobFade = Mathf.Lerp (headBobFade, 0.0f, Time.deltaTime);
		} else {
			headBobFade = Mathf.Lerp (headBobFade, 1.0f, Time.deltaTime);
		}

		float speedHeightFactor = 1 + (flatVelocity * headBobSpeedMultiplier);

		float xPos = -headBobSideMovement * bobSwayFactor;
		float yPos = springPosition * jumpLandMove + bobFactor * headBobHeight * headBobFade * speedHeightFactor;

		float xTilt = -springPosition * jumpLandTilt;
		float zTilt = bobSwayFactor * headBobSwayAngle * headBobFade;



		if (player.grounded) {
			camera.localPosition = Vector3.Slerp (originalLocalPosition, originalLocalPosition + new Vector3 (xPos, yPos, 0), Time.deltaTime * 5);
			camera.localRotation = Quaternion.Euler (xTilt, 0.0f, zTilt);
			if (Input.GetKey (KeyCode.LeftShift)) {
				headBobSideMovement = Mathf.Lerp(headBobSideMovement, headBobSprintSideMovement, Time.deltaTime * 5f);
			} else {
				headBobSideMovement = Mathf.Lerp (headBobSideMovement, defaultHeadSideBob, Time.deltaTime * 5f);
			}
            if (!previousGrounded)
            {
                footstepAudio.clip = landSound;
                footstepAudio.Play();
                nextStepTime = headBobCycle + 0.5f;
            }
            else {
                if (headBobCycle > nextStepTime) {
                    nextStepTime = headBobCycle + 0.5f;
                    int n = Random.Range(1, footstepSounds.Length);
                    footstepAudio.clip = footstepSounds[n];
                    footstepAudio.Play();
                }
            }
			previousGrounded = true;
		} else {
			headBobSideMovement = 0f;
			previousGrounded = false;
		}
	}
}
