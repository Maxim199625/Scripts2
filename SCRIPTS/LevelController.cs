using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    public GameObject[] wallPresetPrefabs;
    public float timeEndHelps;

    private GameObject[] wallPresets;

    private GameObject previouseGO;
    private MoveController moveCont;

    [HideInInspector] public List<Vector3> poses;
    public Transform[] navigationBoxes;
    private Animation[] navigationAnims;
    private MeshRenderer[] renderer;
    public Color32 colorWhite, selfColor;
    public Color32[] colorIndicator, colorIndicatorTr;
    private List<MeshRenderer> activeAnims = new List<MeshRenderer>();
    private Text textNumberWall;
    private Transform wallTrPosition;
    private Camera myCam;
    public delegate void SpawnDelagate();
    public static event SpawnDelagate spawnEvent;
    public static event SpawnDelagate startNewLevel;
    public static event SpawnDelagate finishEvent;
    public static event SpawnDelagate gameOverIvent;

    public static int levelWallsCount = 10,currentWall;
    //private static int currentCount;
    public static int Level = 1,MaximalLevel;
    public static float minLevelMoveSpeed, maxLevelMoveSpeed, currentSpeed;

    public Transform[] grounds;
    private MeshRenderer[] groundMeshes;
    public float startGroundsPosZ;
    public GameObject gameOverPanel,restartPanel,maximalLevelPanel;
    UIController uiController;
    
    public void LevelDificaltSystem()
    {
        //PlayerPrefs.SetInt("Level",59);
        Level = PlayerPrefs.GetInt("Level");
        
        int levelWalls = 14 + Level;
        levelWalls = Mathf.Clamp(levelWalls, 14, wallPresetPrefabs.Length);
        wallPresets = new GameObject[levelWalls];
        for (int i = 0; i < wallPresets.Length; i++)
        {
            wallPresets[i] = wallPresetPrefabs[i];
        }

        currentWall = 0;
        levelWallsCount = 10 + (int)(Level * 0.83f) - (int)(Level * 0.56f);
        levelWallsCount += Random.Range(-2, 3);

        //minLevelMoveSpeed = 33f + ( ( Level * 1.19f ) - Level );
        minLevelMoveSpeed = 33f + ((float)Level * 0.22f);
        print(minLevelMoveSpeed);
        minLevelMoveSpeed = Mathf.Clamp(minLevelMoveSpeed, 30f, 50f);
        maxLevelMoveSpeed = minLevelMoveSpeed * 1.23f * (Level * 0.14f);
        maxLevelMoveSpeed = Mathf.Clamp(maxLevelMoveSpeed, 44f, 69f);

        currentSpeed = Mathf.Lerp(minLevelMoveSpeed, maxLevelMoveSpeed, (float)currentWall / levelWallsCount);
        FinishChecker.countWallSpawns = levelWallsCount;

        MoveController.normalClamp = Mathf.Clamp(20f - (Level * 0.042f),12f, 20f);
        MoveController.magintedClamp = Mathf.Clamp(150f + (Level * 0.3f),150f,188f);

        textNumberWall = GameObject.Find("NumberWallText").GetComponent<Text>();
        moveCont = FindObjectOfType<MoveController>();

        startNewLevel?.Invoke();
    }
    public void SetWallValues()
    {
        currentWall++;
        currentSpeed = Mathf.Lerp(minLevelMoveSpeed, maxLevelMoveSpeed, (float)currentWall / levelWallsCount);        
        //print(currentSpeed + "CurentSpeed   " + minLevelMoveSpeed + "minSpeed   " + maxLevelMoveSpeed + "maxSpeed   " + currentWall + "currentWall  " + levelWallsCount + "wallCount  ");
    }
    public static void FinishLevel()
    {
        Level++;
        int maximal = PlayerPrefs.GetInt("MaximalLevel");
        if (Level > maximal)
        {
            PlayerPrefs.SetInt("MaximalLevel", Level);
        }
        PlayerPrefs.SetInt("Level", Level);

        finishEvent?.Invoke();
    }
    
    private void GameOver()
    {
        gameOverIvent?.Invoke();
        uiController.ShawWallsCount(currentWall,levelWallsCount);
        if (PlayerPrefs.GetInt("MaximalLevel") <= Level && Level != 1)
        {
            restartPanel.SetActive(false);
            maximalLevelPanel.SetActive(true);
        }
        else
        {
            restartPanel.SetActive(true);
            maximalLevelPanel.SetActive(false);
        }
        UnActiveGroundHelps();
        if (previouseGO)
        {
            Destroy(previouseGO);
        }
        poses.Clear();

    }
    public void RestartPreviousLevel()
    {
        Level--;
        PlayerPrefs.SetInt("Level", Level);
        StartNewLevel();
    }
    public void StartNewLevel()
    {
        LevelDificaltSystem();
        PlayerBox.isGameOver = false;

        if(currentWall == 0)
        {
            Invoke("Spawn", 0.5f);
        }
        else
            Spawn();
    }
    private void UnActivePanelsForAds()
    {
        restartPanel.SetActive(true);
        maximalLevelPanel.SetActive(false);
    }
    private void SetScreenConvertion()
    {
        int width = Screen.width;
        int hight = Screen.height;
        float prossent = (float)hight / (float)width;
        if (prossent > 2.1f)
        {
            myCam.fieldOfView = 103f;
        }
        else if (prossent > 1.99f)
        {
            myCam.fieldOfView = 98f;
        }
        else if (prossent > 1.8f)
        {
            myCam.fieldOfView = 94f;
        }
        else if (prossent > 1.3f)
        {
            myCam.fieldOfView = 90f;
        }
        else
        {
            myCam.fieldOfView = 80f;
        }
    }

    private void Start()
    {
        uiController = FindObjectOfType<UIController>();
        AdsManager.AdsCompleteEvent += UnActivePanelsForAds;
        PlayerBox.gameOverEvent += GameOver;
        myCam = Camera.main;
        SetScreenConvertion();

        navigationAnims = new Animation[navigationBoxes.Length];
        renderer = new MeshRenderer[navigationBoxes.Length];
        for (int i = 0; i < navigationBoxes.Length; i++)
        {
            renderer[i] = navigationBoxes[i].GetComponent<MeshRenderer>();
            
            navigationAnims[i] = navigationBoxes[i].GetComponent<Animation>();
        }

        groundMeshes = new MeshRenderer[grounds.Length];
        for(int g = 0;g< grounds.Length; g++)
        {
            groundMeshes[g] = grounds[g].GetComponent<MeshRenderer>();
        }
    }
    private void OnDestroy()
    {
        PlayerBox.gameOverEvent -= GameOver;
        AdsManager.AdsCompleteEvent -= UnActivePanelsForAds;
    }


    public void Spawn()
    {
        if (previouseGO)
        {
            Destroy(previouseGO);
        }
        poses.Clear();
        previouseGO = Instantiate(wallPresets[Random.Range(0,wallPresets.Length)], transform);
        //wallTrPosition = previouseGO.GetComponentInChildren<WallMover>().transform;
        WallMover wallM = previouseGO.GetComponentInChildren<WallMover>();
        Transform[] trs = wallM.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < trs.Length; i++)
        {
            if (!trs[i].gameObject.activeSelf)
            {
                poses.Add(trs[i].position);
                
            }
        }       
        
        SetWallValues();
        moveCont.SetNewPlayer(previouseGO.GetComponentInChildren<Player>());
        Helps();

        spawnEvent?.Invoke();
    }

    private void Helps()
    {
        UnActiveGroundHelps();

        int colorIndex = Random.Range(0, colorIndicator.Length);
        for (int p = 0; p < poses.Count; p++)
        {
            Vector3 posP = poses[p];
            posP.z = 0f;
            for (int g = 0; g < grounds.Length; g++)
            {
                if (posP.x == grounds[g].position.x)
                {
                    grounds[g].position = new Vector3(grounds[g].position.x, grounds[g].position.y, startGroundsPosZ);
                    grounds[g].localScale = new Vector3(0.1f, 0.8f, Random.Range(3f, 6.6f));
                    grounds[g].gameObject.SetActive(true);
                    groundMeshes[g].material.color = colorIndicatorTr[colorIndex];
                }
            }
            for (int n = 0; n < navigationBoxes.Length; n++)
            {
                if(posP == navigationBoxes[n].localPosition)
                {
                    renderer[n].material.color = colorIndicator[colorIndex];
                    navigationAnims[n].Play();
                    activeAnims.Add(renderer[n]);
                    break;
                }
            }
        }
        Invoke("UnActiveHelps", timeEndHelps);
    }
    private void UnActiveHelps()
    {
        foreach (MeshRenderer m in activeAnims)
        {
            m.material.color = colorWhite;
        }
        activeAnims.Clear();
    }
    private void UnActiveGroundHelps()
    {
        for (int g = 0; g < grounds.Length; g++)
        {
            grounds[g].gameObject.SetActive(false);
        }
    }
}
