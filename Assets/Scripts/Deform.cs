using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deform : MonoBehaviour
{

    [Range(0, 1)]
    public float deformRadius = 0.5f;
    [Range(0, 100000)]
    public float minDamage = 1;

    public GameObject objectdef;
    private MeshFilter filter;
    
    private Vector3[] startingVerticies;
    private Vector3[] meshVerticies;
    private Vector3[] originalVerticies;

    System.Random randy;

    public GameObject capsule;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        filter = objectdef.GetComponent<MeshFilter>();
        originalVerticies = filter.mesh.vertices;

        rb = GetComponent<Rigidbody>();

        randy = new System.Random();


    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 center = Vector3.zero;

        for (int i = 0; i < startingVerticies.Length; i++)
        {
            center.x += startingVerticies[i].x;
            center.y += startingVerticies[i].y;
            center.z += startingVerticies[i].z;

        }

        center.x /= (float)startingVerticies.Length;
        center.y /= (float)startingVerticies.Length;
        center.z /= (float)startingVerticies.Length;

        for (int i = 0; i < startingVerticies.Length; i++)
        {
            Vector3 vdf = startingVerticies[i] - center;
            meshVerticies[i] = startingVerticies[i] + vdf * (float)randy.NextDouble()* deformRadius;

        }

        UpdateMeshVerticies();*/
    }

    void OnCollisionEnter(Collision collision)
    {
        startingVerticies = filter.mesh.vertices;
        meshVerticies = filter.mesh.vertices;
        int count = 0;
        float speed = rb.velocity.magnitude;
        float collisionPower = collision.impulse.magnitude;

        float distance = collisionPower * 0.0375f / 50000.0f;
        /*if (collisionPower > 15000)
        {
            distance = 100000;
        }*/

        if(collisionPower > 0.0f)
            Debug.Log("Collision " + collisionPower.ToString() + " " + collision.impulse + " " + speed);
        
        
        if (collisionPower > 1000.0f) {
            ContactPoint[] contactPoint = new ContactPoint[collision.contactCount];
            collision.GetContacts(contactPoint);

            foreach (ContactPoint cpt in contactPoint)
            {
                Vector3 contactVelocity = collision.relativeVelocity * 0.02f;
                Vector3 localContactPoint = objectdef.transform.InverseTransformPoint(cpt.point);
                Vector3 localContactForce = objectdef.transform.InverseTransformDirection(contactVelocity);

                //capsule.transform.position = objectdef.transform.TransformPoint(localContactPoint);

                Debug.Log("contact " + objectdef.transform.InverseTransformPoint(cpt.point) + " " + contactPoint.Length);
                for (int i = 0; i < startingVerticies.Length; i++)
                {
                    float d = Vector3.Distance(meshVerticies[i], objectdef.transform.InverseTransformPoint(cpt.point));
                    //float d = (transform.InverseTransformPoint(cpt.point) - startingVerticies[i]).magnitude;
                    

                    if(d < distance)
                    {
                        //Debug.Log("d " + d + " " + collisionPower * 0.025f / 170.0f);
                        ++count;
                        //0.0000625f
                        float factor = collisionPower * 0.00000390625f / 50000.0f;

                        //Debug.Log("factor " + factor);
                        float rd = (float)randy.NextDouble();
                        int pmx = randy.Next(2);
                        int pmy = randy.Next(2);
                        int pmz = randy.Next(2);
                        //Vector3 vdf = (objectdef.transform.InverseTransformPoint(cpt.point) - objectdef.transform.TransformPoint(startingVerticies[i])).normalized;
                        /*Vector3 mv;
                        if (pm==1) {
                            mv = new Vector3(meshVerticies[i].x + rd * factor,
                                                     meshVerticies[i].y + rd * factor,
                                                     meshVerticies[i].z + rd * factor);
                        }
                        else
                        {
                            mv = new Vector3(meshVerticies[i].x - rd * factor,
                                                     meshVerticies[i].y - rd * factor,
                                                     meshVerticies[i].z - rd * factor);
                        }
                        Vector3 deformation = mv - originalVerticies[i];*/

                        /*if (deformation.magnitude > Mathf.Sqrt(0.001f))
                        {
                            factor = 0.001f; 
                        }*/


                        if (pmx==1)
                        {
                            meshVerticies[i].x += rd * factor;
                            
                        }
                        else
                        {
                            meshVerticies[i].x -= rd * factor;
                          
                        }

                        if (pmy == 1)
                        {
                            meshVerticies[i].y += rd * factor;
                           
                        }
                        else
                        {
                            meshVerticies[i].y -= rd * factor;
                            
                        }

                        if (pmz == 1)
                        {
                            meshVerticies[i].z += rd * factor;
                        }
                        else
                        {
                            meshVerticies[i].z -= rd * factor;
                        }
                        /*if (deformation.magnitude > 0.05f)
                        {
                            meshVerticies[i] = originalVerticies[i] + deformation.normalized * 0.001f;
                        }*/

                        /*
                        meshVerticies[i].x = startingVerticies[i].x + vdf.x * 0.001f ;
                        meshVerticies[i].y = startingVerticies[i].y + vdf.y * 0.001f ;
                        meshVerticies[i].z = startingVerticies[i].z + vdf.z * 0.001f ;
                        */
                    }
                }

            }

            Debug.Log("count " + count + "/" + meshVerticies.Length);


        }

        UpdateMeshVerticies();

    }

    void OnCollisionStay(Collision collision)
    {
        /*float collisionPower = collision.impulse.magnitude;
        float speed = rb.velocity.magnitude;
        Debug.Log("CollisionStay " + collisionPower.ToString() + " " + speed);
        */
    }

        void UpdateMeshVerticies()
    {
        filter.mesh.vertices = meshVerticies;
        filter.mesh.RecalculateNormals();
        //coll.sharedMesh = filter.mesh;
    }


}
