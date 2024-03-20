using UnityEngine;
using UnityEngine.UI; // Required for working with UI components
using TMPro; // Required if using TextMeshPro Dropdowns
using System;
using System.Collections.Generic;

public class DropdownHandler : MonoBehaviour
{
    public leaderBoardHandler leaderBoardHandler; // Assign this in the inspector
    public TMP_Dropdown diskCountDropdown; // Assign this in the inspector

    void Start()
    {
        // Add listener for when the dropdown value changes
        diskCountDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(diskCountDropdown);
        });
    }

    // This method is called whenever the dropdown value changes
    void DropdownValueChanged(TMP_Dropdown change)
    {
        int diskCount = change.value + 3; // Assuming the first option is for 3 disks
        leaderBoardHandler.UpdateDisplayedLeaderboard(diskCount);
    }
}