using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _moveSpeed = 5f;

    [SerializeField] private Material _winMaterial;
    [SerializeField] private Material _loseMaterial;
    [SerializeField] private MeshRenderer _floorMeshRenderer;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
        _targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_targetTransform.localPosition);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        transform.localPosition += new Vector3(moveX, 0f, moveZ) * (Time.deltaTime * _moveSpeed);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuousActionsOut[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            _floorMeshRenderer.material = _winMaterial;
            EndEpisode();
        }
        
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            _floorMeshRenderer.material = _loseMaterial;
            EndEpisode();
        }
    }
}
