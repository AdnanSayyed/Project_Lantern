using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerEquipment))]
public class PlayerInventory : MonoBehaviour {

    [SerializeField]
    private GameObject mirrorNotebookTab;

    [SerializeField]
    private GameObject lanternNotebookTab;

    private NotepadManager notepadManager;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Start()
    {
        inventory.Clear();
        notepadManager = GameObject.Find("Notepad").GetComponent<NotepadManager>();
    }

    public bool HasItemInInventory(string objName)
    {

        if (inventory.ContainsKey(objName) && inventory[objName] > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void PickUp(string objName)
    {

        if (HasItemInInventory(objName))
            return;

        inventory.Add(objName, 1);
        Debug.Log("Picked up " + objName + " !");

        switch (objName) {

            case "mirror":
                
                notepadManager.AddItem("artifact", mirrorNotebookTab);

                break;

            case "lamp":
                
                notepadManager.AddItem("artifact", lanternNotebookTab);

                break;
        }
    }

}
