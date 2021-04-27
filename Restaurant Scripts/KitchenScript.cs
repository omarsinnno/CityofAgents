using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenScript : MonoBehaviour
{
    // Start is called before the first frame update

    private bool targetReached;
    private bool wallHit;


    void Start(){

    	targetReached = false;
    	wallHit = false; 
    }

  	 private void OnTriggerStay(Collider other){
    
    	if(other.gameObject.tag =="GoalK"){

    	targetReached = true;

    	
    	
    }
        if (other.gameObject.tag =="Wall"){

        wallHit = true;

    }



    }


    
    public bool Threshold (Vector3 agent , Vector3 target, float thresh){

        float distanceToTarget = Vector3.Distance(agent, target);

       

        if (distanceToTarget < thresh){

            return true;
        }

        return false;


    }

    public bool GetTargetReached(){

    	return targetReached;


    }

    public bool GetWallHit(){

    	return wallHit;


    }

     public void SetWallHit(bool W){

    	wallHit = W;


    }

    public void SetTargetReached(bool T){

    	targetReached = T;


    }

    
}

