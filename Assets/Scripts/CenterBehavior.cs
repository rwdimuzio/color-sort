using System.IO;
using System.Numerics;
using System;
using UnityEngine;
using Vector3=UnityEngine.Vector3;

public class CenterBehavior : MonoBehaviour
{
    public float speed = 0.5f;
    private Vector3 direction = Vector3.zero;
    Vector3 home;
    bool inFlight = false;
    // Start is called before the first frame update
    void Start()
    {
        home = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(direction != Vector3.zero){
            //speed=0.25f;
            transform.Translate(direction * Time.deltaTime * speed); 
        }

        float x = gameObject.transform.position.x;
        float y = gameObject.transform.position.y;
        if( x < -9.87f || x > 9.75f || y < -5.01 || y > 13.26){
           direction = Vector3.zero;
           inFlight = false;
           gameObject.transform.position = home;
        }
    }

    public void move(Vector3 direction){
        if(inFlight) return;
        this.direction = direction;
        inFlight = true;
    }
}
