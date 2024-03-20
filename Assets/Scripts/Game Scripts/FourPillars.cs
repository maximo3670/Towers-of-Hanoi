using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

public class FourPillars : MonoBehaviour
{
    //Initialising variables
    //Getting the disk object from Unity
    public GameObject disk;
    //Tracking number of disks 
    public int diskCount;
    //Storing the disk which is selected
    GameObject selectedDisk = null;
    //Storing the previously selected disk 
    GameObject previouslySelectedDisk = null;
    Color previousColor = Color.red;
    //Dictionary linking the pillar name with the pillar data
    Dictionary<string, FourPillarData> pillars = new Dictionary<string, FourPillarData>();
    //Tracking the position of the pillars
    public Vector3[] pillarPositions;
    public GameObject illegalMove;
    public GameObject solvedText;
    public Button returnToMenu;
    public bool solved = false;
    public int movesCount = 0;
    public TextMeshProUGUI  movesCountText;

    // Start is called before the first frame update
    void Start()
    {
        if(returnToMenu != null){
            returnToMenu.onClick.AddListener(MainMenu);
        }

        //Creating a stack on each pillar
        int i = 0;
        for (char pillarName = 'A'; pillarName <= 'D'; pillarName++)
        {
            pillars["pillar " + pillarName] = new FourPillarData(pillarPositions[i]);
            i++;
        }

        solvedText.SetActive(false);
        illegalMove.SetActive(false);
        movesCountText.text = "0";

        //spawnin in the disks
        SpawnDisks();
    }

    // update is called once per frame
    void Update()
    {
        //Check to see if the mouse button is pressed
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

    //Funtion SpawnDisks()
    //Used to spawn the disks into the scene
    //Spawns them on the left most pillar and stores them in the stack of pillar A
    void SpawnDisks()
    {
        // Setting initial values
        float positionMove = 0f;
        float scale = 120 + (10 * diskCount);

        // For loop to instantiate all the disks for both pillars
        for (int i = 0; i < diskCount; i++)
        {
            // Create disk for pillar A
            GameObject diskPillarA = InstantiateDisk(positionMove, scale, Color.red, 4.72f);
            pillars["pillar A"].diskStack.Push(diskPillarA);

            // Create disk for pillar D
            GameObject diskPillarD = InstantiateDisk(positionMove, scale, Color.blue, 12.97f);
            pillars["pillar D"].diskStack.Push(diskPillarD);

            // Update position and scale
            positionMove = positionMove + 0.26f;
            scale = scale - 20;
        }
    }

    GameObject InstantiateDisk(float positionMove, float scale, Color color, float z)
    {
        GameObject newDisk = Instantiate(disk, new Vector3(10f, 5.5f + positionMove, z), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
        newDisk.transform.localScale = new Vector3(scale, scale, 12f);
        newDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", color);

        return newDisk;
    }

    //Function SelectDisk(GameObject)
    //This function is to handle selecting a disk
    //The selectDisk variable is assigned the disk and the colour is changed to green
    //Reverts the colour of a previously selected disk
    void SelectDisk(GameObject disk)
    {


        //Gets the pillar the disk is on
        String pillar = GetDiskCurrentPillarName(disk);

        //Checks if a disk is already selected
        if (selectedDisk != null)
        {
            // Change the color of the previously selected disk back to red
            previouslySelectedDisk = selectedDisk;
            previouslySelectedDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", previousColor);
        }

        //Assigns the selectedDisk variable
        //If a disk on the bottom of a pillar is selected it automatically changes to the top disk
        //This is to stop the bottom disks from being able to be moved
        selectedDisk = pillars[pillar].GetTopDisk();
        previousColor = selectedDisk.GetComponent<MeshRenderer>().material.color;
        //Changes the colour to green
        selectedDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
    }

    //Function MoveDisk(GameObject)
    //This function is to handle moving the disk to a different pillar
    //It also checks to see if it is a legal move (large disk cannot go on a smaller disk)
    void MoveDisk(GameObject pillar)
    {
        //Gets the name of the pillar
        string pillarName = pillar.name;

        //Checks to see if the pillar exists which it should 
        //This prevents any naming errors
        if (pillars.ContainsKey(pillarName))
        {
            //Gets the data from the chosen pillar
            FourPillarData targetPillar = pillars[pillarName];

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
                
                    if (selectedDisk != null)
                    {
                        // Reset the color
                        selectedDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", previousColor);

                        // Now, clear or nullify the selectedDisk as it's no longer selected after moving
                        selectedDisk = null;
                        previouslySelectedDisk = null; // Clear this as well if you're done with the move
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
    

    //Function isLegalMove(FourPillarData, FourPillarData)
    //This function checks if the disks can move depening on the size of the disk
    bool isLegalMove(FourPillarData targetPillar, FourPillarData currentPillar)
    {
        //Checks if there are disks on the desired pillar
        if (targetPillar.diskStack.Count > 0)
        {
            //Gets the top disk and the scale of the current disk and the top disk
            GameObject topDisk = targetPillar.GetTopDisk();
            Vector3 selectedDiskScale = selectedDisk.transform.localScale;
            Vector3 topDiskScale = topDisk.transform.localScale;

            //Compares the size of the disks based on their scale
            if (selectedDiskScale.x > topDiskScale.x)
            {
                //doesn't allow the disk to be moved if it is larger
                UnityEngine.Debug.Log("Cannot place larger disk on a smaller one!");
                illegalMove.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f));
                return false;
            }
        }
        //else return true
        return true;
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        illegalMove.SetActive(false);
    }


    //Function GetDiskCurrentPillarName(GameObject)
    //This function returns the name of the pillar a given disk is on
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

    public bool isSolved()
    {
        // Ensure each pillar has the correct number of disks for the win condition
        if (pillars["pillar A"].diskStack.Count == diskCount && pillars["pillar D"].diskStack.Count == diskCount)
        {
            // Check if all disks on pillar A are now blue
            bool pillarAIsCorrect = true;
            foreach (var disk in pillars["pillar A"].diskStack)
            {
                if (disk.GetComponent<MeshRenderer>().material.color != Color.blue)
                {
                    print("False");
                    pillarAIsCorrect = false;
                    break;
                }
            }

            // Check if all disks on pillar D are now red
            bool pillarDIsCorrect = true;
            foreach (var disk in pillars["pillar D"].diskStack)
            {
                if (disk.GetComponent<MeshRenderer>().material.color != Color.red)
                {
                    print("False");
                    pillarDIsCorrect = false;
                    break;
                }
            }

            return pillarAIsCorrect && pillarDIsCorrect;
        }
        return false;
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }

}

// Object FourPillarData
// Stores the data for each pillar 
public class FourPillarData
{
    //Pillar attributes
    public Stack<GameObject> diskStack;
    public Vector3 position;

    //Pillar constructor
    public FourPillarData(Vector3 pos)
    {
        diskStack = new Stack<GameObject>();
        position = pos;
    }


    //Function GetTopDisk()
    //this function returns the disk at the top of the stack
       public GameObject GetTopDisk()
        {
        if (diskStack.Count > 0)
        {
            return diskStack.Peek();
        }
        return null; 
        }
}
    
