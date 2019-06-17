using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FishBehaviour))]
public class SwarmAreaBehaviour : MonoBehaviour
{
    [SerializeField]
    private SwarmableArea _swarmArea;

    private float _swarmDist;
    private float _yPosition;
    private SwarmableArea.SwarmFormation _swarmFormation;

    private enum BehaviourMode
    {
        MoveTowardsArea,
        Swarm
    };
    private BehaviourMode _behaviour = BehaviourMode.MoveTowardsArea;

    private Quaternion _expectedRotation;
    private Vector3 _expectedPosition;
    private float _rotationSpeed;

    private FishBehaviour _fishBehaviour;
    private Rigidbody _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _fishBehaviour = GetComponent<FishBehaviour>();
        //_rigidBody = GetComponent<Rigidbody>();

        //Get the swarm distance
        _swarmDist = _swarmArea.GetSwarmDistance();
        //Get the y position
        _yPosition = _swarmArea.GetRandomY();
        //Get the swarm formation
        _swarmFormation = _swarmArea.GetSwarmFormation();

        //Temporary rotation var
        Quaternion currentRotation = transform.rotation;

        _rotationSpeed = SingleTons.GameController.GetRandomRange(0.8f, 1.2f);

        switch (_swarmFormation)
        {
            case SwarmableArea.SwarmFormation.Sphere:
                //Set to random rotation
                _expectedRotation = Quaternion.Euler(SingleTons.GameController.GetRandomRange(0, 360), SingleTons.GameController.GetRandomRange(0, 360), SingleTons.GameController.GetRandomRange(0, 360));

                transform.rotation = _expectedRotation;

                //Set position based on random rotation
                _expectedPosition = _swarmArea.transform.position + -transform.right * _swarmDist;

                transform.rotation = currentRotation;
                break;
            case SwarmableArea.SwarmFormation.Donut:
                //Set to random rotation
                _expectedRotation = Quaternion.Euler(0, SingleTons.GameController.GetRandomRange(0, 360), 0);

                transform.rotation = _expectedRotation;

                //Set position based on random rotation
                _expectedPosition = _swarmArea.transform.position + -transform.right * _swarmDist;

                transform.rotation = currentRotation;

                //Set y position based on height of the area
                _expectedPosition = new Vector3(_expectedPosition.x, _yPosition, _expectedPosition.z);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // _rigidBody.velocity = Vector3.zero;

        SwitchBehaviour();
        ActOnBehaviour();
    }

    private void SwitchBehaviour()
    {
        if (_behaviour == BehaviourMode.MoveTowardsArea)
        {
            if (Vector3.Distance(transform.position, _expectedPosition) < 0.2f)
            {
                transform.position = _expectedPosition;
                transform.rotation = _expectedRotation;
                _behaviour = BehaviourMode.Swarm;
            }
        }
    }

    private void ActOnBehaviour()
    {
        switch (_behaviour)
        {
            case BehaviourMode.MoveTowardsArea:
                _fishBehaviour.GetDummy().transform.LookAt(_expectedPosition, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, _fishBehaviour.GetDummy().transform.rotation, Time.fixedDeltaTime * _fishBehaviour.GetTurningSpeed() * 10);
                transform.position += (transform.forward * Time.fixedDeltaTime * _fishBehaviour.GetMaxSpeed());
                break;
            case BehaviourMode.Swarm:
                switch (_swarmFormation)
                {
                    case SwarmableArea.SwarmFormation.Sphere:
                        transform.RotateAround(_swarmArea.transform.position, Vector3.Cross(transform.position - _swarmArea.transform.position, transform.forward), _rotationSpeed * _swarmArea.GetSwarmSpeed());
                        break;
                    case SwarmableArea.SwarmFormation.Donut:
                        transform.RotateAround(_swarmArea.transform.position, _swarmArea.transform.up, _rotationSpeed * _swarmArea.GetSwarmSpeed());
                        break;
                }
                break;
        }
    }

    public void SetSwarmArea(SwarmableArea swarmArea)
    {
        _swarmArea = swarmArea;
    }

    public void Reset()
    {
        _behaviour = BehaviourMode.MoveTowardsArea;
        _expectedPosition = Vector3.zero;
        _rotationSpeed = 1;
        Start();
    }
}
