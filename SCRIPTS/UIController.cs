using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text mainTextCountClamp;
    public static int levelCount;
    public Text textNumberWall;
    private Animation textAnim;
    private int numberWall = 0;
    private Animation mainTextAnim;

    public GameObject gamePanel, menuPanel, gameOverPanel,nextLevelPanel;
    public Text textLevel;
    private Animation cameraAnim;
    private AudioSource audioVzmahCamera;
    private AudioSource mainAudioMusic;
    public float addingMusicVolume;
    private WaitForSeconds wait,waitDown;
    public float timeWait, timeWaitDown;
    public AudioClip[] chponeck;
    public GameObject myAdsVisual;
    public Text levelTexMain, levelTextGameOver, levelTextRestart,wallsClambText;

    void Awake()
    {
        wait = new WaitForSeconds(timeWait);
        waitDown = new WaitForSeconds(timeWaitDown);
        mainAudioMusic = FindObjectOfType<Visualizer>().GetComponent<AudioSource>();
        cameraAnim = Camera.main.GetComponent<Animation>();
        audioVzmahCamera = Camera.main.GetComponent<AudioSource>();
        textAnim = textNumberWall.GetComponent<Animation>();
        LevelController.spawnEvent += TextAnim;
        mainTextAnim = mainTextCountClamp.GetComponent<Animation>();

        Checker.checkerEvent += CheckCount;
        LevelController.startNewLevel += SetStartUiValues;
        LevelController.finishEvent += FinishUI;
        PlayerBox.gameOverEvent += GameOver;
        MoveController.magnitingEvent += ForMagniting;
        //SetLevelsUI();
    }

    private void FinishUI()
    {
        myAdsVisual.SetActive(true);
        audioVzmahCamera.Play();
       
        cameraAnim.Play("CameraDown");
        gamePanel.SetActive(false);
        nextLevelPanel.SetActive(true);
        menuPanel.SetActive(true);
        StartCoroutine(SoundMusicUpOrDown(false));
        SetLevelsUI();
    }
    private void TextAnim()
    {
        textAnim.Play();
        numberWall++;
        textNumberWall.text = numberWall.ToString();       
    }
    private void GameOver()
    {
        myAdsVisual.SetActive(true);
        audioVzmahCamera.PlayOneShot(chponeck[2], 1);
        audioVzmahCamera.PlayOneShot(chponeck[3], 1);

        audioVzmahCamera.PlayOneShot(chponeck[4], 0.4f);
        Invoke("GameOverSoundVoice", 0.7f);


        menuPanel.SetActive(true);
        nextLevelPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        //cameraAnim.Stop("CameraDown");
        cameraAnim.Play("CameraGameOver");
        gamePanel.SetActive(false);
        StartCoroutine(SoundMusicUpOrDown(false));
        SetLevelsUI();
    }
    private void SetLevelsUI()
    {
        //levelTexMain.text = LevelController.Level.ToString();
        levelTextGameOver.text = LevelController.Level.ToString();
        levelTextRestart.text = LevelController.Level.ToString();
    }
    public void ShawWallsCount(int walls , int maxWalls)
    {
        wallsClambText.text = walls + " / " + maxWalls;
    }
    private void GameOverSoundVoice()
    {
        cameraAnim.Play("CameraDown");
        audioVzmahCamera.PlayOneShot(chponeck[5], 0.3f);
        
        audioVzmahCamera.PlayOneShot(chponeck[6], 0.35f);
    }
    private IEnumerator SoundMusicUpOrDown(bool isUP)
    {
        if (isUP)
        {
            while (mainAudioMusic.volume < 1f)
            {
                mainAudioMusic.volume += addingMusicVolume;
                yield return wait;
            }
        }
        else
        {
            while (mainAudioMusic.volume > 0.07f)
            {
                mainAudioMusic.volume -= addingMusicVolume;
                yield return waitDown;
            }
        }
       
    }
    private void ForMagniting()
    {
        audioVzmahCamera.PlayOneShot(chponeck[0],1f);
        audioVzmahCamera.PlayOneShot(chponeck[1], 1f);
    }
    private void SetStartUiValues()
    {
        myAdsVisual.SetActive(false);
        StartCoroutine(SoundMusicUpOrDown(true));
        audioVzmahCamera.Play();
        cameraAnim.Stop("cameraDown");
        cameraAnim.Play("CameraUp");
        numberWall = 0;
        mainTextCountClamp.text = 0 + " / " + LevelController.levelWallsCount;
        textLevel.text = "Level: " + LevelController.Level;
    }
    private void CheckCount()
    {
        mainTextCountClamp.text = numberWall + " / " + LevelController.levelWallsCount;
        mainTextAnim.Play();
    }
    private void OnDestroy()
    {
        LevelController.spawnEvent -= TextAnim;
        Checker.checkerEvent -= CheckCount;
        LevelController.startNewLevel -= SetStartUiValues;
        LevelController.finishEvent -= FinishUI;
        PlayerBox.gameOverEvent -= GameOver;
        MoveController.magnitingEvent -= ForMagniting;
    }
}
