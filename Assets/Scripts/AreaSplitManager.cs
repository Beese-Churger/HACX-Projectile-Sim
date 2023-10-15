using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSplitManager : MonoBehaviour
{
    public int numberOfAreas = 5; // Set the number of equally distanced areas here
    [Tooltip("How far the area has to be from a collidable object to actually spawn properly")]
    public float DeadzoneRadius = 0.5f;
    public GameObject objectToSpawn; // The object you want to spawn
    public MainGameManager mainGameManager;
    public int row;
    private void Start()
    {
        SpawnCulprits();
    }
    bool CheckCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, DeadzoneRadius); // Adjust the radius as needed

        return colliders.Length > 0;
    }

    public void SpawnCulprits()
    {
        if (!mainGameManager) mainGameManager = MainGameManager.instance;
        Vector3 boxSize = GetComponent<Renderer>().bounds.size;
        float areaWidth = boxSize.x / numberOfAreas;
        int s = 1;
        for (int i = 0; i < numberOfAreas; i++)
        {

            float spawnPositionX = transform.position.x - (boxSize.x / 2) + (areaWidth * i) + (areaWidth / 2);
            Vector3 spawnPosition = new Vector3(spawnPositionX, transform.position.y, transform.position.z);
            
            if (!CheckCollision(spawnPosition))
            {
                GameObject GO =Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                mainGameManager.SpawnedCulprits.Add(GO);
                GO.name = "Culprit" + mainGameManager.SpawnedCulprits.Count;
                Culprit culprit = GO.GetComponent<Culprit>();
                culprit.column = s;
                culprit.row = row;
                s++;
                mainGameManager.CulpritPositions.Add(culprit.ShootPosition.position);
            }
        }
    }
}
