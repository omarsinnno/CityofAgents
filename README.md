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

The controller is a game object created with no added specification. Meaning that it does not have any visual or tangible shape. It is the parent of a group of agents that will complete tasks in the environment collaboratively.

The agents have a rigidbody and a collider. The rigidbody will be used for the motion of the agents and the collider will be used to detect collisions and or triggers between obstacles. The agents will be the main actors in the environment that collect observations and perform the actions according to script. Each agent has a ray perception sensor 3D with: a specified length, a detection sphere at the end of the ray with a specified sphere radius, X and Y offsets to detect where the ray sensor should be positioned on the agent, number of rays that specify how many rays should stem from the agent, and finally the ray angle that specifies at which angles the rays are located.

The obstacles (such as the walls, NPCs in the environment, game objects, etc.) enforce the realistic motion of the agents in the environment. This is important so that our agent learns not to collide with people and objects while going to target: a real agent should maintain the safety of the people around it so it must not collide with them, a real agent should not break itself by colliding with walls, benches, chairs, etc. These obstacles have tags that allow for the agent to detect them using the ray perception sensor.

# Setting up the scripts
