using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    [Space]
    [Header("Prefabs")]
    [SerializeField] private GameObject m_PlayerSquarePrefab;

    //The Awake of InputRecorder is not guarenteed to run before the OnEnable of PlayerController if spawned in the same frame. 
    // this is to delay the PlayerControllers onEnable by a frame.

    [SerializeField] private GameObject m_ReplayRunnerPrefab;

    [Space]
    [Header("Options")]
    [SerializeField] private bool m_RunReplay;
    [SerializeField] private int m_ReplayLog = 4;


    private ReplayRunner m_Runner; 
    private PlayerController m_Controller; 
    private void Awake() {
        if(GameManager.Instance == null) GameManager.Instance = this;
        else Destroy(this);
    }

    void Start()
    {
        if (m_RunReplay)
        {
            StartCoroutine(RunReplay(m_ReplayLog)); 
        }
        else
        {
            StartCoroutine(SpawnSquare());
        }
    }
    private IEnumerator RunReplay(int logNum){
        var frameWait = new WaitForEndOfFrame();
        yield return frameWait; 

        m_Runner = GetReplayRunner().GetComponent<ReplayRunner>(); 
        m_Runner.Init(logNum); 
        yield return frameWait; 
        m_Controller = GetSquare().GetComponent<PlayerController>(); 
        m_Controller.SubscribeToReplayRunner(m_Runner); 
        m_Runner.StartReplay();

    }
    private IEnumerator SpawnReplayRunner(){
        yield return new WaitForEndOfFrame();
        
        m_Runner = GetReplayRunner().GetComponent<ReplayRunner>();
        m_Runner.Init(m_ReplayLog);
        
    }
    private IEnumerator SpawnSquare()
    {
        yield return new WaitForEndOfFrame();
        m_Controller = GetSquare().GetComponent<PlayerController>();
        m_Controller.SubscribeToInputRecorder();
    }
    public GameObject GetSquare()
    {
        GameObject square = Instantiate(m_PlayerSquarePrefab);
        square.transform.parent = this.transform;
        return square;
    }
    public GameObject GetReplayRunner(){
        return Instantiate(m_ReplayRunnerPrefab);
    }
}
