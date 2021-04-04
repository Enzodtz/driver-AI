using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class CarController : Agent {
	public float maxSteerAngle = 30;
	public float motorForce = 10;
  public float maxSpeed = 20;

  public bool finished = false;

  private Rigidbody carRigidbody;
  private Collider bodyCollider;

	private WheelCollider FRWCollider;
	private WheelCollider FLWCollider;
  private WheelCollider RRWCollider;
  private WheelCollider RLWCollider;

	private Transform FRWTransform;
  private Transform FLWTransform;
	private Transform RRWTransform; 
  private Transform RLWTransform; 

  private float horizontalInput;
	private float verticalInput;
	private float steeringAngle;

  public void Start() {
    carRigidbody = gameObject.GetComponent<Rigidbody>();
    bodyCollider = gameObject.transform.Find("Body").GetComponent<Collider>();

    Transform wheelsWrapper = gameObject.transform.Find("Wheels");
    Transform wheelMeshes = wheelsWrapper.Find("Meshes");
    Transform wheelColliders = wheelsWrapper.Find("Colliders");

    FRWCollider = wheelColliders.Find("FRW").GetComponent<WheelCollider>();
    FLWCollider = wheelColliders.Find("FLW").GetComponent<WheelCollider>();
    RRWCollider = wheelColliders.Find("RRW").GetComponent<WheelCollider>();
    RLWCollider = wheelColliders.Find("RLW").GetComponent<WheelCollider>();

    FRWTransform = wheelMeshes.Find("FRW");
    FLWTransform = wheelMeshes.Find("FLW");
    RRWTransform = wheelMeshes.Find("RRW");
    RLWTransform = wheelMeshes.Find("RLW");  
  }

  void OnCollisionEnter(Collision collision) {
    if(collision.gameObject.tag == "Wall"){
      if(!finished){
        Brake();
        finished = true;
      }
    }
  }

  void OnTriggerEnter(Collider collisionCollider) {
    if(!finished) {
      if(collisionCollider.tag == "FinishLine") {
        AddReward(1.0f);
        Brake();
        finished = true;
      } else if (collisionCollider.tag == "Checkpoint" || collisionCollider.tag == "StartLine"){
        AddReward(1.0f);
      }
    }
  }

	private void Steer(float horizontalInput) {
		steeringAngle = maxSteerAngle * horizontalInput;
		FLWCollider.steerAngle = steeringAngle;
		FRWCollider.steerAngle = steeringAngle;
	}

  private void Unbrake() {
    FLWCollider.brakeTorque = 0;
    FRWCollider.brakeTorque = 0;
  }

  private void Brake() {
    FLWCollider.steerAngle = 0;
    FRWCollider.steerAngle = 0;
    FLWCollider.motorTorque = 0;
    FRWCollider.motorTorque = 0;
    FRWCollider.brakeTorque = Mathf.Infinity;
    FLWCollider.brakeTorque = Mathf.Infinity;
    carRigidbody.velocity = Vector3.zero;
    carRigidbody.angularVelocity = Vector3.zero;
    UpdateWheelPositions();
  }

	private void Accelerate(float verticalInput) {
    float speed = carRigidbody.velocity.sqrMagnitude;

    float newTorque;
    if(speed < maxSpeed && speed > -maxSpeed) {
      newTorque = Mathf.Pow(verticalInput * motorForce, 2);
      if(verticalInput < 0){
        newTorque *= -1;
      }
    } else {
      newTorque = 0;
    }

    FLWCollider.motorTorque = newTorque;
    FRWCollider.motorTorque = newTorque;
  }

	private void UpdateWheelPositions() {
		UpdateWheelPosition(FLWCollider, FLWTransform);
		UpdateWheelPosition(FRWCollider, FRWTransform);
		UpdateWheelPosition(RLWCollider, RLWTransform);
		UpdateWheelPosition(RRWCollider, RRWTransform);
	}

	private void UpdateWheelPosition(WheelCollider wheelCollider, Transform wheelTransform) {
		Vector3 pos = wheelTransform.position;
		Quaternion quat = wheelTransform.rotation;

		wheelCollider.GetWorldPose(out pos, out quat);

		wheelTransform.position = pos;
		wheelTransform.rotation = quat;
	}

	public override void OnActionReceived(ActionBuffers actionBuffers) {
    if(!finished) {
      var continuousActions = actionBuffers.ContinuousActions;

      float moveVertical = continuousActions[0];
      float moveHorizontal = continuousActions[1];

      Steer(moveHorizontal);
      Accelerate(moveVertical);
      UpdateWheelPositions();
    }
	}

  public override void Heuristic(in ActionBuffers actionsOut) {
    var continuousActionsOut = actionsOut.ContinuousActions;
    
    continuousActionsOut[0] = Input.GetAxis("Vertical");
    continuousActionsOut[1] = Input.GetAxis("Horizontal");
  }

  public override void OnEpisodeBegin() {
    Unbrake();
    gameObject.transform.position = new Vector3(Random.Range(6f, -6f), 0, 0);
    gameObject.transform.rotation = Quaternion.identity;
    finished = false;
  }
}
