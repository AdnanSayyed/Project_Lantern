using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerEquipment : MonoBehaviour {

    private PlayerInventory inventory;
    private PlayerController controller;
    private NotepadManager notepadManager;
    private GameObject currentlyEquipped;
    private string hasEquipped = "";
    private Animation anim;
    private WaitForSeconds waitTime;
    private bool playingAnim = false;

    [SerializeField]
    private GameObject notepadCanvas;

    [SerializeField]
    private GameObject lampPrefab;

    [SerializeField]
    private Transform lampAnchor;

    [SerializeField]
    private GameObject mirrorPrefab;

    [SerializeField]
    private Transform mirrorAnchor;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        anim = GetComponent<Animation>();
        controller = GetComponent<PlayerController>();
        notepadManager = GameObject.Find("Notepad").GetComponent<NotepadManager>();
    }

    public bool HasEquippedItem(string objName) {
        if (hasEquipped == objName)
        {
            return true;
        }
        else {
            return false;
        }
    }

    public void TurnOnMirror() {

        currentlyEquipped.GetComponent<Mirror>().TurnOn();

    }

    public bool GetPlayingAnim() {
        return playingAnim;
    }

    public bool ToggleInventoryUI() {

        if (!notepadCanvas.activeInHierarchy){
            notepadCanvas.SetActive(true);
        }
        else {
            notepadCanvas.SetActive(false);
        }

        return notepadCanvas.activeInHierarchy;

    }

    public bool ToggleInventoryUI(bool state)
    {
        if (state)
        {
            notepadCanvas.SetActive(true);
        }
        else
        {
            notepadCanvas.SetActive(false);
        }

        return notepadCanvas.activeInHierarchy;
    }

    public string GetEqippedItemName() {
        return hasEquipped;
    }

    public void Equip(string objName) {

        if (inventory.HasItemInInventory(objName))
        {

            if (!playingAnim) {
                StartCoroutine(EquipItem(objName));
            }

        }
        else {
            Debug.LogError("Player doesn't have a " + objName + " in inventory.");
        }

    }

    IEnumerator EquipItem(string objName) {

        if (objName == hasEquipped)
        {
            Unequip();
            yield break;
        }

            //Check if there is any object in hand
        if (hasEquipped != "")
        {

            //Lower previous object
            switch (hasEquipped)
            {

                case "mirror":

                    playingAnim = true;
                    anim.Play("LowerHandMirror");
                    waitTime = new WaitForSeconds(anim["LowerHandMirror"].length);
                    yield return waitTime;
                    Destroy(currentlyEquipped);
                    hasEquipped = "";
                    playingAnim = false;

                    break;

                case "lamp":

                    playingAnim = true;
                    anim.Play("LowerHandLamp");
                    waitTime = new WaitForSeconds(anim["LowerHandLamp"].length);
                    yield return waitTime;
                    Destroy(currentlyEquipped);
                    hasEquipped = "";
                    playingAnim = false;

                    break;

            }
            
        }

        
        //Instantiate new object in hand and animate the hand back to ideal position
        switch (objName)
        {

            case "mirror":

                playingAnim = true;
                currentlyEquipped = (GameObject)Instantiate(mirrorPrefab, mirrorAnchor);
                anim.Play("RaiseHandMirror");
                waitTime = new WaitForSeconds(anim["RaiseHandMirror"].length);
                yield return waitTime;
                hasEquipped = objName;
                playingAnim = false;

                break;

            case "lamp":

                playingAnim = true;
                currentlyEquipped = (GameObject)Instantiate(lampPrefab, lampAnchor);
                anim.Play("RaiseHandLamp");
                waitTime = new WaitForSeconds(anim["RaiseHandLamp"].length);
                yield return waitTime;
                hasEquipped = objName;
                playingAnim = false;

                break;
        }

    }

    public GameObject GetEquippedItem() {
        return currentlyEquipped;
    }

    public void Unequip() {
        
        if (hasEquipped == "")
        {
            Destroy(currentlyEquipped);
        }
        else if(hasEquipped != "" && !playingAnim){
            StartCoroutine(UnequipItem(hasEquipped));
        }
        
        
    }

    IEnumerator UnequipItem(string objName) {

        switch (objName) {

            case "mirror":

                if (!controller.GetUsingMirror()) {
                    playingAnim = true;
                    anim.Play("LowerHandMirror");
                    waitTime = new WaitForSeconds(anim["LowerHandMirror"].length);
                    yield return waitTime;
                    Destroy(currentlyEquipped);
                    hasEquipped = "";
                    playingAnim = false;
                }

                break;

            case "lamp":

                playingAnim = true;
                anim.Play("LowerHandLamp");
                waitTime = new WaitForSeconds(anim["LowerHandLamp"].length);
                yield return waitTime;
                Destroy(currentlyEquipped);
                hasEquipped = "";
                playingAnim = false;

                break;
        }

        notepadManager.EnableBasePage();

    }

    

}
