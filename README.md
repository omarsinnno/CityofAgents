# City of Agents

This documentation walks through the process of applying the controller-agent framework in an agent collaborative setting and training the agents in the environment. After reading this tutorial, you should be able to adopt the framework in any fitted environment. Specifically, we're going to present an example so that this tutorial flows naturally.

![Controller-Agent Framework Diagram](https://github.com/omarsinnno/CityofAgents/blob/main/draw.png?raw=true)

# Installing the environment

Note: You do not need to have this specific environment to follow the next instructions, so if you have a preferred environment you would like to use, you may skip the environment installation part.

The example environment used in this documentation is the "Restaurant 02" environment purchased from the asset store. [Check here](https://assetstore.unity.com/packages/3d/environments/restaurant-02-154333) to purchase. After purchasing, you import the asset from the Unity editor through the Unity Package Manager. Latest useful guide, at this time, for importing your packages from the asset store using the package manager can be found [here](https://docs.unity3d.com/Manual/upm-ui-import.html).

# Installing Unity ML-Agents

Note: If you have the latest ML-Agents package installed on your machine, you may skip the ML-Agents installation part. It is important that the ML-Agents installation to be the latest releases to avoid issues later on during the project. At the time of writing this, the latest release is Release 16 which introduced the MA-POCA algorithm for the training of multiple collaborative or competitive agents in Unity.

To install Unity ML-Agents, please refer to the official installation menu by the Unity ML-Agents team [here](https://github.com/Unity-Technologies/ml-agents/tree/release_16_docs). 

# Setting up the controller and the agents

The task presented in the Restaurant environment is simple, and will be used as a demo that can be futher developed. The controller-agent framework is linked between a single controller and two agents: the kitchen agent and the waiter agent. The Kitchen agent refers to the agent that brings food from the kitchen, and the Waiter agent refers to the agent that delivers food to the specific table.

The controller is a game object created with no added specification. Meaning that it does not have any visual or tangible shape. It is the parent of a group of agents that will complete tasks in the environment collaboratively. When the agent script is attached to the controller, it is important to check the inherit child sensors in the Unity inspector, so that the observations collected by the controller's children are exploited by the controller.

![Control Components](https://github.com/omarsinnno/CityofAgents/blob/main/Control.png?raw=true)

The agents have a rigidbody and a collider. The rigidbody will be used for the motion of the agents and the collider will be used to detect collisions and or triggers between obstacles. The agents will be the main actors in the environment that collect observations and perform the actions according to script. Each agent has a ray perception sensor 3D with: a specified length, a detection sphere at the end of the ray with a specified sphere radius, X and Y offsets to detect where the ray sensor should be positioned on the agent, number of rays that specify how many rays should stem from the agent, and finally the ray angle that specifies at which angles the rays are located.

![Waiter Components](https://github.com/omarsinnno/CityofAgents/blob/main/Waiter.png?raw=true)

The obstacles (such as the walls, NPCs in the environment, game objects, etc.) enforce the realistic motion of the agents in the environment. This is important so that our agent learns not to collide with people and objects while going to target: a real agent should maintain the safety of the people around it so it must not collide with them, a real agent should not break itself by colliding with walls, benches, chairs, etc. These obstacles have tags that allow for the agent to detect them using the ray perception sensor.

# Setting up the scripts

In the [15th release](https://github.com/Unity-Technologies/ml-agents/tree/release_16_docs) of Unity ML-Agents, a new multi-agent deep reinforcement learning algorithm was introduced: MultiAgent POst Humous Credit Assignment (MA-POCA), that prompted the branching of another way to set up the controller-agent framework, both frameworks are valid.

## First Setup
In the first setup there are multiple scripts: the controller script that derives from the Agent base class, and the agent script(s). There may be multiple scripts if the agents have very different tasks; example: the waiter agent should not go to the kitchen and bring the food, this is the kitchen agent's role, the kitchen agent passes the food to the waiter agent who then delivers the food to the table.

The controller script contains methods that define the reinforcement learning methods. It defines ML-Agents methods such as the Initialize, OnEpisodeBegin, OnActionReceived, CollectObservations, and the Heuristic for debugging. In the Initialize, the speeds of the different agents are initialized, the state of the controller and agents is initialized, the agent game objects are initialized, and the kitchen and agent scripts are linked here, too as follows:
```C#
public override void Initialize()
{
    // Initializes the speed of the agents
 		wspeed = 2f;
 		kspeed = 2f;
    
    // Initializes the state of the controller and the agents
 		counter = 0;

    // Initializes the target of the waiter
    // At the beginning, the waiter and kitchen have to meet to pass the food from
    // Kitchen agent to waiter agent, so they have the same "target location".
 		waitertarget = kitchentarget;

    // This gets the kitchen script and waiter script from the children of the controller
    // (Will be explained in more detail later)
 		Kscript = kitchen.GetComponent<KitchenScript>();
 		Wscript = waiter.GetComponent<WaiterScript>();
    
 		<...>
}
```
In the OnEpisodeBegin method, we define what happens at the beginning of each RL episode. At the beginning of each episode, the speed is reset, the state is reset, the interactions between the agents and the obstacles or the target are reset, the position of the agents are reset, and finally a random table is selected as target to randomize training.
```C#
public override void OnEpisodeBegin()
{
 	// Reset speed, state, and target
  kspeed= 3;
 	counter = 0;
 	<...>
 	waitertarget = kitchentarget;

  // Reset interactions between agents and objects
 	Kscript.SetTargetReached(false);
 	Kscript.SetWallHit(false);
  Wscript.SetTargetReached(false);
 	Wscript.SetWallHit(false);
 	Wscript.SetTargetSwitch(false);
 	Wscript.SetEntryHit(false);

 	// Reset agents positions
  waiter.transform.localPosition = <...>
 	kitchen.transform.localPosition = <...>
 	waiter.transform.rotation = <...>
 	kitchen.transform.rotation = <...>

  // Randomize target
  int rnd_pos = 4;
  switch (rnd_pos)
  {
    <...>
  }
}
```
The OnCollectObservations Method can be neglected since the ray perception sensor is selected. However, to improve training, we preferred to include this method and include in it the agents state and the agent and target positions. This allows for the agent to observe where the target is, and know in what state the agent is.
```C#
public override void CollectObservations(VectorSensor sensor)
{
  sensor.AddObservation(waiter.transform.localPosition);
  sensor.AddObservation(kitchen.transform.localPosition);
  sensor.AddObservation(target.transform.localPosition);
  sensor.AddObservation(kitchentarget.transform.localPosition);
  sensor.AddOneHotObservation( (int)counter , 3);
}
```
The OnActionReceived method is crucial, it is the method in which the reward scheme is described and is where the core of framework functionality is located. We define the actions first and assign aciton signals (moveWaiter) and (moveKitchen) to control the movements of the agents. A small negative reward is given at every step, to make sure the agent learns an optimal path to its target, or less formally, it learns how to hurry up. Then we define the reward scheme in every possible state the agents could be in, we keep track of the states using a counter. So in the code counter==state.
```C#
public override void OnActionReceived(ActionBuffers actions)
{
	// Assigns the velocity signal / Force that moves the agents
	// A signal is assigned to each agent
	Vector3 moveWaiter = new Vector3();
	Vector3 moveKitchen = new Vector3();
	
	// Assigns each signal to continuous actions that move
	// Each agent in the X and Z directions (Vertically and Horizontally)
	moveWaiter.x = actions.ContinuousActions[0];
	moveWaiter.z = actions.ContinuousActions[1];
	moveKitchen.x = actions.ContinuousActions[2];
	moveKitchen.z = actions.ContinuousActions[3];
	
	// Changes the local position of the agents
	waiter.transform.localPosition += (moveWaiter * Time.deltaTime * wspeed); 
	kitchen.transform.localPosition += (moveKitchen * Time.deltaTime * kspeed);
	
	// MaxStep: maximum number of steps allowed in the episode
	// Gives a penalty to the agents on each step in the episode
	// This promotes finding the optimal path toward the target
	AddReward(-1/MaxStep);
	
	<...>
	// If Kitchen Agent reaches target
	if (Kscript.GetTargetReached() == true)
	{
		<...> Specify what happens when the Kitchen Agent reaches target (meeting point with waiter agent)
		// We move from state (counter) 0 to state 1
		counter = 1
	}

	// If Kitchen Agent has not reached Waiter Agent yet
	if (counter == 0)
	{
		<...>
	}

	// Kitchen reaches its target
	if(Kscript.Threshold(kitchen.transform.localPosition, kitchentarget.transform.localPosition, 1.3f))
    	{	
		<...>
		AddReward(1f);
	}

		
	// Waiter reaches kitchen agent
	if(Wscript.Threshold(waiter.transform.localPosition, waitertarget.transform.localPosition, 1f) && waitertarget.tag == "GoalK")
    	{
		AddReward(0.2f);
		waitertarget = target; // Now the waiter can go deliver the food	
    	}

	// Waiter delivers order
	if(Wscript.Threshold(waiter.transform.localPosition, waitertarget.transform.localPosition, 1f) && waitertarget.tag == "Goal")
	{		
		AddReward(2f);	
		EndEpisode();
	}

	// If Waiter or Agent hit an obstacle, end the episode
	if (Kscript.GetWallHit() == true || Wscript.GetWallHit() == true)
    	{
		AddReward(-1f);
		EndEpisode();
	}
	if (Wscript.GetEntryHit() == true)
	{
		AddReward(-1f);
		EndEpisode();
	}
}
```
Furthermore, a heuristic code is implemented.

## Second Setup
In the second setup there are two main scripts. The controller has a controller script that derives from the MonoBehaviour base class not the Agent base class. The idea is to assign the same script to each agent, unlike the first setup where each agent has his own script. This leads to more efficient work that requires less space, since multiple agents will be accessing the same script.

We have not yet included in this repository an example that uses the second setup. If you're interested in implementing a collaborative environment that uses the MA-POCA algorithm, I invite you to refer to [this example from the Unity ML-Agents team](https://github.com/Unity-Technologies/ml-agents/tree/e4e9c51bf1615953759249cc8ad349534d2569f4/Project/Assets/ML-Agents/Examples/DungeonEscape/Scripts). It's their implementation of the DungeonEscape game using the MA-POCA, the framework in which it is implemented is very close to ours.

# Training

# Tensorboard 
