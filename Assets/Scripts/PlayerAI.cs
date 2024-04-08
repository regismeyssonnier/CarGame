using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    //public Camera CamBack;
    //public Camera CamFirstPerson;
    //PlayerController manette;
    bool accelPlayer = false;
    bool brakePlayer = false;
    bool r = false;
    Vector2 mmove;

    float accelSensibility = 15000.0f;
    float brakeSensibility = 100000.0f;
    float turnSensibility = 10.0f;

    float accel = 0.0f;
    float turn = 0.0f;

    float speed = 0.0f;

    float coefAccel = 10.0f;

    float currentAccel = 0.0f;
    float currentBrake = 0.0f;
    float currentTurnAngle = 0.0f;

    public WheelCollider wcfr;
    public WheelCollider wcfl;
    public WheelCollider wcbr;
    public WheelCollider wcbl;

    public Transform tfr;
    public Transform tfl;
    public Transform tbr;
    public Transform tbl;

    Quaternion rotationDeltal, rotationDeltar;

    float wheelfr_in_air = 0f;
    float wheelfl_in_air = 0f;
    float wheelbr_in_air = 0f;
    float wheelbl_in_air = 0f;

    Vector3 last_point_contact;
    bool reposition = false;
    Quaternion qrepos;
    public Vector3 uprepos, frepos;

    public Rigidbody rb;

    //public TextMeshProUGUI speed_screen;

    NavMeshAgent agent;
      
    public Transform[] goals;
    int ind_goal = 0;

    void Awake()
    {
        
    }

   

    // Start is called before the first frame update
    void Start()
    {
        //CamBack.enabled = false;
        //CamFirstPerson.enabled = false;

        //agent = GetComponent<NavMeshAgent>();
        //agent.destination = goals[ind_goal].position;

        rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = new Vector3(0f, 0f, 0f);
        rotationDeltar = Quaternion.FromToRotation(wcfr.transform.forward, tfr.forward);
        rotationDeltal = Quaternion.FromToRotation(wcfl.transform.forward, tfl.forward);
        //uprepos = wcfr.transform.up;
    }

    public void UpdateCarInfo(int acc, int tu)
    {
        int[] corrt = { -1, 0, 1 };
        accel = acc * accelSensibility;
        turn = corrt[tu];
        
    }

    public void Reset()
    {
        qrepos = Quaternion.FromToRotation(transform.up, uprepos);
        Quaternion fqrepos = Quaternion.FromToRotation(transform.forward, frepos);
        transform.localRotation *= qrepos;
        transform.localRotation *= fqrepos;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wcbr.motorTorque = accel * coefAccel;
        wcbl.motorTorque = accel * coefAccel;

        currentTurnAngle = turnSensibility * turn;
        wcfr.steerAngle = currentTurnAngle;
        wcfl.steerAngle = currentTurnAngle;

        //Debug.Log(wcbr.motorTorque + ",  " + wcfl.steerAngle);

        /*if (accelPlayer)
        {
            if (r)
            {
                currentAccel = -accelSensibility;
            }
            else
            {
                r = false;
                currentAccel = accelSensibility;
            }

            currentBrake = 0.0f;
        }
        else
        {
            currentAccel = 0.0f;
            currentBrake = brakeSensibility * 0.25f;
        }

        if (brakePlayer)
        {
            currentBrake = brakeSensibility;

        }

        wcfr.brakeTorque = currentBrake * coefAccel;
        wcfl.brakeTorque = currentBrake * coefAccel;
        wcbr.brakeTorque = currentBrake * coefAccel;
        wcbl.brakeTorque = currentBrake * coefAccel;

        wcbr.motorTorque = currentAccel * coefAccel;
        wcbl.motorTorque = currentAccel * coefAccel;

        Debug.Log(currentAccel);

        speed = rb.velocity.magnitude * 3.6f;
        if (speed < 50)
        {
            turnSensibility = 45.0f;
        }
        else if (speed < 100)
        {
            turnSensibility = 10.0f;
        }
        else if (speed < 200)
        {
            turnSensibility = 5.0f;
        }
        else if (speed < 300)
        {
            turnSensibility = 2.5f;
        }
        else
        {
            turnSensibility = 2.0f;
        }



        currentTurnAngle = turnSensibility * mmove.x;
        wcfr.steerAngle = currentTurnAngle;
        wcfl.steerAngle = currentTurnAngle;
        
      

        UpdateWheel(wcfr, tfr, rotationDeltar, currentTurnAngle, true);
        UpdateWheel(wcfl, tfl, rotationDeltal, currentTurnAngle, true);
        UpdateWheel(wcbr, tbr, rotationDeltar, currentTurnAngle, false);
        UpdateWheel(wcbl, tbl, rotationDeltal, currentTurnAngle, false);
*/

        /*currentAccel = accelSensibility;
        wcbr.motorTorque = currentAccel * coefAccel;
        wcbl.motorTorque = currentAccel * coefAccel;*/

        /*if (!wcfr.isGrounded)
        {
            wheelfr_in_air += Time.deltaTime;
        }
        else
        {
            wheelfr_in_air = 0f;
        }

        if (!wcfl.isGrounded)
        {
            wheelfl_in_air += Time.deltaTime;
        }
        else
        {
            wheelfl_in_air = 0f;

        }

        if (!wcbr.isGrounded)
        {
            wheelbr_in_air += Time.deltaTime;
        }
        else
        {
            wheelbr_in_air = 0f;

        }

        if (!wcbl.isGrounded)
        {
            wheelbl_in_air += Time.deltaTime;
        }
        else
        {
            wheelbl_in_air = 0f;

        }

        int count_wheel_air = 0;

        if (wheelfr_in_air > 2.0f)
        {
            count_wheel_air++;
        }
        if (wheelfl_in_air > 2.0f)
        {
            count_wheel_air++;
        }
        if (wheelbr_in_air > 2.0f)
        {
            count_wheel_air++;
        }
        if (wheelbl_in_air > 2.0f)
        {
            count_wheel_air++;
        }

        if (count_wheel_air >= 2 && !reposition)
        {
            Debug.Log("In air");
            last_point_contact.y += 5.0f;
            qrepos = Quaternion.FromToRotation(transform.up, uprepos);
            transform.localRotation *= qrepos;
            rb.position = last_point_contact;
            reposition = true;
        }


        if (count_wheel_air <= 1)
        {
            if (wcfr.isGrounded)
            {
                WheelHit wht;
                wcfr.GetGroundHit(out wht);
                last_point_contact = wht.point;
            }
            else if (wcfl.isGrounded)
            {
                WheelHit wht;
                wcfl.GetGroundHit(out wht);
                last_point_contact = wht.point;
            }
            else if (wcbr.isGrounded)
            {
                WheelHit wht;
                wcbr.GetGroundHit(out wht);
                last_point_contact = wht.point;
            }
            else if (wcbl.isGrounded)
            {
                WheelHit wht;
                wcbl.GetGroundHit(out wht);
                last_point_contact = wht.point;
            }


            reposition = false;
        }*/

        /*
        if(Vector3.Distance(transform.position, agent.destination) <= 50)
        {
        //if (agent.remainingDistance <= agent.stoppingDistance)
        //{
            ind_goal = (ind_goal + 1) % goals.Length;
            agent.SetDestination(goals[ind_goal].position);
           
        }*/

        //Debug.Log("ind_goal : " + ind_goal);

        //int sp = (int)speed;
        //speed_screen.text = sp.ToString();

        //Debug.Log(rb.centerOfMass);
        //Debug.Log(currentTurnAngle);
        //Debug.Log(rb.velocity.magnitude * 3.6f);


    }

    void UpdateWheel(WheelCollider wc, Transform trans, Quaternion q, float angle, bool forward)
    {

        Vector3 position = trans.position;
        Quaternion rotation = trans.rotation;

        if (!forward)
        {
            wc.GetWorldPose(out position, out rotation);
            trans.rotation = rotation * q;
        }
        else
        {
            if (angle < 0.0f)
            {
                trans.localRotation = Quaternion.Euler(0.0f, -2.5f, 0.0f) * q;
            }
            else if (angle > 0.0f)
            {
                trans.localRotation = Quaternion.Euler(0.0f, 2.5f, 0.0f) * q;
            }
            else
            {
                trans.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f) * q;
            }

        }
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }


}
