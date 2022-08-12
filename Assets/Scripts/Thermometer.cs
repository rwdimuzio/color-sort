using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermometer : MonoBehaviour
{
    public Material[] materials;
    public float maxTime =  5.0f;
    public float minTime =  0.2f;
    public float timeDecrement = 0.5f;

    private float pct=1.0f;
    private float maxScale=20f;
    private Material m_Material;
    private float time;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        maxScale = gameObject.transform.localScale.x;
        m_Material =  GetComponent<Renderer>().material;
        ResetTimer(maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        float prevTime = elapsedTime;
        elapsedTime -= Time.deltaTime;
        if(elapsedTime < 0.0f ) elapsedTime = 0.0f;
        pct = elapsedTime/time;
        gameObject.transform.localScale = new Vector3(pct * maxScale, 1.0f, 1.0f);
        if(pct > .50 )
            m_Material = materials[0];
        else if (pct > .10)
            m_Material = materials[1];
        else 
            m_Material = materials[2];
        GetComponent<Renderer>().material = m_Material;
        if(prevTime > 0.0f && elapsedTime == 0.0f){
//TODO put me back            time -= timeDecrement;
            if(time<minTime) time = minTime; 
            ResetTimer(time);
        }
    }

    public void OnTimeIsUp(){
        ResetTimer(10.0f);
    }

    public void ResetTimer(float timeSec){
        time = timeSec;
        elapsedTime = timeSec;
    }
}
