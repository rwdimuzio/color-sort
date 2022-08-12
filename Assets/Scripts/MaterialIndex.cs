using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialIndex : MonoBehaviour
{
    private int index=0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void setIndex(int i, Material  m){
        index = i;
        gameObject.GetComponent<Renderer>().material = m; 
    }

    public int getIndex(){
        return index;
    }
}
