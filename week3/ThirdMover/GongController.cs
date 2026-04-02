using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GongController : MonoBehaviour
{
    private Rigidbody gongRD;
    float speed = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        gongRD = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) == true)
        {
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            gongRD.AddForce(-speed, 0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) == true)
        {
            //transform.Translate(Vector3.right * speed * Time.deltaTime);
            gongRD.AddForce(speed, 0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) == true)
        {
            //transform.Translate(Vector3.forward * speed * Time.deltaTime);
            gongRD.AddForce(0f, 0f, speed);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) == true)
        {
            //transform.Translate(Vector3.back * speed * Time.deltaTime);
            gongRD.AddForce(0f, 0f, -speed);
        }

        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            gongRD.AddForce(0f, speed, 0f);
        }

    }
}
