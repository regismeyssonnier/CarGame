using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class AI_F_Behavior : Agent
{
    
    public Transform[] lap_check;
    public int ind_check = 1;

    private PlayerAI playerAI;

    bool start = true;

    int count0 = 0;

    private void Awake()
    {
        playerAI = GetComponent<PlayerAI>();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.up);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(lap_check[ind_check].position - transform.position);
        sensor.AddObservation(lap_check[ind_check].position);

        //AddReward(-0.001f);

    }


    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        playerAI.UpdateCarInfo(actionBuffers.DiscreteActions[0], actionBuffers.DiscreteActions[1]);
        Debug.Log("acc " + actionBuffers.DiscreteActions[0] + ", turn " + actionBuffers.DiscreteActions[1]);

        
        
        double dist = Vector3.Distance(transform.position, lap_check[ind_check].position);
        //Debug.Log("distance : " + dist.ToString());
        dist = ((2000.0 - dist) / 2000.0);

        float rew = 0.0f;
        //if (actionBuffers.DiscreteActions[0] == 1) rew = 0.1f;
       


        float speed = playerAI.rb.velocity.magnitude * 3.6f;
        if (speed < 5.0f && !start && count0 > 10)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        if(speed < 5.0f && !start)
        {
            count0++;
        }

        if(speed > 5.0f)
        {
            count0 = 0;
            start = false;
            
        }
        //speed /= 1000.0f;

        //Debug.Log("speed : " + speed.ToString());
        float totrew = (float)dist * 0.1f ;
        // Debug.Log("reward : " + totrew.ToString());
        SetReward(totrew);

        if(transform.position.y < 2.0f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        /*if (actionBuffers.ContinuousActions[0] <= 0.0f) 
        {
            SetReward(-1.0f);
         
        }*/

        //Debug.Log(GetCumulativeReward());


    }

    public override void OnEpisodeBegin()
    {
        int indchk = 0;// ind_check-1;
        ///if (indchk < 0) indchk = lap_check.Length - 1;
        transform.position = lap_check[indchk].position;
        //Vector3 sub = new Vector3(0.0f, 0.0f, 20.0f);
        //transform.position -= sub;
        playerAI.Reset();
        playerAI.uprepos = lap_check[indchk].transform.up;
        playerAI.frepos = lap_check[indchk].transform.forward;
        start = true;
        count0 = 0;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("checkpoint") && other.gameObject.transform == lap_check[ind_check])
        {
            
            SetReward(1.0f);
            Debug.Log("Pass checkpoint!!!!!!!!!!!!!");
            Debug.Log(ind_check);
            ind_check = (ind_check + 1) % lap_check.Length;
        }

        /*if (other.CompareTag("Wall"))
        {
            //Debug.Log("contact");
            SetReward(-1.0f);
            //EndEpisode();

        }*/

        if (other.CompareTag("circuit"))
        {
            //Debug.Log("contact");
            SetReward(-1.0f);
            EndEpisode();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.CompareTag("Wall"))
        {
            //Debug.Log("contact");
            SetReward(-1.0f);
            EndEpisode();

        }

        if (other.CompareTag("circuit"))
        {
            //Debug.Log("contact");
            SetReward(-1.0f);
            EndEpisode();
        }*/

    }

   

}

