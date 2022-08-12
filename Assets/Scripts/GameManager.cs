using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.ComponentModel;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    const int CENTERS=16;
    public Material[] materials;
    public MaterialIndex[] queue;
    public GameObject[] center;
    public GameObject BooBoo;

    public int centerIdx = 5;
    public int materialIdx = 0;


    // preserve key directions
    bool lastNorth;
    bool lastEast;
    bool lastSouth;
    bool lastWest;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }

        //ActivateNext();
        queue[0].setIndex(0, materials[0]);
        queue[1].setIndex(1, materials[1]);
        queue[2].setIndex(2, materials[2]);
        queue[3].setIndex(3, materials[3]);

        // prime queue
        for(int i = 0; i < center.Length; i++){
            popQueue();
        }
        centerIdx = center.Length -1 ;
        ActivateNext();
    }

    public void update()
    {
    }


    void Update()
    {
        if (BooBoo.active)
        {
            return;
        }

        var north = Input.GetKey(KeyCode.UpArrow);
        if (north && !lastNorth)
        {
            if (canGoNorth())
            {
                FlingNorth();
            }
            else
            {
                showBooBoo();
            }
        }
        lastNorth = north;

        var east = Input.GetKey(KeyCode.RightArrow);
        if (east && !lastEast)
        {
            if (canGoEast())
            {
                FlingEast();
                ActivateNext();
            }
            else
            {
                showBooBoo();
            }
        }
        lastEast = east;

        var south = Input.GetKey(KeyCode.DownArrow);
        if (south && !lastSouth)
        {
            if (canGoSouth())
            {
                FlingSouth();
                ActivateNext();
            }
            else
            {
                showBooBoo();
            }
        }
        lastSouth = south;

        var west = Input.GetKey(KeyCode.LeftArrow);
        if (west && !lastWest)
        {
            if (canGoWest())
            {
                FlingWest();
                ActivateNext();
            }
            else
            {
                showBooBoo();
            }
        }
        lastWest = west;
    }

    public void Start()
    {
    }

    public void ActivateNext()
    {
        setZ(CenterObj, -1.58f);

        int m = popQueue();

        centerIdx = (centerIdx + 1) % center.Length;

        CenterObj.GetComponent<MaterialIndex>().setIndex(m, materials[m]);
        setZ(CenterObj, -3.5f);
        materialIdx = m;
    }

    public GameObject CenterObj
    {
        get
        {
            return center[mcenter(centerIdx)];
        }
    }

    private GameObject Obj(int idx)
    {
        return center[mcenter(idx)].gameObject;
    }

    private void setZ(GameObject obj, float pos)
    {
        Vector3 t = obj.transform.position;
        obj.gameObject.transform.position = new Vector3(t.x, t.y, pos);
    }

    private int mcenter(int pos)
    {
        return pos % center.Length;
    }

    private int popQueue()
    {
        int idx = queue[0].getIndex();
        for (int i = 0; i < 4 - 1; i++)
        {
            queue[i]
                .setIndex(queue[i + 1].getIndex(),
                materials[queue[i + 1].getIndex()]);
        }
        int prevIdx = queue[3].getIndex();
        int nextIdx = prevIdx;
        while (prevIdx == nextIdx)
        {
            nextIdx = Random.Range(0, materials.Length);
        }
        queue[3].setIndex(nextIdx, materials[nextIdx]);
        return idx;
    }

    private void FlingNorth()
    {
        setZ(CenterObj, -3.25f);
        var behavior = CenterObj.GetComponent<CenterBehavior>();
        behavior.move(Vector3.up);
        ActivateNext();
    }

    private void FlingEast()
    {
        setZ(CenterObj, -3.26f);
        var behavior = CenterObj.GetComponent<CenterBehavior>();
        behavior.move(Vector3.right);
        ActivateNext();
    }

    private void FlingSouth()
    {
        setZ(CenterObj, -3.27f);
        var behavior = CenterObj.GetComponent<CenterBehavior>();
        behavior.move(Vector3.down);
        ActivateNext();
    }

    private void FlingWest()
    {
        setZ(CenterObj, -3.28f);
        var behavior = CenterObj.GetComponent<CenterBehavior>();
        behavior.move(Vector3.left);
        ActivateNext();
    }

    private bool canGoNorth()
    {
        return materialIdx == 0;
    }

    private bool canGoEast()
    {
        return materialIdx == 1;
    }

    private bool canGoSouth()
    {
        return materialIdx == 2;
    }

    private bool canGoWest()
    {
        return materialIdx == 3;
    }


    public void showBooBoo()
    {
        StartCoroutine(booBooCoroutine());
    }

    private IEnumerator booBooCoroutine()
    {
        BooBoo.active = true;
        yield return new WaitForSeconds(.001f);
        BooBoo.active = false;
    }
}
