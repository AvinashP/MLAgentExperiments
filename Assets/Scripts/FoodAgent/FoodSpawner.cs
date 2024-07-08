using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform _foodTransform;
    
    public bool HasFoodSpawned()
    {
        return true;
    }
    
    public Transform GetLastFoodTransform()
    {
        return _foodTransform;
    }
}