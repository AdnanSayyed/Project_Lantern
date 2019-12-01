using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour {

    private float distanceToPlayer;

    private GameObject player;

    private PlayerInventory inventory;
    private PlayerMotor motor;
    private PlayerController controller;
    private PlayerEquipment equipmentManager;
    private NotepadManager notepadManager;

    [SerializeField]
    float triggerDistance = 0f;

    [SerializeField]
    private string objectName;

    [SerializeField]
    private bool dontRotate = false;

    [SerializeField]
    private string pickUpText;

    [SerializeField]
    private GameObject indicatorText;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private bool Look3D = false;

    [SerializeField]
    float moonlightAngle = 0f;

    private bool tilted = false;

    private string doorText;

    private bool tilting = false;

    Quaternion newRotation;

    Vector3 v;

    float angleToRotate;

    float tiltStartTime;

    float rotationMult;

    float rotationFraction;

    private void Start()
    {
        player = GameObject.Find("Player");

        inventory = player.GetComponent<PlayerInventory>();
        motor = player.GetComponent<PlayerMotor>();
        controller = player.GetComponent<PlayerController>();
        equipmentManager = player.GetComponent<PlayerEquipment>();
        notepadManager = GameObject.Find("Notepad").GetComponent<NotepadManager>();

        if (triggerDistance == 0f)
            triggerDistance = player.GetComponent<Player>().GetTriggerDistance();
            

        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        if (indicatorText != null)
            indicatorText.GetComponent<Text>().text = pickUpText;
    }

    private void Interact()
    {
        switch (objectName)
        {
            case "mirror":
                inventory.PickUp(objectName);
                Destroy(gameObject, 0.1f);
                notepadManager.EnableItem("mirror");
                break;

            case "lamp":
                inventory.PickUp(objectName);
                Destroy(gameObject, 0.1f);
                notepadManager.EnableItem("lamp");
                break;

            case "painting":
                if (!tilting)
                {
                    tilting = true;
                    tiltStartTime = Time.time;
                }
                break;

            case "key":
                inventory.PickUp(objectName);
                Destroy(gameObject, 0.1f);
                break;

            case "door":
                if (inventory.HasItemInInventory("key"))
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    pickUpText = "You need key to unlock!";
                    doorText = indicatorText.GetComponent<Text>().text;
                    indicatorText.GetComponent<Text>().text = pickUpText;
                    Invoke("ResetDoorText", 2f);
                }
                break;

            case "moonlight":

                //Check if player has mirror and is in moonlight
                if (inventory.HasItemInInventory("mirror") && !controller.GetUsingNotepad() && !equipmentManager.GetPlayingAnim())
                {
                    //Center Mirror Beam
                    motor.AimMirrorForward();

                    //Set Moonlight Angle
                    motor.SetMoonlightFocus(moonlightAngle);

                    //Enable Mirror Controls
                    controller.SetUsingMirror(true);

                    Debug.Log("Inventory has mirror");

                    if (equipmentManager.HasEquippedItem("mirror"))
                    {
                        equipmentManager.GetEquippedItem().GetComponent<Mirror>().TurnOn();
                    }
                    else
                    {
                        equipmentManager.Equip("mirror");
                        equipmentManager.Invoke("TurnOnMirror",1.1f);
                    }
                }
                break;

            default:
                return;
        }
    }

    void ResetDoorText()
    {
        indicatorText.GetComponent<Text>().text = doorText;
    }

    private void Update()
    {

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (tilting)
        {

            float currentDuration = Time.time - tiltStartTime;

            if (!tilted)
            {
                if (Math.Round(transform.localRotation.eulerAngles.z,2) != 90f)
                {
                    angleToRotate = 90f;
                    rotationMult = 20f;
                }else{
                    tilted = true;
                    tilting = false;
                }
            }else if (tilted){

                if(Math.Round(transform.localRotation.eulerAngles.z,2) != 0f)
                {
                    angleToRotate = 360f;
                    rotationMult = 60f;
                }
                else{
                    tilted = false;
                    tilting = false;
                }
            }

            newRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, angleToRotate);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, currentDuration * rotationMult / angleToRotate);
            
        }

        if (distanceToPlayer <= triggerDistance)
        {

            if (objectName == "moonlight")
            {
                if (inventory.HasItemInInventory("mirror") && !controller.GetUsingMirror())
                {
                    if (indicatorText != null)
                        indicatorText.GetComponent<Text>().enabled = true;
                }
                else {
                    if (indicatorText != null)
                        indicatorText.GetComponent<Text>().enabled = false;
                }
            }
            else {
                if (indicatorText != null)
                    indicatorText.GetComponent<Text>().enabled = true;
            }

           
            
            if (Input.GetButtonDown("Interact"))
            {
                Interact();
            }

        }
        else
        {
            if (indicatorText != null)
                indicatorText.GetComponent<Text>().enabled = false;
        }

        if (!dontRotate)
        {
            if (Look3D)
            {
                if (canvas != null)
                    canvas.transform.LookAt(player.transform.position, Vector3.up);
            }
            else
            {

                if (canvas != null)
                    v = player.transform.position - canvas.transform.position;

                v.x = v.z = 0;

                if (canvas != null)
                    canvas.transform.LookAt(player.transform.position - v);
            }

            if (canvas != null)
                canvas.transform.Rotate(0, 180f, 0);
        }

    }

}
