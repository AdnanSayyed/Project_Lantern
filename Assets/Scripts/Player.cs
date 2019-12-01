using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour {

    [SerializeField]
    private float indicatorTriggerDistance = 10f;

    [SerializeField]
    private GameObject statueObject;

    [SerializeField]
    private GameObject hallwayBlock;

    [SerializeField]
    private GameObject hallwayJumpScare;

    [SerializeField]
    private float hallwayJumpScareSpeed = 10f;

    [SerializeField]
    Transform hallwayEntrance;

    [SerializeField]
    Camera envCam;

    [SerializeField]
    Camera hallCam;

    [SerializeField]
    Transform respawnPoint;

    [SerializeField]
    Camera handCam;

    public float GetTriggerDistance()
    {
        return indicatorTriggerDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Scare1")
        {
            statueObject.SetActive(true);
        }else if (other.gameObject.tag == "HallwayActivate")
        {
            envCam.enabled = false;
            handCam.enabled = false;
            hallCam.enabled = true;
            GetComponent<PlayerController>().SetInHallway(true);
            transform.position = hallwayEntrance.position;
            transform.rotation = Quaternion.Euler(0,0,0);
            Debug.Log("In Hallway");
        }
        else if (other.gameObject.tag == "HallwayDeactivate")
        {
            envCam.enabled = true;
            handCam.enabled = true;
            hallCam.enabled = false;
            GetComponent<PlayerController>().SetInHallway(false);
            hallwayBlock.SetActive(true);
            Debug.Log("Exited Hallway");
        }

        if (other.gameObject.tag == "HallwayJumpScare")
        {
            hallwayJumpScare.SetActive(true);
            hallwayJumpScare.GetComponent<Rigidbody>().velocity = hallwayJumpScare.transform.right * -hallwayJumpScareSpeed;
            Destroy(hallwayJumpScare, 5);
        }

        if (other.gameObject.tag == "Death")
        {
            transform.position = respawnPoint.position;
        }

        if (statueObject.activeInHierarchy && other.gameObject.tag=="StopScare1")
        {
            statueObject.SetActive(false);
        }
    }

}
