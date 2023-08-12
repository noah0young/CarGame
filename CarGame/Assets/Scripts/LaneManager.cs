using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> carPrefabs;
    [SerializeField] private int minNumCarsSpawned = 3;
    [SerializeField] private int maxNumCarsSpawned = 6;
    [SerializeField] private float screenScrollSpeed = 3;
    [SerializeField] private float minCarDistance = 3;
    [SerializeField] private float startSpawnDistance = 30;
    [SerializeField] private float endSpawnDistance = 60;
    [SerializeField] private float laneHeight;
    [SerializeField] private float extraDelay = .3f;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SpawnCars());
    }

    private IEnumerator SpawnCars()
    {
        while (true)
        {
            List<float> alreadySelectedXPos = new List<float>();
            int numCarsSpawned = Random.Range(minNumCarsSpawned, maxNumCarsSpawned);
            for (int i = 0; i < numCarsSpawned; i++)
            {
                GameObject chosenPreFab = GetRandomFromList<GameObject>(carPrefabs);
                GameObject newCar = Instantiate(chosenPreFab, transform);
                newCar.GetComponent<BasicCar>().SetScreenScrollSpeed(screenScrollSpeed);
                float xPos = SelectXPos(alreadySelectedXPos);
                if (xPos > 0)
                {
                    alreadySelectedXPos.Add(xPos);
                    newCar.transform.position = new Vector2(xPos, laneHeight);
                }
                else
                {
                    // The car couldn't be placed, so end the loop
                    Destroy(newCar);
                    break;
                }
            }
            alreadySelectedXPos.Clear();
            yield return new WaitForSeconds((endSpawnDistance - startSpawnDistance) / screenScrollSpeed + extraDelay);
            // This is the time it takes for the game to reach the end of the spawned cars
        }
    }

    private float SelectXPos(List<float> alreadySelectedXPos)
    {
        float xPos = -1;
        int numAllowedRerolls = 20;
        while (xPos == -1 || InRangeOfCar(alreadySelectedXPos, xPos, minCarDistance))
        {
            numAllowedRerolls -= 1;
            if (numAllowedRerolls <= 0)
            {
                return -1;
                // No positions are available
            }
            xPos = Random.Range(startSpawnDistance, endSpawnDistance);
        }
        return xPos;
    }

    public T GetRandomFromList<T>(List<T> list)
    {
        int chosen = Random.Range(0, list.Count);
        return list[chosen];
    }

    private bool InRangeOfCar(List<float> alreadySelectedXPos, float xPos, float minDistance)
    {
        foreach (float selectedXPos in alreadySelectedXPos)
        {
            if (Mathf.Abs(xPos - selectedXPos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }
}
