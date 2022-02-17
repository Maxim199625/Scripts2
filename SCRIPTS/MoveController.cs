using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveController : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private float clamp;
    public static float magintedClamp,normalClamp;
    private Vector3 delta,lateDelta;

    [HideInInspector] public Player player;
    [HideInInspector] public Transform[] playerBoxes;

    private bool isVibrate;
    private WaitForSeconds time;
    private WaitForSeconds timeVisualizing;
    private LevelController levelCont;
    int minBoxCount;
    private bool canMove;
    public delegate void MagnitingDelegate();
    public static MagnitingDelegate magnitingEvent;


    private void Awake()
    {
        canMove = true;
        levelCont = FindObjectOfType<LevelController>();
        time = new WaitForSeconds(0.1f);
        timeVisualizing = new WaitForSeconds(0.4f);
        //player = FindObjectOfType<Player>();
        Checker.checkerEvent += ToCantMove;
        Checker.endCheckerEvent += ToCanMove;
        PlayerBox.gameOverEvent += ToCantMove;
        LevelController.spawnEvent += ToCanMove;
    }
    private void OnDestroy()
    {
        Checker.checkerEvent -= ToCantMove;
        Checker.endCheckerEvent -= ToCanMove;
        PlayerBox.gameOverEvent -= ToCantMove;
        LevelController.spawnEvent -= ToCanMove;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && canMove/* && controlling*/)
        {
            CheckMoving();
        }
    }
    private void ToCanMove()
    {
        canMove = true;
    }
    private void ToCantMove()
    {
        canMove = false;
    }
    private void CheckMoving()
    {
        Magniting();
        //if((Input.mousePosition - lateDelta).magnitude < 40f)
        delta += Input.mousePosition - lateDelta;
        lateDelta = Input.mousePosition;
        if (delta.magnitude > clamp)
        {
            
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0f)
                {
                    player.transform.Translate(Vector3.right);
                }
                else
                {
                    player.transform.Translate(Vector3.left);
                }
            }
            else
            {
                if (delta.y > 0f)
                {
                    player.transform.Translate(Vector3.up);
                }
                else
                {
                    player.transform.Translate(Vector3.down);

                }
            }

            if (!isVibrate)
                StartCoroutine(Vibrator());
                  
            delta = Vector3.zero;
            Vector3 plPos = player.transform.position;
            Vector3 pos = new Vector3(Mathf.Clamp(plPos.x, -4, 4f - player.maxX), Mathf.Clamp(plPos.y, 0f, 2f - player.maxY), 0f);
            player.transform.position = pos;
            delta = Vector3.zero;
        }
        
    }
    private void Magniting()
    {
        
        for (int t = 1; t < playerBoxes.Length; t++)
        {
            for (int p = 0; p < levelCont.poses.Count; p++)
            {
                Vector3 nP = levelCont.poses[p];
                nP.z = 0f;
                if (nP == playerBoxes[t].position)
                {
                    minBoxCount++;
                }
            }
        }
        if (minBoxCount == playerBoxes.Length - 1)
        {
            clamp = magintedClamp;
            if (!isVisualizing)
                VisualizeMagniting();
        }
        else
        {
            clamp = normalClamp;
            EndVisualizeMagniting();
        }
        minBoxCount = 0;
    }
    bool isVisualizing;
    private void VisualizeMagniting()
    {
        magnitingEvent?.Invoke();
        isVisualizing = true;
        for (int t = 1; t < playerBoxes.Length; t++)
        {
            playerBoxes[t].GetComponent<Animation>().Play();
        }
    }
    private void EndVisualizeMagniting()
    {
        isVisualizing = false;
        for (int t = 1; t < playerBoxes.Length; t++)
        {
            playerBoxes[t].GetComponent<Animation>().Stop();
            playerBoxes[t].transform.localScale = Vector3.one;
        }
    }

    public void SetNewPlayer(Player pl)
    {
        player = pl;
        playerBoxes = player.GetComponentsInChildren<Transform>();
    }

    private IEnumerator Vibrator()
    {
        Vibration.Vibrate(20);
        isVibrate = true;
        yield return time;
        isVibrate = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        lateDelta = eventData.position;
        delta = Vector3.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lateDelta = eventData.position;
        delta = Vector3.zero;
    }
}
