using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	/*
	 * 
	 *         This component is only enabled for "my player" (the character belonging to the local client machine)
	 * 
	 * 
	 */

	public float speed = 10f;
	public float jumpSpeed = 6f;
	Vector3 direction = Vector3.zero; // forward/back & left/right
	float verticalVelocity = 0;

	CharacterController cc;
	Animator anim;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		//movement is stored in "direction"
		direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if(direction.magnitude > 1f){
			direction = direction.normalized;
		}

		anim.SetFloat("Speed", direction.magnitude);

		// Handel Jumpng
		if(cc.isGrounded && Input.GetButton("Jump")){
			verticalVelocity = jumpSpeed;
		}

		AdjustAimAngle();

	}

	void AdjustAimAngle(){
		Camera myCamera = this.GetComponentInChildren<Camera>();

		if(myCamera == null){
			Debug.LogError("no camera on character");
			return;
		}

		float AimAngle = 0;

		if(myCamera.transform.rotation.eulerAngles.x <=90){
			//we are looking DOWN
			AimAngle = -myCamera.transform.rotation.eulerAngles.x;
		}
		else {
			AimAngle = 360 - myCamera.transform.rotation.eulerAngles.x;
		}

		anim.SetFloat("AimAngle", AimAngle);
	}

	//Called once per physics loop
	//Do all movement and other physics stuff here
	void FixedUpdate(){

		Vector3 dist = direction * speed * Time.deltaTime;
		if(cc.isGrounded && verticalVelocity < 0){
			anim.SetBool("Jumping", false);
			verticalVelocity = Physics.gravity.y * Time.deltaTime;
		}
		else {
			if(Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f){
				anim.SetBool("Jumping", true);
			}

			verticalVelocity += Physics.gravity.y * Time.deltaTime;

		}

		dist.y = verticalVelocity * Time.deltaTime;

		cc.Move(dist);
	}
}
