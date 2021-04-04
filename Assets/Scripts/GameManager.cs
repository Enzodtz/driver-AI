using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  List<CarController> agents;
  int framesCountingToReset = 0;

  void Start() {
    agents = new List<CarController>();

    GameObject[] agentsObjects = GameObject.FindGameObjectsWithTag("Car");
    foreach (var agentObject in agentsObjects) {
      agents.Add(agentObject.gameObject.GetComponent<CarController>());        
    }
  }

  void Update() {
    bool allFinished = true;
    foreach (var agent in agents) {
      if(!agent.finished) {
        allFinished = false;
      }
    }
    if(allFinished) {
      if(framesCountingToReset >= 20){
        foreach (var agent in agents) {
          agent.EndEpisode();
        }
      } else {
        framesCountingToReset += 1;
      }
    } else {
      framesCountingToReset = 0;
    }
  }
}
