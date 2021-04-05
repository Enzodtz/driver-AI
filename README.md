# driver-AI
A Machine Learning project to teach the agent how to drive and race.

<p align="center">
  <img src="https://user-images.githubusercontent.com/7780770/113526464-8b4f6e80-9590-11eb-9b0d-9e7a220bd3a7.gif">
</p>

## Running

The project is developed using the Unity MLAgents package, so there are 3 modes that you can run it. To change the mode, `Inspect` the Agent, and open the `Behaviour Parameters` group, then you will be able to select one.

#### Default

This is where you train your agents, the MLAgents package trains them by connecting unity to a python server. 
To setup it, follow the instructions of MLAgent instalation for the unity's `1.9.0` version.
Then, run it via the MLAgents CLI.

#### Inference

In this mode, you can run my pre-trainned brain, or add yours into the `NN-Brain` section of the `Behaviour Parameters` group.

#### Heuristic

To test it, you can run it in the Heuristic mode, where you will be able to test the agent with the input arrows or WASD.

## How it works

Each Agent starts before the StartLine, and for each checkpoint that it reaches, it will get a point. 
If the Agent hit a wall, it will instantly be stopped and will not be able to collect more points.
The Agents is able to see the walls in 180 degrees and 7 directions.

## Modifying

You can add more agents if you want by adding the `Car` prefab, or copying the `Car` instance. The race will only reset when all cars finish their episodes.
You can also change the environment and the race, but be careful about the checkpoints, the spawnpoint, and the walls.
Each Agent has a `RayPerception3DSensor`, that can will detect the checkpoints as a wall if you don't be careful, and it will need a `BoxCollider` at the road sides to work.
The Walls also need to be tagged as `Wall`.

