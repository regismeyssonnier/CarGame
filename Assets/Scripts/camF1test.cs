using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camF1test : MonoBehaviour
{
    public Camera CamBack;
    public Camera CamFirstPerson;
    float time_cam = 0f;
    bool bcam = true;
    // Start is called before the first frame update
    void Start()
    {
        CamBack.enabled = false;
        CamFirstPerson.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*time_cam += Time.deltaTime;
        if(time_cam > 3.0f)
        {
            if (bcam)
            {
                CamFirstPerson.enabled = true;
                CamBack.enabled = false;
            }
            else
            {
                CamFirstPerson.enabled = false;
                CamBack.enabled = true;
            }

            bcam = !bcam;
            time_cam = 0.0f;
            
        }*/
        
    }
}
