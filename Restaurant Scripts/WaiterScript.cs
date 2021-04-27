using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterScript : MonoBehaviour
{
    // Start is called before the first frame update

    private bool targetReached;
    private bool wallHit;
    private bool entryHit;
    private bool targetSwitch;


    void Start(){

    	targetReached = false;
    	wallHit = false; 
        targetSwitch = false;
    }

    public bool Threshold (Vector3 agent , Vector3 target, float thresh){

        float distanceToTarget = Vector3.Distance(agent, target);

      

        if (distanceToTarget < thresh){

            return true;
        }

        return false;

    }



  	 private void OnTriggerEnter(Collider other){
    
    	/*if(other.gameObject.tag =="GoalK"){

    	targetSwitch = true;
        
        

    	
    }
*/
      /*  if(other.gameObject.tag =="GoalW"){

        targetReached = true;  
        
    }
*/

        if (other.gameObject.tag =="Wall"){

        wallHit = true;

    }

         if (other.gameObject.tag =="noentry"){

        entryHit = true;

    }

        

    }

    public bool GetTargetReached(){

    	return targetReached;


    }

    public bool GetWallHit(){

    	return wallHit;


    }

    public bool GetTargetSwitch(){

        return targetSwitch;


    }


    public bool GetEntryHit(){

        return entryHit;


    }

     public void SetWallHit(bool W){

    	wallHit = W;


    }

    public void SetTargetReached(bool T){

    	targetReached = T;


    }


    public void SetTargetSwitch(bool S){

        targetSwitch = S;


    }

     public void SetEntryHit(bool E){

        entryHit = E;


    }

    
}

