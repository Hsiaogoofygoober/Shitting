using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Pistol gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, fpsCam;
    public Item gunItem;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;
    bool IsPickUP;
    bool IsDropDown;
    public InputActionReference PickUpRef;
    public InputActionReference DropDownRef;

    [SerializeField] Item[] items;
    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        coll = GetComponentInChildren<BoxCollider>();
        IsPickUP = PickUpRef.action.ReadValue<bool>();
        IsDropDown = DropDownRef.action.ReadValue<bool>();
        items = GetComponentInParent<Item[]>();
        
        //Setup
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }

    private void Update()
    {
        //Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && IsPickUP && !slotFull) PickUp();

        //Drop if equipped and "Q" is pressed
        if (equipped && IsDropDown) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        //Make weapon a child of the camera and move it to default position
        //transform.SetParent(gunContainer);
        items[0] = gunItem;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        //Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;

        //Enable script
        gunScript.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        //Set parent to null
        //transform.SetParent(null);

        //Make Rigidbody not kinematic and BoxCollider normal
        items[0] = null;
        rb.isKinematic = false;
        coll.isTrigger = false;

        //Gun carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        //AddForce
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        //Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        //Disable script
        gunScript.enabled = false;
    }
}
