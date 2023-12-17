using System;
using System.Collections.Generic;
using UnityEngine;

public class DiskSpawner : MonoBehaviour
{
    public GameObject disk;
    public int diskCount;
    GameObject selectedDisk = null;
    GameObject previouslySelectedDisk = null;
    Dictionary<string, PillarData> pillars = new Dictionary<string, PillarData>();
    public Vector3[] pillarPositions;

    // Stack to keep track of spawned disks
    Stack<GameObject> spawnedDisks = new Stack<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        for (char pillarName = 'A'; pillarName <= 'C'; pillarName++)
        {
            pillars["pillar " + pillarName] = new PillarData(pillarPositions[i]);
            i++;
        }

        SpawnDisks();
    }

    void SpawnDisks()
    {
        // Destroy previously spawned disks
        while (spawnedDisks.Count > 0)
        {
            GameObject oldDisk = spawnedDisks.Pop();
            Destroy(oldDisk);
        }

        // Setting initial values
        float positionMove = 0f;

        // From testing these values were found to be best for the size of the disks
        float scale = 75 + (10 * diskCount);

        // For loop to instantiate all the disks
        for (int i = 0; i < diskCount; i++)
        {
            // Creates a new disk with the position moving each loop
            // Sets the size of the disk which changes in each loop
            GameObject newDisk = Instantiate(disk, new Vector3(10f, 5.5f + positionMove, 6f), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
            newDisk.transform.localScale = new Vector3(scale, scale, 12f);

            // Assigns a color to each disk (red, green, blue in a loop)
            newDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            newDisk.tag = "Disk";

            // Add the newly spawned disk to the stack of the first pillar
            pillars["pillar A"].diskStack.Push(newDisk);

            // New position and scale. These values were found from testing to be the best
            // to ensure the disks were not touching
            positionMove = positionMove + 0.26f;
            scale = scale - 20;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Check if the hit object is a disk
                if (hitObject.CompareTag("Disk"))
                {
                    // Handle disk click
                    SelectDisk(hitObject);
                }
                else if (selectedDisk != null && hitObject.CompareTag("Pillar"))
                {
                    MoveDisk(hitObject);
                }
            }
        }
    }

    void SelectDisk(GameObject disk)
    {
        String pillar = GetDiskCurrentPillarName(disk);
        if (selectedDisk != null)
        {
            // Change the color of the previously selected disk back to red
            previouslySelectedDisk = selectedDisk;
            previouslySelectedDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }

        selectedDisk = pillars[pillar].GetTopDisk();
        selectedDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        UnityEngine.Debug.Log(selectedDisk);
    }

    void MoveDisk(GameObject pillar)
    {
        string pillarName = pillar.name;

        if (pillars.ContainsKey(pillarName))
        {
            PillarData targetPillar = pillars[pillarName];

            // Get the selected disk's current pillar
            string currentPillarName = GetDiskCurrentPillarName(selectedDisk);

            if (currentPillarName != null && currentPillarName != pillarName)
            {
                // Check if the target pillar is not empty
                if (targetPillar.diskStack.Count > 0)
                {
                    // Get the top disk in the target pillar
                    GameObject topDisk = targetPillar.diskStack.Peek();

                    // Get the scale of the selected disk and the top disk in the target pillar
                    Vector3 selectedDiskScale = selectedDisk.transform.localScale;
                    Vector3 topDiskScale = topDisk.transform.localScale;

                    // Compare the x scales of the disks
                    if (selectedDiskScale.x > topDiskScale.x)
                    {
                        // Disallow the move as it violates the rule (larger disk on a smaller one)
                        UnityEngine.Debug.Log("Cannot place larger disk on a smaller one!");
                        return;
                    }
                }

                // Move the disk from its current pillar to the target pillar
                GameObject movedDisk = pillars[currentPillarName].diskStack.Pop();
                targetPillar.diskStack.Push(movedDisk);

                // Calculate the new Y position based on the number of disks on the target pillar
                float newYPosition = 5.5f + ((targetPillar.diskStack.Count - 1) * 0.26f);

                // Update the disk's position to the new pillar's top position
                movedDisk.transform.position = new Vector3(targetPillar.position.x, newYPosition, targetPillar.position.z);
            }
        }
    }


    

    string GetDiskCurrentPillarName(GameObject disk)
    {
        foreach (var pillar in pillars)
        {
            if (pillar.Value.diskStack.Contains(disk))
            {
                return pillar.Key;
            }
        }
        return null;
    }
}

public class PillarData
{
    public Stack<GameObject> diskStack;
    public Vector3 position;

    public PillarData(Vector3 pos)
    {
        diskStack = new Stack<GameObject>();
        position = pos;
    }

       public GameObject GetTopDisk()
    {
        if (diskStack.Count > 0)
        {
            return diskStack.Peek();
        }
        return null; 
    }
}
    
