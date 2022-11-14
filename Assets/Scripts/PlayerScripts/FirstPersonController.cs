using UnityEngine;
using Cinemachine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.IO;
using UnityEngine.InputSystem;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using TMPro;

#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviourPunCallbacks, IDamageable
    {
        //public static FirstPersonController instance;

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
        public float FallTimeout = 0.5f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = 0.5f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public GameObject AimCinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        public string killer = "";
        public string killerAccount = "";

        public PhotonView PV;

        PlayerManagers playerManagers;

        // cinemachine
        private float _cinemachineTargetPitch;
        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private bool _isGrounded = false;


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
        public CinemachineVirtualCamera aimVirtualCamera;
        public GameObject PistolInitPos;
        public GameObject PistolAimPos;
        public GameObject SniperAimPos;
        public GameObject Target;
        public GameObject SniperScope;
        public GameObject GunCrosshair;

        // health
        [SerializeField] Image healthbarImage;
        [SerializeField] GameObject ui;
        const float maxHealth = 100f;
        public float currentHealth = maxHealth;

        // item
        [SerializeField] Item[] items;
        int itemIndex;
        int previousItemIndex = -1;

        // switch weapon rigging
        public TwoBoneIKConstraint constraint;
        public RigBuilder rigBuilder;

        // switch gun
        public InputActionReference action_view;
        private float scrolling_value;


        // pick up and drop down
        public Transform player, fpsCam, itemHolder;
        public Item gunItem;
        public Camera playerCam;
        private GameObject weapon;
        public float pickUpRange;
        public float dropForwardForce, dropUpwardForce;
        public bool equipped;
        public static bool slotFull;
        public bool canPick = true;
        public bool canDrop = true;
        public InputActionReference PickUpRef;
        public InputActionReference DropDownRef;

        //damage text
        public GameObject damageText;


        //public List<Tool> toolList = new List<Tool>(18);

        [SerializeField]
        public GameObject Mybag;

        public Player[] other;

        //public Inventorys inventorys;

        public static int status;

        public InventoryManager2 InventoryManager2;
        public bool canUse = true;

        //current ammo
        public int pistolAmmo = 0;
        public int shotgunAmmo = 0;
        public int rifleAmmo = 0;
        public TextMeshProUGUI ammunitionDisplay;

        //audio
        [SerializeField] private AudioSource walkSoundEffect;
        [SerializeField] private AudioSource sprintSoundEffect;
        [SerializeField] private AudioSource groundSoundEffect;

        private void Awake()
        {
            //if (instance != null)
            //    Destroy(this);
            //instance = this;
           // Debug.Log("Master Client: " + PhotonNetwork.MasterClient.NickName);

            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            PV = GetComponent<PhotonView>();
            //Debug.Log(GameObject.FindWithTag("MainCamera").transform);
            playerManagers = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManagers>();
            fpsCam = _mainCamera.transform;
            Debug.Log("player owner : " + PV.Owner);
            Debug.Log("hello world " + playerManagers.photonView);
            action_view.action.performed += _x => scrolling_value = _x.action.ReadValue<float>();

            //toolList[17] = null;
        }

        private void Start()
        {
            constraint = GetComponentInChildren<TwoBoneIKConstraint>();
            if (PV.IsMine)
            {

            }
            else
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(playerFollowCamera);
                Destroy(aimVirtualCamera);
                Destroy(ui);
                //ui.SetActive(false);
            }



            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _animator = GetComponentInChildren<Animator>();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            //inventorys.ItemUsed += Inventorys_ItemUsed;
        }

        private void Inventorys_ItemUsed(object sender, InventoryEventArgs e)
        {
            IInventoryItem item = e.Item;
        }

        private void Update()
        {

            if (!PV.IsMine)
            {
                return;
            }
            else
            {
                if (_input.move != Vector2.zero && !_input.sprint && Grounded)
                {
                    _animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
                    walkSoundEffect.enabled = true;
                    sprintSoundEffect.enabled = false;
                }
                else if (_input.sprint && _input.move != Vector2.zero && Grounded)
                {
                    _animator.SetFloat("Speed", 1.0f, 0.1f, Time.deltaTime);
                    sprintSoundEffect.enabled = true;
                    walkSoundEffect.enabled = false;
                }
                else
                {
                    _animator.SetFloat("Speed", 0);
                    walkSoundEffect.enabled = false;
                    sprintSoundEffect.enabled = false;
                }

                if (scrolling_value < 0 && items[0] != null && items[1] != null)
                {
                    if (itemIndex >= items.Length - 1)
                    {
                        EquiptItem(0);
                        Debug.Log("equip 0");
                    }
                    else
                    {
                        EquiptItem(itemIndex + 1);
                        Debug.Log("equip 1");
                    }
                }
                Mybag.SetActive(_input.openbag);
                if (_input.openbag)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                ControllPickAndDrop();
                ControllShoot();
                JumpAndGravity();
                GroundedCheck();
                Move();
                healthbarImage.fillAmount = currentHealth / maxHealth;

                //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                //{
                //    PlayerPrefs.SetInt("Winner", 1);
                //    playerManagers.Win();
                //}
                //InventoryManager.RefreshTool();
                //Aimming();
            }
        }


        private void AddNewItem(Tool thisTool)
        {
            if (PV.IsMine)
            {
                for (int i = 0; i < PV.GetComponentInChildren<InventorySystem>().toolList.Length; i++)
                {
                    if (PV.GetComponentInChildren<InventorySystem>().toolList[i] == null)
                    {
                        
                        PV.GetComponentInChildren<InventorySystem>().toolList[i] = thisTool;
                        if(thisTool.toolName == "PistolAmmo" || thisTool.toolName == "RifleAmmo" || thisTool.toolName == "ShotgunAmmo")
                        {
                            PlayerPrefs.SetInt("SlotID", i);
                            Debug.Log("pick slotid :"+ PlayerPrefs.GetInt("SlotID"));
                        }
                        break;
                    }
                }
                InventoryManager2.toolList = PV.GetComponentInChildren<InventorySystem>().toolList;
                if (InventoryManager2.toolList[0] != null)
                    Debug.Log("洗洗囉");
                InventoryManager2.RefreshTool();

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

        public void CameraRotation()
        {
            Ray ray = _mainCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Target.transform.position = ray.GetPoint(50);
            
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * Time.deltaTime * Sensitivity;
                _rotationVelocity = _input.look.x * RotationSpeed * Time.deltaTime * Sensitivity;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                AimCinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
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

                
                if (_isGrounded && _fallTimeoutDelta <= 0.0f)
                {
                    groundSoundEffect.Play();
                    _isGrounded = false;
                }

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
                _isGrounded = true;
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


        private void EquiptItem(int _index, int weaponID)
        {


            GameObject weapon = PhotonView.Find(weaponID).gameObject;

            itemIndex = _index;
            constraint.data.target = PistolInitPos.transform;
            rigBuilder.Build();

            weapon.transform.SetParent(itemHolder);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            weapon.transform.localScale = Vector3.one;
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            weapon.GetComponentInChildren<BoxCollider>().enabled = false;

            items[itemIndex] = weapon.GetComponent<Item>();
            equipped = true;


            items[itemIndex].itemGameObject.SetActive(true);

            if (previousItemIndex != -1)
            {
                items[previousItemIndex].itemGameObject.SetActive(false);

            }
            
            previousItemIndex = itemIndex;



        }

        private void EquiptItem(int _index)
        {

            itemIndex = _index;

            items[itemIndex].itemGameObject.SetActive(true);

            if (previousItemIndex != -1 && items[previousItemIndex] != null)
            {

                items[previousItemIndex].itemGameObject.SetActive(false);
            }

            previousItemIndex = itemIndex;


            constraint.data.target = PistolInitPos.transform;
            rigBuilder.Build();

            if (PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("itemIndex", itemIndex);
                Debug.Log(hash);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            }

        }

        private void EquiptItemWhenDrop(int _index)
        {


            itemIndex = _index;

            items[itemIndex].itemGameObject.SetActive(true);

            if (previousItemIndex != -1 && items[previousItemIndex] != null)
            {

                items[previousItemIndex].itemGameObject.SetActive(false);
            }

            Debug.Log(items[itemIndex].name);
            Debug.Log(constraint.data.target);
            Debug.Log(items[itemIndex].GetComponentInChildren<BoxCollider>().transform);

            previousItemIndex = itemIndex;

            constraint.data.target = PistolInitPos.transform;
            rigBuilder.Build();



        }



        private void ControllPickAndDrop()
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
            RaycastHit hit;

            if (items[0] != null && items[1] != null)
            {
                slotFull = true;
            }
            else
            {
                slotFull = false;
            }

            if (_input.pick && canPick)
            {

                Debug.Log("hit E");
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.gameObject);

                    //pick gun
                    if (hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("weapon") && hit.distance <= pickUpRange && !slotFull)
                    {
                        canPick = false;
                        Debug.Log("is weapon");
                        weapon = hit.rigidbody.gameObject;
                        //TakeDamage(10);
                        PickWeapon(weapon.GetComponent<PhotonView>().ViewID);
                        //PickUp(weapon);
                        Invoke("readyToPick", 0.1f);

                        Debug.Log("pick");
                    }
                    //pick burger
                    else if(hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("burger") && hit.distance <= pickUpRange && !InventoryManager2.IsBagFull())
                    {
                        canPick = false;
                        Tool tool = hit.collider.GetComponent<Tool>();
                        AddNewItem(tool);
                        hit.collider.gameObject.SetActive(false);
                        //Destroy(hit.collider.gameObject); 
                        Invoke("readyToPick", 0.1f);
                    }
                    //pick pistol ammo
                    else if(hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("pistolAmmo") && hit.distance <= pickUpRange && !InventoryManager2.IsBagFull())
                    {
                        canPick = false;
                        Tool tool = hit.collider.GetComponent<Tool>();
                        AddNewItem(tool);
                        pistolAmmo += tool.toolValue;
                        InactiveKit(hit.collider.gameObject.GetComponent<PhotonView>().ViewID);
                        //hit.collider.gameObject.SetActive(false);
                        //Destroy(hit.collider.gameObject);
                        Invoke("readyToPick", 0.1f);
                    }
                    //pick rifle ammo
                    else if (hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("rifleAmmo") && hit.distance <= pickUpRange && !InventoryManager2.IsBagFull())
                    {
                        canPick = false;
                        Tool tool = hit.collider.GetComponent<Tool>();
                        AddNewItem(tool);
                        rifleAmmo += tool.toolValue;
                        InactiveKit(hit.collider.gameObject.GetComponent<PhotonView>().ViewID);
                        //hit.collider.gameObject.SetActive(false);
                        //Destroy(hit.collider.gameObject);
                        Invoke("readyToPick", 0.1f);
                    }
                    //pick shotgun ammo
                    else if (hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("shotgunAmmo") && hit.distance <= pickUpRange && !InventoryManager2.IsBagFull())
                    {
                        canPick = false;
                        Tool tool = hit.collider.GetComponent<Tool>();
                        AddNewItem(tool);
                        shotgunAmmo += tool.toolValue;
                        InactiveKit(hit.collider.gameObject.GetComponent<PhotonView>().ViewID);
                        //hit.collider.gameObject.SetActive(false);
                        //Destroy(hit.collider.gameObject);
                        Invoke("readyToPick", 0.1f);
                    }

                }

            }
            else if (_input.drop && canDrop && items[itemIndex] != null)
            {
                canDrop = false;
                Debug.Log("drop");
                DropWeapon(itemIndex);

                Invoke("readyToDrop", 0.5f);
            }

            /*if (PV.IsMine)
			{
				Hashtable hash = new Hashtable();
				hash.Add("itemIndex", itemIndex);
				PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
				Debug.Log(hash);
			}*/

        }

        void readyToPick()
        {
            canPick = true;
        }

        void readyToDrop()
        {
            canDrop = true;
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!PV.IsMine && targetPlayer == PV.Owner)
            {
                EquiptItem((int)changedProps["itemIndex"]);

            }
        }

        public void TakeDamage(int damage)
        {
            //only victom sends message to everyone
            
            if (PV.IsMine)
            {
                PV.RPC("RPC_TakeDameage", RpcTarget.All, damage);
                if (currentHealth <= 0)
                {
                    // PV.RPC("RPC_killerInfo", RpcTarget.Others, killerAccount);
                    PV.RPC("RPC_killerInfo", RpcTarget.Others, killer);
                    Debug.Log(killer + "殺了你!!!");
                    Debug.Log("目前Master Client為: " + PhotonNetwork.LocalPlayer.NickName);
                    //Kill.killAmount = 0;
                    DropKitWhenDie();
                    Die();
                }
            }
            else
            {
                DamageIndicator indicator = Instantiate(damageText, transform.position + Vector3.up * 2, Quaternion.identity).GetComponent<DamageIndicator>();
                indicator.SetDamageText(damage);
            }
        }

        public void PickWeapon(int weaponID)
        {
            PV.RPC("RPC_PickWeapon", RpcTarget.All, weaponID);
        }

        public void DropWeapon(int itemIndex)
        {
            PV.RPC("RPC_DropWeapon", RpcTarget.All, itemIndex);
        }

        public void InactiveKit(int ID)
        {
            PV.RPC("RPC_InactiveKit", RpcTarget.All, ID);
        }

        [PunRPC]
        void RPC_killerInfo(string str) 
        {
            //if (str == PlayerPrefs.GetString("Account"))
            //    Kill.killAmount++;

            if (str == killer) 
            {
                KillAmount.instance.amount++;
                Kill.killAmount++;
            }
                
        }


        [PunRPC]

        void RPC_DropWeapon(int itemIndex)
        {
            equipped = false;
            slotFull = false;
            //Set parent to null
            GameObject weapon = items[itemIndex].gameObject;

            items[itemIndex] = null;
            weapon.transform.SetParent(null);

            weapon.GetComponent<StateReset>().ResetState();
            //Make Rigidbody not kinematic and BoxCollider normal
            weapon.GetComponent<Rigidbody>().isKinematic = false;
            weapon.GetComponentInChildren<BoxCollider>().enabled = true;

            //Gun carries momentum of player
            weapon.GetComponent<Rigidbody>().velocity = player.GetComponent<CharacterController>().velocity;

            //AddForce
            weapon.GetComponent<Rigidbody>().AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
            weapon.GetComponent<Rigidbody>().AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            weapon.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 10);
            previousItemIndex = -1;
            Debug.Log("drop" + itemIndex);
            Debug.Log(weapon.name);


            if (itemIndex == 0 && items[1] != null)
            {
                Debug.Log(items[1].name);
                EquiptItemWhenDrop(1);
            }
            else if (itemIndex == 1 && items[0] != null)
            {
                Debug.Log(items[0].name);
                EquiptItemWhenDrop(0);
            }
            else if (items[0] == null && items[1] == null)
            {
                if (PV.IsMine)
                {
                    ammunitionDisplay.SetText("no weapon");
                    GunCrosshair.SetActive(false);
                }
                constraint.data.target = null;
                rigBuilder.Build();
                Debug.Log(constraint.data.target);
            }
        }

        [PunRPC]

        void RPC_PickWeapon(int weaponID)
        {
            if (PV.IsMine)
            {
                PhotonView.Find(weaponID).RequestOwnership();
                
                Debug.Log("change owership");
            }
            equipped = true;

            //itemHolder = GameObject.Find("ItemHolder").transform;
            if (items[0] == null)
            {

                EquiptItem(0, weaponID);


            }
            else if (items[1] == null)
            {

                EquiptItem(1, weaponID);

            }

            //weapon.GetPhotonView().RequestOwnership();

        }

        [PunRPC]
        void RPC_TakeDameage(int damage)
        {
            currentHealth -= damage;

            Debug.Log("TakeDamage");

        }

        [PunRPC]
        void RPC_InactiveKit(int ID)
        {
            PhotonView.Find(ID).gameObject.SetActive(false);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                StateController.status = 1;
                //PlayerPrefs.SetInt("Winner", 1);
                playerManagers.Win();
            }
        }

        void Die()
        {
            StateController.status = 0;
            Debug.Log("玩家目前人數 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("AutoCleanUP : " + PhotonNetwork.CurrentRoom.AutoCleanUp);

            playerManagers.Die();
        }
        void DropKitWhenDie()
        {
            //drop weapon to null
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    string weaponName = items[i].gameObject.GetComponent<Item>().itemInfo.itemName;
                    GameObject weapon = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Gun", weaponName),transform.position,Quaternion.identity);
                    PhotonNetwork.Destroy(items[i].GetComponent<PhotonView>());
                    items[i] = null;
               
                    //Gun carries momentum of player
                    weapon.GetComponent<Rigidbody>().velocity = player.GetComponent<CharacterController>().velocity;

                    //AddForce
                    weapon.GetComponent<Rigidbody>().AddForce(Vector3.up * dropUpwardForce, ForceMode.Impulse);

                    float random = Random.Range(-1f, 1f);
                    weapon.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 10);
                }
            }
            //drop kits in bag
            InventoryManager2.DropKitWhenDie(dropUpwardForce);
        }
    }
}