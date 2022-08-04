using UnityEngine;
using Cinemachine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;

#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviourPunCallbacks,IDamageable
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		public float Sensitivity = 1f;
		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		public string killer = "";

		PhotonView PV;

		PlayerManagers playerManagers;

		// cinemachine
		private float _cinemachineTargetPitch;
		[SerializeField] private GameObject gun;
		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;
		const float maxHealth = 100f;
		float currentHealth = maxHealth;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private Animator _animator;
		private const float _threshold = 0.01f;
		[SerializeField] private CinemachineVirtualCamera playerFollowCamera;

		// aimming
		[SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
		[SerializeField] private float normalSensitivity;
		[SerializeField] private float aimSensitivity;

		// health
		[SerializeField] Image healthbarImage;
		[SerializeField] GameObject ui;

		// item
		[SerializeField] Item[] items;
		int itemIndex;
		int previousItemIndex = -1;

		// switch weapon rigging
		TwoBoneIKConstraint constraint;
		public RigBuilder rigBuilder;

		// switch gun
		public InputActionReference action_view;
		private float scrolling_value;
		

		// pick up and drop down
		public Rigidbody gunRb;
		public BoxCollider gunColl;
		public Transform player, fpsCam, itemHolder;
		public Item gunItem;
		public Camera playerCam;
		private GameObject weapon;
		private bool isHoldingWeapon;
		public float pickUpRange;
		public float dropForwardForce, dropUpwardForce;
		private int equipIndex = 0;
		public bool equipped;
		public static bool slotFull;
		
		public InputActionReference PickUpRef;
		public InputActionReference DropDownRef;

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			PV = GetComponent<PhotonView>();
			//Debug.Log(GameObject.FindWithTag("MainCamera").transform);
			playerManagers = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManagers>();

			action_view.action.performed += _x => scrolling_value = _x.action.ReadValue<float>();
			isHoldingWeapon = false;
		}

		private void Start()
		{
			constraint = GetComponentInChildren<TwoBoneIKConstraint>();
			if (PV.IsMine) 
			{
                //EquiptItem(0);
                if (isHoldingWeapon)
                {
					constraint.data.target = GameObject.FindWithTag("weaponHold").transform;
				}
				
				rigBuilder.Build();
				itemHolder = GameObject.FindWithTag("ItemHolder").transform;
			}
            else 
			{
				Destroy(GetComponentInChildren<Camera>().gameObject);
				Destroy(playerFollowCamera);
				Destroy(aimVirtualCamera);
				Destroy(ui);
			}

			
			
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_animator = GetComponentInChildren<Animator>();
			
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			if (!PV.IsMine) {
				return;
			}
			if (_input.move != Vector2.zero && !_input.sprint)
			{
				_animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
			}
			else if (_input.sprint && _input.move != Vector2.zero)
			{

				_animator.SetFloat("Speed", 1.0f, 0.1f, Time.deltaTime);
			}
			else
			{
				_animator.SetFloat("Speed", 0);
			}

            if (scrolling_value < 0 && items[0] != null && items[1] != null)
            {
				if (itemIndex >= items.Length - 1) 
				{
					EquiptItem(0);
				}
                else 
				{
					EquiptItem(itemIndex + 1);
				}
                if (isHoldingWeapon)
                {
					constraint.data.target = GameObject.FindWithTag("weaponHold").transform;
					rigBuilder.Build();
					Debug.Log(constraint.data.target);
				}
				
				
			}
			ControllPickAndDrop();
			ControllShoot();
			JumpAndGravity();
			GroundedCheck();
			Move();
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1) 
			{
				playerManagers.Win();
			}
		}

		private void LateUpdate()
		{
			if (!PV.IsMine)
			{
				return;
			}
			CameraRotation();
		}
     
		private void ControllShoot() 
		{

            if (items[itemIndex] != null)
            {
				items[itemIndex].Use();
			}
			
			
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * Time.deltaTime * Sensitivity;
				_rotationVelocity = _input.look.x * RotationSpeed * Time.deltaTime * Sensitivity;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
				
				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}
	
			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				_animator.SetBool("IsLoopingDown", true);
				_animator.SetBool("IsJumping", false);
				_animator.SetBool("IsLoopingUp", false);
				_animator.SetBool("IsMoving", true);
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					_animator.SetBool("IsJumping", true);
					
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				_animator.SetBool("IsLoopingUp", true);
				_animator.SetBool("IsLoopingDown", false);
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
		public void SetSensitivity(float newSensitivity) 
		{
			Sensitivity = newSensitivity;
		
		}


		private void EquiptItem(int _index) 
		{
			
			itemIndex = _index;
			items[itemIndex].itemGameObject.SetActive(true);

			if (previousItemIndex != -1 && items[0] != null && items[1] != null) 
			{
				items[previousItemIndex].itemGameObject.SetActive(false);			
			}

			previousItemIndex = itemIndex;

            if (PV.IsMine) 
			{
				Hashtable hash = new Hashtable();
				hash.Add("itemIndex", itemIndex);
				PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
			}
			
		}

		private void ControllPickAndDrop()
		{
			Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
			RaycastHit hit;

			
			if (_input.pick)
			{
				Debug.Log("hit E");
				if (Physics.Raycast(ray, out hit))
                {
					Debug.Log(hit.collider.gameObject);
					
                    if (hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("weapon"))
                    {
						Debug.Log("is weapon");
						weapon = hit.rigidbody.gameObject;
						PickUp(weapon);
					}
					
				}
				
			}
			else if (_input.drop) 
			{
				Drop(weapon);
			}

			/*if (PV.IsMine)
			{
				Hashtable hash = new Hashtable();
				hash.Add("itemIndex", itemIndex);
				PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
				Debug.Log(hash);
			}*/

		} 

		private void PickUp(GameObject weapon)
		{
			equipped = true;
			slotFull = true;
			isHoldingWeapon = true;
            //Make weapon a child of the camera and move it to default position
            if (PV.IsMine) 
			{
				itemHolder = GameObject.Find("ItemHolder").transform;
			}
			//Make Rigidbody kinematic and BoxCollider a trigger

			
            if (equipIndex < 2)
            {
				weapon.GetComponent<Rigidbody>().isKinematic = true;
				weapon.GetComponentInChildren<BoxCollider>().isTrigger = true;
				weapon.transform.SetParent(itemHolder);
				weapon.transform.localPosition = Vector3.zero;
				weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
				weapon.transform.localScale = Vector3.one;
				items[equipIndex++] = weapon.GetComponent<Item>();
				//EquiptItem(equipIndex);
			}
            
			
			
			constraint.data.target = GameObject.FindWithTag("weaponHold").transform;
			rigBuilder.Build();
			Debug.Log(constraint.data.target);
			

			//Enable script
			//gunScript.enabled = true;
		}

		private void Drop(GameObject weapon)
		{
			equipped = false;
			slotFull = false;
			isHoldingWeapon = false;
            //Set parent to null

            if (equipIndex > 0)
            {
				weapon.transform.SetParent(null);

				//Make Rigidbody not kinematic and BoxCollider normal
				items[--equipIndex] = null;
				weapon.GetComponent<Rigidbody>().isKinematic = false;
				weapon.GetComponentInChildren<BoxCollider>().isTrigger = false;

				//Gun carries momentum of player
				weapon.GetComponent<Rigidbody>().velocity = player.GetComponent<CharacterController>().velocity;

				//AddForce
				weapon.GetComponent<Rigidbody>().AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
				weapon.GetComponent<Rigidbody>().AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

				float random = Random.Range(-1f, 1f);
				weapon.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 10);
				//EquiptItem(equipIndex);
			}
			
			

			constraint.data.target = null;
			rigBuilder.Build();
			Debug.Log(constraint.data.target);
			
		}

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
			if (!PV.IsMine && targetPlayer == PV.Owner)
			{
				EquiptItem((int)changedProps["itemIndex"]);
			}
        }
        public void TakeDamage(float damage) 
		{
			PV.RPC("RPC_TakeDameage", RpcTarget.All, damage);
		}

		
		
		[PunRPC]
		void RPC_TakeDameage(float damage) 
		{
			if (!PV.IsMine)
				return;
			
			
			currentHealth -= damage;
			healthbarImage.fillAmount = currentHealth / maxHealth;

			if (currentHealth <= 0) 
			{
				Debug.Log(killer + "殺了你!!!");
				PlayerPrefs.SetString("killer", killer);
				Die();
			}
			
		}

		void Die()
        {
            playerManagers.Die(); 
		}
	}
}