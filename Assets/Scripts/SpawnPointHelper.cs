using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script does error checking to make sure the player spawnpoints are indexed correctly
//It goes through all possible cases of an index being bad for that given spawnpoint to make sure that they are made correctly
public class SpawnPointHelper : MonoBehaviour
{
    [Tooltip("Add a SpawnPoint Index" +
             "\nMust be between 0 - #OfSpawnpoints-1" +
             "\nAny number less than 0 is invalid" + 
             "\nPlease start at 0")]
    public int spawnPointIndex = -1;

    //[HideInInspector]
    //public bool spawnPointUsed = false;
    //Used by instantiation script to indicate whether or not a player has used this spawnPoint
    //(Wasn't actually used)

    [HideInInspector]
    public bool gaveWarning = false;
    //We only want to give the spawnpoint warning once, not everytime, otherwise we would be spewing out many errors for one spawnpoint being bad for all spawnpoints

    private void Start()
    {
        GameObject[] spawnpoints;
        spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //The first error check occurs to make sure you choose a positive number, as an index can't be negative
        //-1 is also the default number, so this is used to make sure you pikced a default number
        if (spawnPointIndex < 0)
        {
            int minSpawnPoint = 0;
            //If a spawnpoint is given that's less than 0, this for loop finds the minimum available spawnpoint and takes it
            foreach (GameObject spawnpoint in spawnpoints)
            {
                if (spawnpoint.GetComponent<SpawnPointHelper>().spawnPointIndex > minSpawnPoint)
                    minSpawnPoint = spawnpoint.GetComponent<SpawnPointHelper>().spawnPointIndex;
            }
            if (minSpawnPoint != 0)
                minSpawnPoint += 1;
            spawnPointIndex = minSpawnPoint;
            Debug.LogError("Invalid spawnpoint index in " + this.name + "\nPlease select a valid spawn index, currently defaulting index to " + minSpawnPoint);
        }
        else
        {
            //The following for loop makes sure we do not have duplicate spawn point indexes
            foreach (GameObject spawnpoint in spawnpoints)
            {
                if (spawnpoint.GetComponent<SpawnPointHelper>().spawnPointIndex == spawnPointIndex)
                    if (spawnpoint.name != this.name)
                        Debug.LogError("Invalid spawnpoint index in " + this.name + " and " + spawnpoint.name + "\nIndex is used twice");
            }
        }

        //The following for loop makes sure that we choose a number that is valid given the amount of spawnpoints
        //For example: If the spawnpoint index is 6, but there is also 4 spawnpoint locations, then it is invalid since there should only be numbers between 0-3
        foreach (GameObject spawnpoint in spawnpoints)
        {
            if (spawnpoint.GetComponent<SpawnPointHelper>().spawnPointIndex > spawnpoints.Length && !spawnpoint.GetComponent<SpawnPointHelper>().gaveWarning)
            {
                spawnpoint.GetComponent<SpawnPointHelper>().gaveWarning = true;
                Debug.LogError("Spawnpoint: " + spawnpoint.name + " has an invalid index:\n The index is too high! Please order your spawnpoints");
            }
        }
    }
}
