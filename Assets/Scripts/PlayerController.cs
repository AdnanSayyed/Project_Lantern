using UnityEngine;
using UnityEngine.PostProcessing;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerLook))]
[RequireComponent(typeof(PlayerEquipment))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float mouseSensitivity = 10f;

    [SerializeField]
    private float wristSensitivity = 5f;

    [SerializeField]
    private float camLookAngle = 85f;

    private PlayerMotor motor;
    private PlayerLook camMotor;
    private PlayerEquipment equipmentManager;

    [SerializeField]
    private Camera envCam;

    private float horizontalMove;
    private float verticalMove;
    private float mouseX;
    private float mouseY;

    [SerializeField]
    private float wristMaxRotX = 6f;

    [SerializeField]
    private float wristMaxRotZ = -150f;

    [SerializeField]
    private float wristMinRotZ = -75f;

    [SerializeField]
    private Transform mirrorAnchor;

    [SerializeField]
    private GameObject mirrorPrefab;

    [SerializeField]
    private Transform inspectHolder;

    [SerializeField]
    private float inspectDist = 5f; 

    private bool usingMirror = false;
    private bool isInHallway = false;
    private bool usingNotepad = false;
    private bool isInspecting = false;
    private GameObject inspectedObj;
    private GameObject inspectedCloneObj;

    [SerializeField]
    private Light inspectLight;

    private string inspectLayer = "Inspectable";

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        camMotor = GetComponent<PlayerLook>();
        equipmentManager = GetComponent<PlayerEquipment>();
        motor.SetSensitivity(mouseSensitivity);
        motor.SetWristSensitivity(wristSensitivity);
        camMotor.SetSensitivity(mouseSensitivity);
        camMotor.SetLookAngle(camLookAngle);
        motor.SetWristRot(wristMaxRotX, wristMinRotZ, wristMaxRotZ);
    }

    private void Update()
    {
        ReadInput();
    }

    public void SetUsingMirror(bool isUsing)
    {
        usingMirror = isUsing;
    }

    public bool GetUsingMirror() {
        return usingMirror;
    }

    public bool GetUsingNotepad()
    {
        return usingNotepad;
    }

    void ReadInput()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        //Exit moonlight
        if (usingMirror && Input.GetKeyDown(KeyCode.Escape) && !equipmentManager.GetPlayingAnim()) {
            equipmentManager.GetEquippedItem().GetComponent<Mirror>().TurnOff();
            usingMirror = false;
            equipmentManager.Unequip();
            motor.ResetWrist();
        }

        //Open notepad inventory
        if (Input.GetButtonDown("Inventory") && !usingMirror && !isInspecting) {
            usingNotepad = equipmentManager.ToggleInventoryUI();
        }

        //Exit Inspection
        if (isInspecting)
        {
            if(Input.GetKeyDown(KeyCode.Escape)){
                Destroy(inspectedCloneObj);
                inspectedObj.GetComponent<MeshRenderer>().enabled = true;
                envCam.GetComponent<PostProcessingBehaviour>().enabled = false;
                inspectLight.enabled = false;
                isInspecting = false;
            }
        }

        //Inspect object
        if (Input.GetButtonDown("Inspect"))
        {
            Ray ray = envCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            int layerNo = LayerMask.GetMask(inspectLayer); 

            if (Physics.Raycast(ray,out hit, inspectDist))
            {
                if (hit.transform.gameObject.layer == 10 && !isInspecting && !usingMirror && !usingNotepad)
                {
                    inspectLight.enabled = true;
                    Debug.Log("Inspecting " + hit.transform.name);
                    isInspecting = true;
                    inspectedObj = hit.transform.gameObject;
                    inspectedCloneObj = Instantiate(inspectedObj, inspectHolder.position, Quaternion.identity) as GameObject;
                    inspectedObj.GetComponent<MeshRenderer>().enabled = false;
                    inspectedCloneObj.AddComponent<InspectObject>();
                    inspectedCloneObj.layer = 9;
                    envCam.GetComponent<PostProcessingBehaviour>().enabled = true;
                    inspectedCloneObj.GetComponent<Collider>().isTrigger = true;
                    
                }
            }

        }

    }

    public void SetInHallway(bool _state)
    {
        isInHallway = _state;
    }

    public void SetUsingNotepad(bool _state) {
        usingNotepad = _state;
    }

    private void FreezePlayer() {
        motor.SetMovement(0f, 0f);
        motor.SetRotation(0f);
        camMotor.SetRotation(0f);
    }

    public bool GetInHallway()
    {
        return isInHallway;
    }

    private void LateUpdate()
    {
        if (!usingMirror && !usingNotepad && !isInspecting)
        {
            if (!isInHallway)
            {
                motor.SetMovement(horizontalMove, verticalMove);
                motor.SetRotation(mouseX);
                camMotor.SetRotation(mouseY);
                camMotor.PerformHandRotation(0f, 0f);
            }
            else
            {
                motor.SetHallwayMovement(verticalMove);
                motor.SetRotation(0f);
                camMotor.SetRotation(0f);
                camMotor.PerformHandRotation(mouseX,mouseY);
            }
            
        }
        else if (usingMirror)
        {
            motor.FocusMirror();
            camMotor.ResetCamera();
            motor.SetWristMovement(mouseX, mouseY);
        }else if (isInspecting) {
            motor.SetRotation(0f);
            camMotor.SetRotation(0f);
            motor.SetMovement(0f, 0f);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else{
            FreezePlayer();
        }
    }

}