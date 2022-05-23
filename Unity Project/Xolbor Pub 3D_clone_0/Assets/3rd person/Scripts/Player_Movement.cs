using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class Player_Movement : NetworkBehaviour
{

	CharacterController controller;
	Animator animator;
	Rigidbody rigidbody;

	public GameObject thirdPersonCameraPrefab;
	public GameObject playerFPSCameraPrefab;
	GameObject playerFPSCameraObject;
	public Transform playerFPSCameraSpawnpoint;

	public float rotationSpeed, movementSpeed, gravity = 20;
	Vector3 movementVector = Vector3.zero;
	private float desiredRotationAngle = 0;

	public bool canControlCharacterMovement = true;
	bool isCameraActive = false;

	private void Start()
	{
		if (IsClient && IsOwner)
        {
			Invoke("SetFirstPersonCamera", 3f);
			Invoke("SetThirdPersonCamera", 3f);
			Invoke("SetupComponent", 3f);
		}

	}

	private void SetThirdPersonCamera()
	{
		if (IsClient && IsOwner)
		{
			GameObject camera = Instantiate(thirdPersonCameraPrefab);
			camera.GetComponent<CinemachineFreeLook>().LookAt = transform;
			camera.GetComponent<CinemachineFreeLook>().Follow = transform;
			camera.GetComponent<CinemachineFreeLook>().GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = 1.3f;
		}
	}
	private void SetFirstPersonCamera()
    {
		if (IsClient && IsOwner)
		{
			playerFPSCameraObject = Instantiate(playerFPSCameraPrefab, playerFPSCameraSpawnpoint.position, playerFPSCameraSpawnpoint.rotation);
			playerFPSCameraObject.transform.parent = playerFPSCameraSpawnpoint;
			playerFPSCameraObject.SetActive(false);
		}
	}
	private void SetupComponent()
	{
		if (IsClient && IsOwner)
		{
			controller = GetComponent<CharacterController>();
			animator = GetComponent<Animator>();
			rigidbody = GetComponent<Rigidbody>();
		}
	}

	public void HandleMovement(Vector2 input)
	{
		if (controller.isGrounded)
		{
			if (input.y > 0)
			{
				movementVector = transform.forward * movementSpeed;
			}
			else
			{
				movementVector = Vector3.zero;
				animator.SetFloat("move", 0);
			}
		}
	}
	void Awake()
	{
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 80;
	}
	public void HandleMovementDirection(Vector3 direction)
	{
		desiredRotationAngle = Vector3.Angle(transform.forward, direction);
		var crossProduct = Vector3.Cross(transform.forward, direction).y;
		if (crossProduct < 0)
		{
			desiredRotationAngle *= -1;
		}
	}

	private void RotateAgent()
	{
		if (desiredRotationAngle > 10|| desiredRotationAngle < -10)
		{
			transform.Rotate(Vector3.up * desiredRotationAngle * rotationSpeed * Time.deltaTime);
		}
	}

	private float SetCorrectAnimation()
	{
		float currentAnimationSpeed = animator.GetFloat("move");

		if (desiredRotationAngle > 10 || desiredRotationAngle < -10)
		{
			if (currentAnimationSpeed < 0.2f)
			{
				currentAnimationSpeed += Time.deltaTime * 2;
				currentAnimationSpeed = Mathf.Clamp(currentAnimationSpeed, 0, 0.2f);
			}
			animator.SetFloat("move", currentAnimationSpeed);
		}
		else
		{
			if (currentAnimationSpeed < 1)
			{
				currentAnimationSpeed += Time.deltaTime * 2;
			}
			else
			{
				currentAnimationSpeed = 1;
			}
			animator.SetFloat("move", currentAnimationSpeed);
		}
		return currentAnimationSpeed;
	}

	private void Update()
	{
		if (IsClient && IsOwner)
		{
			if (canControlCharacterMovement == false) { return; }
			PlayerMovement();
			//testMovement();
			RotateCharacter();
			AnimationWheel();
			ResetAnimation();
			PlayerFirstPersonView();
		}
	}
	private void PlayerMovement()
    {
		if (controller.isGrounded)
		{
			if (movementVector.magnitude > 0)
			{
				var animationSpeedMultiplier = SetCorrectAnimation();
				RotateAgent();
				movementVector *= animationSpeedMultiplier;
			}
		}
		movementVector.y -= gravity;
		controller.Move(movementVector * Time.deltaTime);
    }
	private void testMovement()
    {
		float translation = Input.GetAxis("Vertical") * 150f;
		translation *= Time.deltaTime;
		rigidbody.MovePosition(rigidbody.position + this.transform.forward * translation);

		float rotation = Input.GetAxis("Horizontal");
		if (rotation != 0)
		{
			rotation *= rotationSpeed;
			Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
			rigidbody.MoveRotation(rigidbody.rotation * turn);
		}
		else
		{
			rigidbody.angularVelocity = Vector3.zero;
		}
	}
	private void AnimationWheel()
    {
		if (Input.GetKeyDown(KeyCode.C) )
        {
			animator.SetBool("Emote_1", true);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			animator.SetBool("Emote_2", true);
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			animator.SetBool("Emote_3", true);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			animator.SetBool("Drink", true);
		}
		if (Input.GetKeyDown(KeyCode.B) && Vector3.Distance(controller.transform.position, this.transform.position)<0.6)
		{
			controller.transform.rotation = this.transform.rotation;
			animator.SetBool("Sit", true);
		}
    }
    private void ResetAnimation()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
			animator.SetBool("Emote_1", false);
			animator.SetBool("Emote_2", false);
			animator.SetBool("Emote_3", false);
			animator.SetBool("Drink", false);
			animator.SetBool("Sit", false);
        }
	}
    private void RotateCharacter()
    {
		if (Input.GetKey(KeyCode.A))
        {
			transform.Rotate(Vector3.down * Time.deltaTime * 270f);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(Vector3.up * Time.deltaTime * 270f);
		}
	}
	private void PlayerFirstPersonView()
	{
		if (Input.GetKeyDown(KeyCode.Q) && isCameraActive == false)
		{
			playerFPSCameraObject.SetActive(true);
			isCameraActive = true;
			return;
		}
		if (Input.GetKeyDown(KeyCode.Q) && isCameraActive == true)
		{
			playerFPSCameraObject.SetActive(false);
			isCameraActive = false;
		}
	}
}
