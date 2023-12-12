using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class diskSpawner : MonoBehaviour
{
    public GameObject disk;
    public int diskCount;
    // Stack to keep track of spawned disks
    Stack<GameObject> spawnedDisks = new Stack<GameObject>(); 

    // Update is called once per frame
    void Update()
    {
        // Check to see if space bar is pressed
        // If pressed, the disks will spawn in
        if (Input.GetKeyDown(KeyCode.Space))
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

            // Array of colors to cycle through
            Color[] colors = { Color.red, Color.green, Color.blue };

            // For loop to instantiate all the disks
            for (int i = 0; i < diskCount; i++)
            {
                // Creates a new disk with the position moving each loop
                // Sets the size of the disk which changes in each loop
                GameObject newDisk = Instantiate(disk, new Vector3(10f, 5.5f + positionMove, 6f), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
                newDisk.transform.localScale = new Vector3(scale, scale, 12f);

                // Assigns a color to each disk (red, green, blue in a loop)
                newDisk.GetComponent<MeshRenderer>().material.SetColor("_Color", colors[i % 3]);

                // Add the newly spawned disk to the stack
                spawnedDisks.Push(newDisk); 

                // New position and scale. These values were found from testing to be the best
                // to ensure the disks were not touching
                positionMove = positionMove + 0.26f;
                scale = scale - 20;
            }
        }
    }
}
