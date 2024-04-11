using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class AI_Demo : Agent
{

    public Transform[] lap_check;
    public int ind_check = 1;

    private PlayerF90 playerAI;

    bool start = true;

    int count0 = 0;

    float reward = 0.0f;

    private void Awake()
    {
        playerAI = GetComponent<PlayerF90>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int actacc = 0;
        if (playerAI.brakePlayer) actacc = 1;

        int actturn = 1;
        if (playerAI.mmove.x < 0) actturn = 0;
        else if (playerAI.mmove.x > 0) actturn = 2;

        //Debug.Log("acc " + actacc + " , turn " + actturn);

        ActionSegment<int> discreteA = actionsOut.DiscreteActions;
        discreteA[0] = actacc;
        discreteA[1] = actturn;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.up);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(lap_check[ind_check].position - transform.position);
        sensor.AddObservation(lap_check[ind_check].position);
        sensor.AddObservation(playerAI.rb.velocity.magnitude * 3.6f);

        //AddReward(-0.001f);

    }

    private float getCol()
    {
        Vector3 speed = playerAI.rb.velocity;

        //col = ((self.sim.speed.x * (checkpoint[self.sim.check_point].x - self.sim.pos.x) + self.sim.speed.y * (checkpoint[self.sim.check_point].y - self.sim.pos.y)) /
        //       (math.sqrt(self.sim.speed.x * self.sim.speed.x + self.sim.speed.y * self.sim.speed.y) *
        //        math.sqrt((checkpoint[self.sim.check_point].x - self.sim.pos.x) * (checkpoint[self.sim.check_point].x - self.sim.pos.x) +
        //                  (checkpoint[self.sim.check_point].y - self.sim.pos.y) * (checkpoint[self.sim.check_point].y - self.sim.pos.y)) + 0.000001))

        float col = ((speed.x * (lap_check[ind_check].position.x - transform.position.x) + speed.z * (lap_check[ind_check].position.z - transform.position.z)) /
               ((float)Mathf.Sqrt(speed.x * speed.x + speed.z * speed.z) *
                (float)Mathf.Sqrt((lap_check[ind_check].position.x - transform.position.x) * (lap_check[ind_check].position.x - transform.position.x) +
                          (lap_check[ind_check].position.z - transform.position.z) * (lap_check[ind_check].position.z - transform.position.z)) + 0.000001f));


        return col;


    }


    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        playerAI.UpdateCarInfo(actionBuffers.DiscreteActions[0], actionBuffers.DiscreteActions[1]);
        //Debug.Log("acc " + actionBuffers.DiscreteActions[0] + ", turn " + actionBuffers.DiscreteActions[1]);

        double dist = Vector3.Distance(transform.position, lap_check[ind_check].position);
        //Debug.Log("distance : " + dist.ToString());
        dist = ((2000.0 - dist) / 2000.0);

        float rew = 0.0f;
        //if (actionBuffers.DiscreteActions[0] == 1) rew = 0.1f;

        float speed = playerAI.rb.velocity.magnitude * 3.6f;
        if (speed < 5.0f && !start && count0 > 33)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        if (speed < 5.0f && !start)
        {
            count0++;
        }

        if (speed > 5.0f)
        {
            count0 = 0;
            start = false;

        }

        //speed /= 1000.0f;

        float col = getCol();

        if (actionBuffers.DiscreteActions[0] == 1 && speed <= 100.0f)
        {
            //Debug.Log("Don't do Dicks " + speed);
            reward = -1.0f;
            SetReward(-1.0f);
        }
        else if (speed < 20.0f && actionBuffers.DiscreteActions[0] != 1)
        {
            //Debug.Log("Fucked up " + speed);
            reward = -1.0f;
            SetReward(-1.0f);
        }
        else if (speed < 50.0f && !start)
        {
            reward = -1.0f;
            SetReward(-1.0f);
        }
        else
        {
            //Debug.Log("speed : " + speed.ToString());
            //float totrew = (float)dist * 0.1f;
            // Debug.Log("reward : " + totrew.ToString());

            if (speed > 150.0f)
                reward += 0.1f;
            if (col >= 0.75)
            {
                reward += 0.1f;
            }
            else if (col <= 0.6)
            {
                if (actionBuffers.DiscreteActions[0] != 1)
                {
                    reward += 0.05f;
                }
            }

            if (reward > 1.0f) reward = 1.0f;
            if (reward < -1.0f) reward = -1.0f;
            SetReward(reward);

        }



        /*else
        {
            reward -= 0.2f;
        }*/

        if (transform.position.y < 2.0f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }



        /*if (actionBuffers.ContinuousActions[0] <= 0.0f) 
        {
            SetReward(-1.0f);
         
        }*/


        //Debug.Log("reward "  + reward);


        reward = 0.0f;

    }

    public override void OnEpisodeBegin()
    {

        int indchk = ind_check;
        //if (indchk < 0) indchk = lap_check.Length - 1;
        transform.position = lap_check[indchk].position;
        Vector3 sub = new Vector3(0.0f, 3.0f, 0.0f);
        transform.position += sub;

        playerAI.rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        //int indchk2 = ind_check - 1; 
        //if (indchk2 < 0) indchk2 = lap_check.Length - 1;
        playerAI.uprepos = lap_check[indchk].transform.up;
        playerAI.frepos = lap_check[indchk].transform.forward;
        playerAI.Reset();
        start = true;
        count0 = 0;
        reward = 0.0f;
        //ind_check = (ind_check + 1) % lap_check.Length;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("checkpoint") && other.gameObject.transform == lap_check[ind_check])
        {
            reward += 1.0f;
            if (reward > 1.0f) reward = 1.0f;
            SetReward(1.0f);
            //Debug.Log("Pass checkpoint!!!!!!!!!!!!!" + ind_check);
            //Debug.Log(ind_check);
            ind_check = (ind_check + 1) % lap_check.Length;
        }
        else if (other.CompareTag("checkpoint") && other.gameObject.transform != lap_check[ind_check])
        {
            SetReward(-1.0f);
            //Debug.Log("Bad checkpoint!!!!!!!!!!!!!");
            EndEpisode();
        }
        else if (other.CompareTag("Bonus_Droite"))
        {
            reward = 1.0f;
            //Debug.Log("bonus");
            SetReward(1.0f);


        }
        else if (other.CompareTag("Wall"))
        {
            //Debug.Log("contact");
            reward = -1.0f;
            SetReward(-1.0f);
            //EndEpisode();

        }
        else if (other.CompareTag("BPenalty"))
        {
            //Debug.Log("Penalty Bordure");
            reward = -1.0f;
            SetReward(-1.0f);

        }
        else if (other.CompareTag("circuit"))
        {
            //Debug.Log("contact");
            reward = -1.0f;
            SetReward(-1.0f);
           
        }

        

        

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bonus_Droite"))
        {
            reward = 1.0f;
            Debug.Log("bonus");
            SetReward(1.0f);


        }
        else if (other.CompareTag("BPenalty"))
        {
            //Debug.Log("Penalty Bordure");
            reward = -1.0f;
            SetReward(-1.0f);

        }
        else if (other.CompareTag("Wall"))
        {
            reward = -1.0f;
            //Debug.Log("contact");
            SetReward(-1.0f);
            //EndEpisode();

        }
        else if (other.CompareTag("circuit"))
        {
            //Debug.Log("contact");
            reward = -1.0f;
            SetReward(-1.0f);
            EndEpisode();
        }

        


    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.gameObject.tag == "circuit")
        {
            reward = -1.0f;
            //Debug.Log("circuit");
            SetReward(-1.0f);
            //EndEpisode();

        }





    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.gameObject.tag == "circuit")
        {
            reward = -1.0f;
            //Debug.Log("circuit");
            SetReward(-1.0f);
            //EndEpisode();

        }





    }




}
