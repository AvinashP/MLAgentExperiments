using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodAgent : Agent
{
    public event EventHandler OnAteFood;
    public event EventHandler OnEpisodeBeginEvent;
    
    [SerializeField] private FoodSpawner _foodSpawner;
    [SerializeField] private FoodButton _foodButton;
    
    private Rigidbody _rigidbody;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
        
        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       sensor.AddObservation(_foodButton.CanUseButton()? 1 : 0);
       
       Vector3 dirToFoodButton = (_foodButton.transform.localPosition - transform.localPosition).normalized;
       sensor.AddObservation(dirToFoodButton.x);
       sensor.AddObservation(dirToFoodButton.z);
       
       sensor.AddObservation(_foodSpawner.HasFoodSpawned() ? 1 : 0);

       if (_foodSpawner.HasFoodSpawned())
       {
           Vector3 dirToFood = (_foodSpawner.GetLastFoodTransform().localPosition - transform.localPosition).normalized;
           sensor.AddObservation(dirToFood.x);
           sensor.AddObservation(dirToFood.z);
       }
       else
       {
           // Food not spawned
           sensor.AddObservation(0f); // x
           sensor.AddObservation(0f); // z
       }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.DiscreteActions[0]; // 0 = Don't move; 1 = Left; 2 = Right
        float moveZ = actions.DiscreteActions[1]; // 0 = Don't move; 1 = Back; 2 = Forward

        Vector3 addForce = new Vector3(0f, 0f, 0f);

        switch (moveX)
        {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = +1f; break;
        }

        switch (moveZ)
        {
            case 0: addForce.z = 0f; break;
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = +1f; break;
        }
        
        float moveSpeed = 5f;
        _rigidbody.velocity = addForce * moveSpeed + new Vector3(0, _rigidbody.velocity.y, 0);
        
        bool isUseButtonDown = actions.DiscreteActions[2] == 1;
        if (isUseButtonDown)
        {
            // Use Action
            Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.one * 0.5f);
            foreach (var collider in colliderArray)
            {
                if(collider.TryGetComponent<FoodButton>(out FoodButton foodButton))
                {
                    if (foodButton.CanUseButton())
                    {
                        foodButton.UseButton();
                        AddReward(1f);
                    }
                }
            }
        }
        
        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case +1: discreteActions[0] = 2; break;
        }
        
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case +1: discreteActions[1] = 2; break;
        }
        
        discreteActions[2] = Input.GetKey(KeyCode.E) ? 1 : 0; // Use Action
    }

    private void OnCollisionEnter(Collision other)
    {
        // if (other.collider.TryGetComponent<Food>(out Food food))
        // {
        //     
        // }
    }
}