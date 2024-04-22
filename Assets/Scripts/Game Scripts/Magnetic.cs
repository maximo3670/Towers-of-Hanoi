using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

public class Magnetic : MonoBehaviour{

    public GameObject disk;
    public int diskCount;
    Dictionary<string, PillarData> pillars = new Dictionary<string, PillarData>();
    private Dictionary<GameObject, DiskData> diskPolarity = new Dictionary<GameObject, DiskData>();
    public Vector3[] pillarPositions;
    GameObject selectedDisk = null;
    public int movesCount = 0;
    public bool solved = false;
    public TextMeshProUGUI  movesCountText;
    public GameObject solvedText;
    public GameObject illegalMove;
    public Button returnToMenu;
    public GameObject indicator;
    private GameObject currentIndicator;
    void Start(){
        if(returnToMenu != null)
        {
            returnToMenu.onClick.AddListener(MainMenu);
        }
        solvedText.SetActive(false);
        illegalMove.SetActive(false);
        movesCountText.text = "0";

        //Creating a stack on each pillar
        int i = 0;
        for (char pillarName = 'A'; pillarName <= 'C'; pillarName++)
        {
            pillars["pillar " + pillarName] = new PillarData(pillarPositions[i]);
            i++;
        }

        spawnDisks();

    }

    void Update(){
        //Check to see if the mouse button is pressed
        if(!solved){
            if (Input.GetMouseButtonDown(0))
            {
                //Gets the location of the mouse pointer after mous is pressed
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //Checked to see if the mouse has clicked on something
                if (Physics.Raycast(ray, out hit))
                {
                    //Gets the object that was clicked on
                    GameObject hitObject = hit.collider.gameObject;

                    // Check if the hit object is a disk
                    if (hitObject.CompareTag("Disk"))
                    {
                        // calls the SelectDisk() function
                        SelectDisk(hitObject);
                    }
                    // Checks if a pillar has been selected and is a disk is selected to move the disk
                    else if (selectedDisk != null && hitObject.CompareTag("Pillar"))
                    {
                        // Calls the MoveDisk function 
                        MoveDisk(hitObject);
                    }
                }
            }
        }
    }

    void spawnDisks(){ 
        float positionMove = 0f;
        float scale = 2f;

        for (int i = 0; i < diskCount; i++) {
            GameObject newDisk = Instantiate(disk, new Vector3(10f, 5.5f + positionMove, 6f), Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
            newDisk.transform.localScale = new Vector3(scale, 1.2f, scale);

            // Assign polarity to each disk based on its index
            // bool polarity = i % 2 == 0; // true for even indices, false for odd indices
            DiskData diskData = new DiskData(true);
            diskPolarity[newDisk] = diskData; // Store the disk data

            // Add the newly spawned disk to the stack of the first pillar
            pillars["pillar A"].diskStack.Push(newDisk);

            positionMove = positionMove + 0.26f;
            scale = scale - 0.2f;
        }
    }

    void SelectDisk(GameObject disk) {
        // Gets the pillar the disk is on
        string pillar = GetDiskCurrentPillarName(disk);

        // Move or instantiate the selection indicator
        if (currentIndicator == null) {
            currentIndicator = Instantiate(indicator);
        }

        Vector3 indicatorPosition = GetPositionAbovePillar(pillars[pillar]);
        currentIndicator.transform.position = indicatorPosition;
        // Assigns the selectedDisk variable
        selectedDisk = pillars[pillar].GetTopDisk();
    }

    private Vector3 GetPositionAbovePillar(PillarData pillarData) {
        return pillarData.position + new Vector3(0, 5, 0); // 10 units above the pillar
    }

   void MoveDisk(GameObject pillar)
    {
        //Gets the name of the pillar
        string pillarName = pillar.name;

        //Checks to see if the pillar exists which it should 
        //This prevents any naming errors
        if (pillars.ContainsKey(pillarName))
        {
            //Gets the data from the chosen pillar
            PillarData targetPillar = pillars[pillarName];

            //Gets the current pillar the disk is on
            string currentPillarName = GetDiskCurrentPillarName(selectedDisk);

            //Checks a pillar has been selected to prevent errors
            //Checks its not the same pillar being chosen as it cannot move to the same pillar
            if (currentPillarName != null && currentPillarName != pillarName)
            {
                //Checks if it is a legal move
                if (isLegalMove(targetPillar, pillars[currentPillarName]))
                {
                    //Removes top disk from the stack and adds to the stack of the desired pillar
                    GameObject movedDisk = pillars[currentPillarName].diskStack.Pop();
                    targetPillar.diskStack.Push(movedDisk);

                    //calculates the y position of the disk depending on how many disks there is in the stack
                    float newYPosition = 5.5f + ((targetPillar.diskStack.Count - 1) * 0.26f);

                    //Moves the disk visually with the coordinates of the target pillar
                    movedDisk.transform.position = new Vector3(targetPillar.position.x, newYPosition, targetPillar.position.z);
                    flipDisk(movedDisk);

                    if (selectedDisk != null)
                    {
                        Destroy(currentIndicator);
                    }

                    movesCount++;
                    movesCountText.text = movesCount.ToString();

                    if(isSolved()){
                        solvedText.SetActive(true);
                        solved = true;
                    }
                }
            }
        }
    }

    bool isLegalMove(PillarData targetPillar, PillarData currentPillar)
    {
        // Checks if there are disks on the desired pillar
        if (targetPillar.diskStack.Count > 0)
        {
            // Gets the top disk and the scale of the current disk and the top disk
            GameObject topDisk = targetPillar.GetTopDisk();
            Vector3 selectedDiskScale = selectedDisk.transform.localScale;
            Vector3 topDiskScale = topDisk.transform.localScale;

            // Compares the size of the disks based on their scale
            if (selectedDiskScale.x > topDiskScale.x)
            {
                // Doesn't allow the disk to be moved if it is larger
                illegalMove.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f));
                return false;
            }

            // Retrieve DiskData for polarity comparison
            DiskData selectedDiskData = diskPolarity[selectedDisk];
            DiskData topDiskData = diskPolarity[topDisk];

            // Checks the polarity of the disks
            if (selectedDiskData.getPolarity() == topDiskData.getPolarity())
            {
                // Doesn't allow the disk to be moved if it has the same polarity
                illegalMove.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f));
                return false;
            }
        }
        // else return true if there's no disk on the target pillar or the move is legal
        return true;
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        illegalMove.SetActive(false);
    }

    string GetDiskCurrentPillarName(GameObject disk)
    {
        //loops through all the pillars to see if the stack contains the given disk
        foreach (var pillar in pillars)
        {
            if (pillar.Value.diskStack.Contains(disk))
            {
                return pillar.Key;
            }
        }
        //else return null
        return null;
    }

    public void flipDisk(GameObject disk){
        if (diskPolarity.ContainsKey(disk)) {
            // Access the DiskData and flip its polarity
            diskPolarity[disk].flipPolarity();

            Vector3 currentRotation = disk.transform.eulerAngles;
            disk.transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + 180);

        } else {
            Debug.LogError("Disk not found in polarity dictionary.");
        }
    }

    public bool isSolved(){
        if(pillars["pillar C"].diskStack.Count == diskCount){
            return true;
        }
        return false;
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }

}

public class DiskData{
    public bool polarity;

    public DiskData(bool polarity){
        this.polarity = polarity;
    }

    public bool getPolarity(){
        return this.polarity;
    }

    public void flipPolarity(){
        this.polarity = !this.polarity;
    }
}
