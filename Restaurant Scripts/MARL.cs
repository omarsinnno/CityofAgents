using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MARL : Agent
{
	[SerializeField] private bool kitchenAchieved; //When kitchen arrives to counter
	[SerializeField] private bool waiterAchieved; // When waiter arrives to counter
	[SerializeField] private Transform waitertarget; 
	[SerializeField] public Transform kitchentarget;
	[SerializeField] public Transform target;
 	[SerializeField] private float wspeed;
 	[SerializeField] private float kspeed;
 	[SerializeField] public GameObject waiter;
 	[SerializeField] public GameObject kitchen;
 	private KitchenScript Kscript;
 	private WaiterScript Wscript;
 	private int counter;
 	private GameObject foodK;
 	private GameObject foodW;




 	public override void Initialize(){

 		wspeed = 2f;
 		kspeed =2f;
 		//kitchenAchieved= false;
 		//waiterAchieved = false;
 		counter = 0;

 		waitertarget = kitchentarget;

 		Kscript = kitchen.GetComponent<KitchenScript>();
 		Wscript = waiter.GetComponent<WaiterScript>();
 		foodK = this.gameObject.transform.GetChild(1).transform.GetChild(0).gameObject;
 		foodW = this.gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;

 		foodK.SetActive(true);
		foodW.SetActive(false);

 	}

 	public override void OnEpisodeBegin(){

 	kspeed= 3;
 	counter = 0;
 	foodK.SetActive(true);
	foodW.SetActive(false);
 

 	
 	

 	waitertarget = kitchentarget;

 	Kscript.SetTargetReached(false);
 	Kscript.SetWallHit(false);

 	Wscript.SetTargetReached(false);
 	Wscript.SetWallHit(false);
 	Wscript.SetTargetSwitch(false);
 	Wscript.SetEntryHit(false);


 	waiter.transform.localPosition = new Vector3( 9.59f, 0, -1.97f);
 	kitchen.transform.localPosition = new Vector3( 7f, 0, -12f);

 	waiter.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f));
 	kitchen.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f));




 	//int rnd_pos = Random.Range(0, 3);
 	int rnd_pos = 4;

    switch (rnd_pos)
        {
            case 1:
                target.localPosition = new Vector3( 3.76f, 0f, 1.55f);
                break;
            case 2:
                target.localPosition = new Vector3( -1.2f, 0f, 1.55f);
                break;
            case 3:
                target.localPosition = new Vector3( -3.9f, 0f, 1.55f);
                break;
            case 4:
                target.localPosition = new Vector3( -5.9f, 0f, 1.55f);
                break;
            case 5:
                target.localPosition = new Vector3( -8.5f, 0.4f, 1.55f);
                break;
            case 6:
                target.localPosition = new Vector3( -8.5f, 0.4f, -0.17f);
                break;
            case 7:
                target.localPosition = new Vector3( -6.5f, 0.4f, -0.17f);
                break;
        }





 	}



 	  public override void CollectObservations(VectorSensor sensor)

    {
    	sensor.AddObservation(waiter.transform.localPosition);
    	sensor.AddObservation(kitchen.transform.localPosition);
		sensor.AddObservation(target.transform.localPosition);
		sensor.AddObservation(kitchentarget.transform.localPosition);
		sensor.AddOneHotObservation( (int)counter , 3);
		



 	}

 		public override void OnActionReceived(ActionBuffers actions)
	{
		// Speed up velocity
		Vector3 moveWaiter = new Vector3();
		Vector3 moveKitchen = new Vector3();



		moveWaiter.x = actions.ContinuousActions[0];
		moveWaiter.z = actions.ContinuousActions[1];

		moveKitchen.x = actions.ContinuousActions[2];
		moveKitchen.z = actions.ContinuousActions[3];

		// ** Create movement
		waiter.transform.localPosition += (moveWaiter * Time.deltaTime * wspeed); 
		kitchen.transform.localPosition += (moveKitchen * Time.deltaTime * kspeed);



		AddReward(-1/MaxStep);

		//Wscript.SetTargetReached(Wscript.Threshold(waiter.transform.position , waitertarget.transform.position , 0.4f));

		if (kitchen.transform.localPosition.y < -2 || waiter.transform.localPosition.y <-2 ){

			EndEpisode();
		}

		if (Kscript.GetTargetReached()== true){


			counter = 1;
			Kscript.SetTargetReached(false);
			kspeed = 0;
		}


		if (counter == 0){

			waiter.transform.localPosition = new Vector3( 9.59f, 0, -1.97f);

		}

		// If either agent reaches the first target (meeting point)
		if (counter ==1 ){ 

			kitchen.transform.localPosition = new Vector3( 7f, 0, -12f);
			
		}


		// Kitchen reaches its target
		if(Kscript.Threshold(kitchen.transform.localPosition, kitchentarget.transform.localPosition, 1.3f)){

			
			foodK.SetActive(false);
			foodW.SetActive(true);
			
			kitchen.transform.localPosition = new Vector3( 7f, 0, -12f);
			Kscript.SetTargetReached(true);
			AddReward(1f);
			



		}

		
		// Waiter meetins with the kitchen agent
		if(Wscript.Threshold(waiter.transform.localPosition, waitertarget.transform.localPosition, 1f) && waitertarget.tag == "GoalK"){

				AddReward(0.2f);
				waitertarget = target;
				//Debug.Log("Still here");
				

			}

		// Waiter delivers order
		if(Wscript.Threshold(waiter.transform.localPosition, waitertarget.transform.localPosition, 1f) && waitertarget.tag == "Goal"){

				
				AddReward(2f);	
				EndEpisode();

		}

		// Agents hit a wall
		if (Kscript.GetWallHit() == true || Wscript.GetWallHit() == true){

			AddReward(-1f);
			EndEpisode();

		}

		

		

		// Hit obstacle
		if (Wscript.GetEntryHit() == true){

			AddReward(-1f);
			EndEpisode();
		}

		// Hit obstacle
	



	}

		public override void Heuristic(in ActionBuffers actionsOut)
	{

		ActionSegment<float> continousActions = actionsOut.ContinuousActions;

		// Move Agent
		continousActions[0] = Input.GetAxis("Horizontal");
		continousActions[1] = Input.GetAxis("Vertical");

		// Move Samir
		continousActions[2] = Input.GetAxis("Horizontal_Secondary");
		continousActions[3] = Input.GetAxis("Vertical_Secondary");
	}


}
