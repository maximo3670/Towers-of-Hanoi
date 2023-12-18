using System;
using System.Collections.Generic;
using UnityEngine;

public class DiskSpawner : MonoBehaviour
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
    //Dictionary linking the pillar name with the pillar data
    Dictionary<string, PillarData> pillars = new Dictionary<string, PillarData>();
    //Tracking the position of the pillars
    public Vector3[] pillarPositions;

    // Start is called before the first frame update
    void Start()
    {
        //Creating a stack on each pillar
        int i = 0;
        for (char pillarName = 'A'; pillarName <= 'C'; pillarName++)
        {
            pillars["pillar " + pillarName] = new PillarData(pillarPositions[i]);
            i++;
        }

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

        // From testing these values were found to be best for the size of the disks
        float scale = 120 + (10 * diskCount);

        // For loop to instantiate all the disks
        for (int i = 0; i < diskCount; i++)
        {
            // Creates a new disk with the position moving each loop
            // Sets the size and position of the disk which changes in each loop
            GameObject newDisk = Instantiate(disk, new Vector3(10f, 5.5f + positionMove, 6f), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
            newDisk.transform.localScale = new Vector3(scale, scale, 12f);

            // Assigns a color to each disk 
            newDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);

            // Add the newly spawned disk to the stack of the first pillar
            pillars["pillar A"].diskStack.Push(newDisk);

            // New position and scale. These values were found from testing to be the best
            // to ensure the disks were not touching
            positionMove = positionMove + 0.26f;
            scale = scale - 20;
        }
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
            previouslySelectedDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }

        //Assigns the selectedDisk variable
        //If a disk on the bottom of a pillar is selected it automatically changes to the top disk
        //This is to stop the bottom disks from being able to be moved
        selectedDisk = pillars[pillar].GetTopDisk();

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
                }
            }
        }
    }
    

    //Function isLegalMove(PillarData, PillarData)
    //This function checks if the disks can move depening on the size of the disk
    bool isLegalMove(PillarData targetPillar, PillarData currentPillar)
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
                return false;
            }
        }
        //else return true
        return true;
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
}

// Object PillarData
// Stores the data for each pillar 
public class PillarData
{
    public Stack<GameObject> diskStack;
    public Vector3 position;

    public PillarData(Vector3 pos)
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
    