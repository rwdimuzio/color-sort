using System;
using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    const int NORTH=0;
    const int EAST=1;
    const int SOUTH=2;
    const int WEST=3;
    const float MIN_SECS=0.100f;
    const float MAX_SECS=3.00f;
    const float SEC_DEC=.200f;

    public Material[] materials; // 4 x target colors, one for every point of the compass 
    public MaterialIndex[] queue;  // show them the next view in the queue
    public Thermometer thermometer; // the countdown timer
    public GameObject center; // center piece - dups of this are also flung
    public GameObject BooBoo; // fault object
    public GameObject gameOverUI; // game over/start presentation
    public TMP_Text scoreUI;
    public TMP_Text streakUI;
    public TMP_Text highScoreUI;
    public TMP_Text longestStreakUI;

    private int targetDirection = 0;

    // score, level
    private float countDownSecs = MAX_SECS;
    private int hits = 0;
    private int level=0;
    private int score=0;
    private int streak=0;
    private int highScore=0;
    private int longestStreak=0;
    private bool gameOver = true; 


    // preserve key directions
    bool lastNorth;
    bool lastEast;
    bool lastSouth;
    bool lastWest;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this);
        }
        highScore = 0;
        longestStreak = 0;
    }

    void Update()
    {
        if(gameOver){
            return;
        }
        
        if (BooBoo.active){
            return;
        }

        var north = Input.GetKey(KeyCode.UpArrow);
        if (north && !lastNorth) {
            if (canGo(NORTH)) {
                OnHit();
                FlingNorth();
            } else {
                showBooBoo();
            }
        }
        lastNorth = north;

        var east = Input.GetKey(KeyCode.RightArrow);
        if (east && !lastEast) {
            if (canGo(EAST)) {
                OnHit();
                FlingEast();
            } else {
                showBooBoo();
            }
        }
        lastEast = east;

        var south = Input.GetKey(KeyCode.DownArrow);
        if (south && !lastSouth) {
            if (canGo(SOUTH)) {
                OnHit();
                FlingSouth();
            } else {
                showBooBoo();
            }
        }
        lastSouth = south;

        var west = Input.GetKey(KeyCode.LeftArrow);
        if (west && !lastWest) {
            if (canGo(WEST) ) {
                OnHit();
                FlingWest();
            } else {
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
        int m = popQueue();
        center.GetComponent<MaterialIndex>().setIndex(m, materials[m]);
        targetDirection = m;
        thermometer.ResetTimer(countDownSecs);
    }

    private void setZ(GameObject obj, float pos)
    {
        Vector3 t = obj.transform.position;
        obj.gameObject.transform.position = new Vector3(t.x, t.y, pos);
    }

    private int popQueue()
    {
        int idx = queue[0].getIndex();
        for (int i = 0; i < 4 - 1; i++) {
            queue[i].setIndex(
                queue[i + 1].getIndex(),
                materials[queue[i + 1].getIndex()]
            );
        }
        int prevIdx = queue[3].getIndex();
        int nextIdx = prevIdx;
        while (prevIdx == nextIdx) {
            nextIdx = Random.Range(0, materials.Length);
        }
        queue[3].setIndex(nextIdx, materials[nextIdx]);
        return idx;
    }

    private void FlingNorth()
    {
        SendChip(Vector3.up);
        ActivateNext();
    }

    private void FlingEast()
    {
        SendChip(Vector3.right);
        ActivateNext();
    }

    private void FlingSouth()
    {
        SendChip(Vector3.down);
        ActivateNext();
    }

    private void FlingWest()
    {
        SendChip(Vector3.left);
        ActivateNext();
    }

    private void SendChip(Vector3 direction)
    {
        GameObject obj   =  Instantiate(center,
            new Vector3(
                center.transform.position.x,
                center.transform.position.y,
                center.transform.position.z-1.0f
            ),
            Quaternion.identity
        );
        MaterialIndex mat = obj.GetComponent<MaterialIndex>();
        mat.setIndex(targetDirection, materials[targetDirection]);
        var behavior = obj.GetComponent<CenterBehavior>();
        behavior.move(direction);
    }

    private bool canGo(int destDir){
        return targetDirection == destDir;
    }

    public void showBooBoo()
    {
        streak=0;
        StartCoroutine(booBooCoroutine());
    }

    private IEnumerator booBooCoroutine()
    {
        BooBoo.active = true;
        yield return new WaitForSeconds(.500f);
        BooBoo.active = false;
    }

    public void OnStartGame(){
        Debug.Log("OnGameStart");   
        //ActivateNext();
        // prime the pump
        for(int i = 0; i<5; i++){
           ActivateNext();
        }
        countDownSecs = MAX_SECS;
        hits = 0;
        level=1;
        score=0;
        gameOver=false;
        thermometer.ResetTimer(countDownSecs);
        gameOverUI.active = false;
    }

    public void OnHit(){
        hits++;
        streak++;
        score += level * 100 + streak * 10;
        if(streak > longestStreak){
            longestStreak = streak;
        }
        if(score > highScore ){
            highScore = score;
        }

        if(hits % 25 == 0){
            level += level;
            countDownSecs -= SEC_DEC;
            if(countDownSecs < MIN_SECS){
                countDownSecs = MIN_SECS;
            }
        }
        updateUI();
    }

    public void OnPlayerDeath(){
        OnEndGame();
    }

    public void OnEndGame(){
        gameOver=true;
        gameOverUI.active = true;
    }
    private void updateUI(){
        scoreUI.text = "Score: "+score;
        streakUI.text = "Streak: "+streak;
        highScoreUI.text = ""+highScore;
        longestStreakUI.text = ""+longestStreak;
    }

}
