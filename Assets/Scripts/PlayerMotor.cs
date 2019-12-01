using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {


    [SerializeField]
    float minMoveSpeed = 0.5f;

    [SerializeField]
    float maxMoveSpeed = 5f;

    float walkSpeed = 0f;

    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseSensitivity;
    private float wristSensitivity;
    private float wristX;
    private float wristY;

    private float wristMaxRotX;
    private float wristMinRotZ;
    private float wristMaxRotZ;
    private float wristRotX = 0f;
    private float wristRotZ = 0f;
    private float hallVelocity = 0f;
    private float wristAngleY = 0f;

    private float moonlightAngle;

    private Rigidbody rb;

    private Animation anim;

    private PlayerController controller;

    WaitForSeconds waitTime;

    private bool movingInHallway = false;

    [SerializeField]
    private Transform playerWrist;

    [SerializeField]
    private Transform moonlightAnchor;

    [SerializeField]
    private float mirrorTurnSpeed;

    private Quaternion defaultWristRot;

    private float timer = 0.0f;

    [SerializeField]
    private float maxBobSpeed = 0.2f;

    [SerializeField]
    private float minBobSpeed = 0.1f;

    [SerializeField]
    private float maxBobAmount = 0.2f;

    [SerializeField]
    private float minBobAmount = 0.1f;

    public float midpoint = 0.2f;

    float bobbingAmount = 0f;
    float bobbingSpeed = 0f;

    [SerializeField]
    Transform envCam;

    private bool isLimping = true;

    private float startTime;
    private float currentTime;

    [SerializeField]
    private int limpDuration;

    private void Start()
    {
        anim = GetComponent<Animation>();
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        defaultWristRot = playerWrist.localRotation;
        startTime = Time.time;
        bobbingAmount = maxBobAmount;
    }

    public void SetMovement(float _horizontal, float _vertical)
    {
        horizontal = _horizontal;
        vertical = _vertical;
    }

    public void SetWristMovement(float _horizontal, float _vertical)
    {
        wristX = _horizontal;
        wristY = _vertical;
    }

    public void SetWristRot(float _maxX, float _minZ, float _maxZ) {
        wristMaxRotX = _maxX;
        wristMinRotZ = _minZ;
        wristMaxRotZ = _maxZ;
    }

    public void SetSensitivity(float _sensitivity)
    {
        mouseSensitivity = _sensitivity;
    }

    public void SetMoonlightFocus(float _angle)
    {
        moonlightAngle = _angle;
    }

    public void SetWristSensitivity(float _sensitivity)
    {
        wristSensitivity = _sensitivity;
    }

    public void AimMirrorForward() {
        playerWrist.localRotation = Quaternion.Euler(0, 8f, -175f);
    }

    public void SetRotation(float _mouseX)
    {
        mouseX = _mouseX;
    }

    private void Update()
    {

        currentTime = Time.time - startTime;

        if (currentTime > limpDuration)
        {
            isLimping = false;
        }

        if (walkSpeed < maxMoveSpeed)
            walkSpeed = maxMoveSpeed * currentTime / limpDuration;

        if (walkSpeed > maxMoveSpeed)
            walkSpeed = maxMoveSpeed;

        if (walkSpeed < minMoveSpeed)
            walkSpeed = minMoveSpeed;

        if (isLimping)
        {
            if (bobbingSpeed < maxBobSpeed)
                bobbingSpeed = maxBobSpeed * currentTime / limpDuration;

            if (bobbingSpeed < minBobSpeed)
                bobbingSpeed = minBobSpeed;

            if (bobbingAmount > minBobAmount)
                bobbingAmount = maxBobAmount * (limpDuration / limpDuration - (currentTime * 0.1f));

            if (bobbingAmount < minBobAmount)
                bobbingAmount = minBobAmount;
        }

        float waveslice = 0.0f;
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        Debug.Log("Movespeed: "+walkSpeed);

        if (Mathf.Abs(hor) == 0 && Mathf.Abs(ver) == 0)
            {
                timer = 0.0f;
            }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }

        Vector3 v3T = envCam.localPosition;
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(hor) + Mathf.Abs(ver);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            v3T.y = midpoint + translateChange;
        }
        else
        {
            v3T.y = midpoint;
        }
        envCam.localPosition = v3T;

        if (controller.GetInHallway())
        {
            rb.velocity = new Vector3(0, 0, hallVelocity);
        }

        if (horizontal != 0 || vertical != 0)
        {
            if (controller.GetInHallway())
            {
                if (vertical > 0)
                {
                    if (!movingInHallway)
                    {
                        StartCoroutine("MoveForwardHallway");
                    }
                }
                else
                {
                    //Move backward
                }
            }
            else
            {
                PerformMovement();
            }
        }

        if (wristX != 0 || wristY != 0)
        {
            PerformWristMovement();
        }

        if (playerWrist.transform.localRotation.eulerAngles.y > 180f)
        {
            wristAngleY = playerWrist.transform.localRotation.eulerAngles.y - 360;
        }
        else if (playerWrist.transform.localRotation.eulerAngles.y < 180f)
        {
            wristAngleY = playerWrist.transform.localRotation.eulerAngles.y;
        }

        if (wristAngleY > 36f || wristAngleY < -36f)
        {
            if (Mathf.Abs(wristAngleY)<40f)
            {
                //transform.Rotate(0, (wristAngleY / 3) * Time.deltaTime * mirrorTurnSpeed, 0);
                transform.RotateAround(moonlightAnchor.position,Vector3.up, (wristAngleY / 3) * Time.deltaTime * mirrorTurnSpeed);
            }
            else if(Mathf.Abs(wristAngleY) > 40f)
            {
                //transform.Rotate(0, wristAngleY * Time.deltaTime * mirrorTurnSpeed, 0);
                transform.RotateAround(moonlightAnchor.position, Vector3.up, wristAngleY * Time.deltaTime * mirrorTurnSpeed);
            }
        }

        if (mouseX != 0)
        {
            PerformRotation();
        }
    }

    IEnumerator MoveForwardHallway()
    {

        movingInHallway = true;

        anim.Play("Strafe");

        hallVelocity = 2f;

        waitTime = new WaitForSeconds(anim["Strafe"].length);

        Debug.Log("Called routine");

        yield return waitTime;

        hallVelocity = 0f;

        waitTime = new WaitForSeconds(0.01f);

        yield return waitTime;

        movingInHallway = false;
        
    }

    void PerformRotation()
    {
        transform.Rotate(0,mouseX * mouseSensitivity,0);
    }

    public void FocusMirror()
    {
        rb.velocity = Vector3.zero;
    }

    void PerformWristMovement() {
        wristRotX += (wristX * wristSensitivity);
        wristRotX = Mathf.Clamp(wristRotX, -wristMaxRotX, wristMaxRotX);

        wristRotZ += (wristY * wristSensitivity);
        wristRotZ = Mathf.Clamp(wristRotZ, -wristMaxRotZ, -wristMinRotZ);

        playerWrist.localRotation = Quaternion.Euler(0, wristRotX, wristRotZ);
    }

    public void ResetWrist()
    {
        playerWrist.localRotation = defaultWristRot;
    }

    public void SetHallwayMovement(float _vertical)
    {
        vertical = _vertical;
    }

    void PerformMovement()
    {
        Vector3 moveHorizontal = transform.right * horizontal;
        Vector3 moveVertical = transform.forward * vertical;

        Vector3 movement = (moveVertical + moveHorizontal) * walkSpeed;

        //Preserve velocity from gravity
        movement.y = rb.velocity.y;

        rb.velocity = movement;
    }
}
