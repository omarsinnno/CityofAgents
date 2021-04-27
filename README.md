# City of Agents

This documentation walks through the process of applying the controller-agent framework in an agent collaborative setting and training the agents in the environment. After reading this tutorial, you should be able to adopt the framework in any fitted environment. Specifically, we're going to present an example so that this tutorial flows naturally.

![Controller-Agent Framework Diagram](https://github.com/omarsinnno/CityofAgents/blob/main/draw.png?raw=true)

# Installing the environment

Note: You do not need to have this specific environment to follow the next instructions, so if you have a preferred environment you would like to use, you may skip the environment installation part.

The example environment used in this documentation is the "Restaurant 02" environment purchased from the asset store. ![Check here](https://assetstore.unity.com/packages/3d/environments/restaurant-02-154333) to purchase. After purchasing, you import the asset from the Unity editor through the Unity Package Manager. Latest useful guide, at this time, for importing your packages from the asset store using the package manager can be found ![here](https://docs.unity3d.com/Manual/upm-ui-import.html).

# Installing Unity ML-Agents

Note: If you have the latest ML-Agents package installed on your machine, you may skip the ML-Agents installation part. It is important that the ML-Agents installation to be the latest releases to avoid issues later on during the project.

To install Unity ML-Agents, please refer to the official installation menu by the Unity ML-Agents team ![here](https://github.com/Unity-Technologies/ml-agents/tree/release_16_docs). At the time of writing this, the latest release is Release 16 which introduced the MA-POCA algorithm for the training of multiple collaborative or competitive agents in Unity.

