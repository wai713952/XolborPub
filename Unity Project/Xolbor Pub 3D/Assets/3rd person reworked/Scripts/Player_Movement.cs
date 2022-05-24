using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class Player_Movement_New : NetworkBehaviour
{

	Animator animator;
	Rigidbody rigidbody;
	PlayerControllerInteraction interaction;

	public GameObject thirdPersonCameraPrefab;
	public GameObject playerFPSCameraPrefab;
	GameObject playerFPSCameraObject;
	public Transform playerFPSCameraSpawnpoint;

	public float rotationSpeed;
	public float movementSpeedCurrent;
	public float movementSpeedMax;
	public float movementSpeedMin;
	public float animationFloat;

	public bool canControlCharacterMovement = true;
	public bool isCameraActive = false;

	public bool isStoppedFromEmote = false;
	public bool isEmotePlaying;
	
	public string animatiomName;
	public NetworkVariable<NetworkString> animationNameNetwork = new NetworkVariable<NetworkString>();

	private void Start()
	{
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		interaction = GetComponent<PlayerControllerInteraction>();

		Cursor.lockState = CursorLockMode.Locked;

		if (IsClient && IsOwner)
        {
			Invoke("SetFirstPersonCamera", 0.3f);
			Invoke("SetThirdPersonCamera", 0.3f);
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


	void Awake()
	{
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 80;
	}

	private void Update()
	{
		if (IsClient && IsOwner)
		{
			if (canControlCharacterMovement == false) { return; }
			AnimationWheel();
			ResetAnimation();

            if (isStoppedFromEmote == false)
            {
				PlayerSpeed();
				PlayerMovement();
				RotateCharacter();
				PlayerFirstPersonView();
            }

			PlayerSetAnimationName();
		}

		PlayerAnimationInOtherClient();
		print("active in other");
	}
	private void PlayerMovement()
    {
		float translation = Input.GetAxis("Vertical") * movementSpeedCurrent;
		translation *= Time.deltaTime;
		rigidbody.MovePosition(rigidbody.position + this.transform.forward * translation);
		
	}
	private void PlayerSpeed()
    {
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))	//run
        {
			animatiomName = "run";
			animator.SetFloat("move", 1f);
			movementSpeedCurrent = movementSpeedMax;
			animator.speed = 1;
		}
		else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))	//walk
		{
			animatiomName = "walk";
			animator.SetFloat("move", 0.18f);
			movementSpeedCurrent = movementSpeedMin;
			animator.speed = 1;
		}
		else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))	//step back
		{
			animatiomName = "step back";
			animator.SetFloat("move", 0.145f);
			movementSpeedCurrent = movementSpeedMin / 1.5f;
			animator.speed = 1;
		}
		else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))	//idle
        {
			animatiomName = "idle";
			animator.SetFloat("move", 0f);
			movementSpeedCurrent = 0;
			animator.speed = 1;
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
		if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
        {
			animatiomName = "turning";
			animator.SetFloat("move", 0.12f);
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
	private void AnimationWheel()
    {
		if(isStoppedFromEmote == true) { return; }
		if (Input.GetKeyDown(KeyCode.C))
        {
			animatiomName = "emote 1";
			AnimationWheelPreventMovement();
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			animatiomName = "emote 2";
			AnimationWheelPreventMovement();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			animatiomName = "emote 3";
			AnimationWheelPreventMovement();
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			animatiomName = "drink";
			AnimationWheelPreventMovement();
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			animatiomName = "sit";
			AnimationWheelPreventMovement();
		}
    }
	private void AnimationWheelPreventMovement()
    {
		isStoppedFromEmote = true;
		rigidbody.isKinematic = true;
	}
	private void ResetEmoteAnimationInOtherClient()
    {
		animator.SetBool("Emote_1", false);
		animator.SetBool("Emote_2", false);
		animator.SetBool("Emote_3", false);
		animator.SetBool("Drink", false);
		animator.SetBool("Sit", false);
	}

    private void ResetAnimation()
    {
		if (Input.GetKeyDown(KeyCode.Space) && isStoppedFromEmote == true)
        {
			isStoppedFromEmote = false;
			rigidbody.isKinematic = false;
		}
	}
	public void PlayerPreventActionCall(float waitTime)
    {
		isStoppedFromEmote = true;
		canControlCharacterMovement = false;
		interaction.canControlCharacterRay = false;
		Invoke("PlayerPreventActionEnd", waitTime);
    }
	private void PlayerPreventActionEnd()
    {
		isStoppedFromEmote = false;
		canControlCharacterMovement = true;
		interaction.canControlCharacterRay = true;
    }

	[ServerRpc]
    private void SendAnimationNameToHostServerRpc(string animationName)
    {
		animationNameNetwork.Value = animationName;
		print(animationNameNetwork.Value);
	}

	private void PlayerSetAnimationName()
    {
		if (Input.anyKeyDown || !Input.anyKeyDown)
		{
			SendAnimationNameToHostServerRpc(animatiomName);
		}
	}

	private void PlayerAnimationInOtherClient()
    {
		print("running in other player");
		if (animationNameNetwork.Value == "run")
		{
			animator.SetFloat("move", 1f);
			ResetEmoteAnimationInOtherClient();
		}
		if (animationNameNetwork.Value == "walk")
		{
			animator.SetFloat("move", 0.18f);
			ResetEmoteAnimationInOtherClient();
		}
		if (animationNameNetwork.Value == "idle")
		{
			animator.SetFloat("move", 0f);
			ResetEmoteAnimationInOtherClient();
		}
		if (animationNameNetwork.Value == "step back")
        {
			animator.SetFloat("move", 0.145f);
			ResetEmoteAnimationInOtherClient();
		}
		if (animationNameNetwork.Value == "turning")
		{
			animator.SetFloat("move", 0.12f);
			ResetEmoteAnimationInOtherClient();
		}
		if (animationNameNetwork.Value == "emote 1")
        {
			animator.SetBool("Emote_1", true);
		}
		if (animationNameNetwork.Value == "emote 2")
		{
			animator.SetBool("Emote_2", true);
		}
		if (animationNameNetwork.Value == "emote 3")
		{
			animator.SetBool("Emote_3", true);
		}
		if (animationNameNetwork.Value == "drink")
		{
			animator.SetBool("Drink", true);
		}
		if (animationNameNetwork.Value == "sit")
		{
			animator.SetBool("Sit", true);
		}
	}
}
