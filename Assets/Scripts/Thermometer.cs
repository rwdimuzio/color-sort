using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thermometer : MonoBehaviour
{
    public Material[] materials;
    public float timeDecrement = 0.5f;

    private float pct=1.0f;
    private float maxScale=20f;
    private Material m_Material;
    private float time;
    private float elapsedTime;
    private bool onACall=false;

    void Start()
    {
        maxScale = gameObject.transform.localScale.x;
        m_Material =  GetComponent<Renderer>().material;
    }

    void Update()
    {
        if(!onACall) return;

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
            OnTimeIsUp();
        }
    }

    public void OnTimeIsUp(){
        onACall=false;
        GameManager.Instance.OnPlayerDeath();
    }

    public void ResetTimer(float timeSec){
        onACall=true;
        time = timeSec;
        elapsedTime = timeSec;
    }
}
