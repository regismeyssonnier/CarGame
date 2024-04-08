using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCheck : MonoBehaviour
{
    public Player player;
    public PlayerF90 playerf90;
    public int id;

    bool valid_lap = false;
    bool start = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

   
    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {
            
            if (player.goal == id || player.goal == 6)
            {
                player.goal++;
                Debug.Log("goal " + player.goal);

                if (player.goal == 7 && id == 0)
                {
                    player.goal = 1;
                    valid_lap = true;

                }
            }

            if (id == 0 && valid_lap && !start)
            {
                if (player.time_lap < player.best_time_lap)
                {
                    player.best_time_lap = player.time_lap;
                }

                player.time_lap = 0f;
                valid_lap = false;

            }

            if (id == 0 && start)
            {
                player.time_lap = 0f;
            }
            start = false;

        }
        else if (other.gameObject.tag == "PlayerF90")
        {
            Debug.Log("goal " + id);
            if (playerf90.goal == id || playerf90.goal == 6)
            {
                playerf90.goal++;
                Debug.Log("goalf90 " + playerf90.goal);

                if (playerf90.goal == 7 && id == 0)
                {
                    playerf90.goal = 1;
                    valid_lap = true;

                }
            }

            if (id == 0 && valid_lap && !start)
            {
                if (playerf90.time_lap < playerf90.best_time_lap)
                {
                    playerf90.best_time_lap = playerf90.time_lap;
                }

                playerf90.time_lap = 0f;
                valid_lap = false;

            }

            if (id == 0 && start)
            {
                playerf90.time_lap = 0f;
            }
            start = false;

        }



    }


}