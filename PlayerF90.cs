using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerF90 : MonoBehaviour
{
    PlayerController manette;
    public static bool GameEnd = false;
    public bool accelPlayer = false;
    public bool brakePlayer = false;
    bool r = false;
    public Vector2 mmove;

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
    bool crash = false;

    Vector3 last_point_contact;
    bool reposition = false;
    Quaternion qrepos;
    public Vector3 uprepos, frepos;

    public Rigidbody rb;

    public TextMeshProUGUI speed_screen;
    //public TextMeshProUGUI speed_volant;
    public TextMeshProUGUI time_lap_screen;
    public TextMeshProUGUI best_lap_screen;

    public Camera CamBack;
    //public Camera CamFirstPerson;
    bool camerab = true;

    public int goal = 0;
    public float time_lap = 0f;
    public float best_time_lap = 1000000f;

    //public Transform ReflectProbe;

    //public GameObject volant;
    //public GameObject brasgroup;

    /*public GameObject trailBRo;
    public GameObject trailBLo;
    public GameObject trailFRo;
    public GameObject trailFLo;*/

    TrailRenderer trailBR;
    TrailRenderer trailBL;
    TrailRenderer trailFR;
    TrailRenderer trailFL;

    public Transform lap_check1;
    public Transform lap_check2;
    public Transform lap_check3;
    public Transform lap_check4;
    public Transform lap_check5;
    public Transform lap_check6;


    float time_lc = 0.0f;
    bool start_lc = false;

    public bool demo;


    void Awake()
    {
        Debug.Log("Awake");
        manette = new PlayerController();
        manette.Gameplay.Accel.performed += Accel;
        manette.Gameplay.Accel.canceled += CancelAccel;
        manette.Gameplay.Brake.performed += Brake;
        manette.Gameplay.Brake.canceled += CancelBrake;
        manette.Gameplay.Move.performed += Move;
        manette.Gameplay.Move.canceled += CancelMove;
        manette.Gameplay.R.performed += R;
        manette.Gameplay.R.canceled += CancelR;
        manette.Gameplay.ChangeView.performed += ChangeView;
        manette.Gameplay.ChangeView.canceled += CancelChangeView;
        manette.Gameplay.Restart.performed += Restart;
        manette.Gameplay.MenuPrinc.performed += MenuPrinc;

    }

    public void Accel(InputAction.CallbackContext ctx)
    {
        accelPlayer = true;
        //Debug.Log("Accel");
        start_lc = true;
    }

    public void CancelAccel(InputAction.CallbackContext ctx)
    {
        accelPlayer = false;

    }

    public void Brake(InputAction.CallbackContext ctx)
    {
        brakePlayer = true;
        //Debug.Log("Brake");
        start_lc = false;
    }

    public void CancelBrake(InputAction.CallbackContext ctx)
    {
        brakePlayer = false;

    }

    public void Move(InputAction.CallbackContext ctx)
    {
        mmove = ctx.ReadValue<Vector2>();

    }

    public void CancelMove(InputAction.CallbackContext ctx)
    {
        mmove = Vector2.zero;

    }

    public void R(InputAction.CallbackContext ctx)
    {
        if (speed <= 0.5f) r = !r;

    }

    public void CancelR(InputAction.CallbackContext ctx)
    {
        //r = false;

    }

    public void ChangeView(InputAction.CallbackContext ctx)
    {
        /*camerab = !camerab;

        if (camerab)
        {
            CamBack.enabled = false;
            CamFirstPerson.enabled = true;
        }
        else
        {*/
            CamBack.enabled = true;
            /*CamFirstPerson.enabled = false;
        }*/


    }

    public void CancelChangeView(InputAction.CallbackContext ctx)
    {


    }

    public void Restart(InputAction.CallbackContext ctx)
    {
        if (crash)
        {
            Vector3 lp = Vector3.zero;
            if (goal == 0)
            {
                lp = lap_check1.position;
            }
            else if (goal == 1)
            {
                lp = lap_check1.position;
            }
            else if (goal == 2)
            {
                lp = lap_check2.position;
            }
            else if (goal == 3)
            {
                lp = lap_check3.position;
            }
            else if (goal == 4)
            {
                lp = lap_check4.position;
            }
            else if (goal == 5)
            {
                lp = lap_check5.position;
            }
            else if (goal == 6)
            {
                lp = lap_check6.position;
            }

            lp.y += 5.0f;
            qrepos = Quaternion.FromToRotation(transform.up, uprepos);
            transform.localRotation *= qrepos;
            rb.position = lp;
            reposition = true;
        }
    }

    public void MenuPrinc(InputAction.CallbackContext ctx)
    {
        Debug.Log("Menu Princ");

        GameEnd = !GameEnd;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = new Vector3(0f, 0f, 0f);
        rotationDeltar = Quaternion.FromToRotation(wcfr.transform.forward, tfr.forward);
        rotationDeltal = Quaternion.FromToRotation(wcfl.transform.forward, tfl.forward);
        uprepos = wcfr.transform.up;
        best_time_lap = 1000000f;

        /*trailBR = trailBRo.GetComponent<TrailRenderer>();
        trailBL = trailBLo.GetComponent<TrailRenderer>();
        trailFR = trailFRo.GetComponent<TrailRenderer>();
        trailFL = trailFLo.GetComponent<TrailRenderer>();*/
    }

    public void UpdateCarInfo(int acc, int tu)
    {
        float speed = rb.velocity.magnitude * 3.6f;

        currentBrake = 0.0f;
        accel = 0.0f;
        if (acc == 1 && speed > 100.0f)
        {
            //float sp = rb.velocity.magnitude * 3.6f;
            //if(sp > 50.0f)
                currentBrake = brakeSensibility;
            //Debug.Log("Brake");
        }
        else
        {
            accel = accelSensibility;
            //Debug.Log("accel");
        }


        int[] corrt = { -1, 0, 1 };
        if (speed >= 20.0f)
        {
            
            turn = corrt[tu];

        }
        else
        {
            turn = corrt[0];
        }

        /*float turn_w = (speed * 4000.0f / 200.0f);
        if (speed > 0.5f)
        {

            tbr.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltar;
            tbl.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltal;

        }

        currentTurnAngle = turnSensibility * turn;
        UpdateWheel(wcfr, tfr, rotationDeltar, currentTurnAngle, true);
        UpdateWheel(wcfl, tfl, rotationDeltal, currentTurnAngle, true);
        //UpdateWheel(wcbr, tbr, rotationDeltar, currentTurnAngle, false);
        //UpdateWheel(wcbl, tbl, rotationDeltal, currentTurnAngle, false);

        if (speed > 0.5f)
        {
            tfr.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltar;
            tfl.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltal;
        }*/

        int sp = (int)speed; 
        speed_screen.text = sp.ToString();

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
        

        time_lap += Time.deltaTime;

        float minutes = 0f;
        float sec = 0f;
        float ms = 0f;

        minutes = (int)(time_lap / 60f);
        sec = (int)(time_lap - (minutes * 60f));
        ms = (int)((time_lap - Mathf.Floor(time_lap)) * 100);
        time_lap_screen.text = "Time : " + minutes.ToString() + ":" + ((sec < 10) ? "0" : "") + sec.ToString() + ":" + ms.ToString();

        if (best_time_lap != 1000000f)
        {
            minutes = (int)(best_time_lap / 60f);
            sec = (int)(best_time_lap - (minutes * 60f));
            ms = (int)((best_time_lap - Mathf.Floor(best_time_lap)) * 100);
            best_lap_screen.text = "Best Time : " + minutes.ToString() + ":" + ((sec < 10) ? "0" : "") + sec.ToString() + ":" + ms.ToString();
        }

        //ReflectProbe.position = transform.position;

        if (demo)
        {
            wcfr.brakeTorque = currentBrake * coefAccel;
            wcfl.brakeTorque = currentBrake * coefAccel;
            wcbr.brakeTorque = currentBrake * coefAccel;
            wcbl.brakeTorque = currentBrake * coefAccel;

            wcbr.motorTorque = accel * coefAccel;
            wcbl.motorTorque = accel * coefAccel;

            float speed = rb.velocity.magnitude * 3.6f;
            if (speed < 50)
            {
                turnSensibility = 25.0f;
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

            currentTurnAngle = turnSensibility * turn;
            wcfr.steerAngle = currentTurnAngle;
            wcfl.steerAngle = currentTurnAngle;

        
        }
        else
        {

            if (accelPlayer)
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

            /*trailBR.emitting = false;
            trailBL.emitting = false;
            trailFR.emitting = false;
            trailFL.emitting = false;*/

            if (brakePlayer)
            {
                currentBrake = brakeSensibility;
                time_lc = 0.0f;
                /*if (wcbr.isGrounded) trailBR.emitting = true;
                if (wcbl.isGrounded) trailBL.emitting = true;
                if (wcfr.isGrounded) trailFR.emitting = true;
                if (wcfl.isGrounded) trailFL.emitting = true;*/

            }

            wcfr.brakeTorque = currentBrake * coefAccel;
            wcfl.brakeTorque = currentBrake * coefAccel;
            wcbr.brakeTorque = currentBrake * coefAccel;
            wcbl.brakeTorque = currentBrake * coefAccel;

            wcbr.motorTorque = currentAccel * coefAccel;
            wcbl.motorTorque = currentAccel * coefAccel;

            //Debug.Log(currentAccel);

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

            //GetComponent<AudioSource>().pitch = 1.0f + speed / 200f;


            currentTurnAngle = turnSensibility * mmove.x;
            wcfr.steerAngle = currentTurnAngle;
            wcfl.steerAngle = currentTurnAngle;

        }

        //tfr.localEulerAngles = new Vector3(tfr.localEulerAngles.x, wcfr.steerAngle - tfr.localEulerAngles.z, tfr.localEulerAngles.z);
        //tfl.localEulerAngles = new Vector3(tfl.localEulerAngles.x, wcfl.steerAngle - tfl.localEulerAngles.z, tfl.localEulerAngles.z);

        speed = rb.velocity.magnitude * 3.6f;
        float turn_w = (speed * 4000.0f / 200.0f);
        if (speed > 0.5f)
        {

            tbr.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltar;
            tbl.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltal;

        }


        UpdateWheel(wcfr, tfr, rotationDeltar, currentTurnAngle, true);
        UpdateWheel(wcfl, tfl, rotationDeltal, currentTurnAngle, true);
        //UpdateWheel(wcbr, tbr, rotationDeltar, currentTurnAngle, false);
        //UpdateWheel(wcbl, tbl, rotationDeltal, currentTurnAngle, false);

        if (speed > 0.5f)
        {
            tfr.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltar;
            tfl.localRotation = Quaternion.Euler(turn_w * 360 * Time.deltaTime, 0.0f, 0.0f) * rotationDeltal;
        }

        if (!wcfr.isGrounded)
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
            //Debug.Log("In air");
            crash = true;
        }


        if (count_wheel_air <= 1)
        {

            int count = 0;
            Vector3 pt_contact = Vector3.zero;
            if (wcfr.isGrounded)
            {

                WheelHit wht;
                wcfr.GetGroundHit(out wht);
                if (wht.collider.gameObject.tag == "circuit")
                {
                    pt_contact += wht.point;
                    ++count;
                }
            }
            if (wcfl.isGrounded)
            {
                WheelHit wht;
                wcfl.GetGroundHit(out wht);
                if (wht.collider.gameObject.tag == "circuit")
                {
                    pt_contact += wht.point;
                    ++count;
                }
            }
            if (wcbr.isGrounded)
            {
                WheelHit wht;
                wcbr.GetGroundHit(out wht);
                if (wht.collider.gameObject.tag == "circuit")
                {
                    pt_contact += wht.point;
                    ++count;
                }
            }
            if (wcbl.isGrounded)
            {
                WheelHit wht;
                wcbl.GetGroundHit(out wht);
                if (wht.collider.gameObject.tag == "circuit")
                {
                    pt_contact += wht.point;
                    ++count;
                }
            }

            if (count > 0)
            {
                last_point_contact = pt_contact / count;
                crash = false;

            }

            reposition = false;
        }

        //volant.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f * mmove.x);
        //brasgroup.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f * mmove.x);

        int sp = (int)speed;
        speed_screen.text = sp.ToString();
        //speed_volant.text = sp.ToString();


        if (sp <= 100 && start_lc) time_lc += Time.deltaTime;
        if (sp >= 100)
        {
            //Debug.Log("0-100: " + time_lc);
            start_lc = false;
        }

        //Debug.Log(rb.centerOfMass);
        //Debug.Log(currentTurnAngle);
        //Debug.Log(rb.velocity.magnitude * 3.6f);


    }

    void UpdateWheel(WheelCollider wc, Transform trans, Quaternion q, float angle, bool forward)
    {

        Vector3 position = trans.position;
        Quaternion rotation = trans.rotation;

        /*if (!forward)
        {
            wc.GetWorldPose(out position, out rotation);
            if (speed > 0.5f)
                trans.rotation = rotation * q;
          
        }
        else
        {*/
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

        /* wc.GetWorldPose(out position, out rotation);
         if (speed > 0.5f)
             trans.rotation = rotation * q;


     }*/
    }

    void OnEnable()
    {
        manette.Gameplay.Enable();
    }

    void OnDisable()
    {
        manette.Gameplay.Disable();
    }


}
