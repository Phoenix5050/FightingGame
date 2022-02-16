using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

public class MainMenuScript : MonoBehaviour
{

    [SerializeField] private Transform m_ButtonHolder;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button m_PlayButton;
    [SerializeField] private Button m_ReplayButton;
    [SerializeField] private GameObject m_ButtonPrefab;

    [Space]
    [Header("Animation Positions")]
    [SerializeField] private Vector3 m_PlayFinalPosition;
    [SerializeField] private Vector3 m_ReplayFinalPosition;

    [Space]
    [Header("Animation Durations")]
    [SerializeField] private float m_AnimateShort;
    [SerializeField] private float m_AnimateMeduim;
    [SerializeField] private float m_AnimateLong;


    private List<string> m_EntryNames;

    private void Awake()
    {
        m_PlayButton.onClick.AddListener(() =>
        {
            Debug.Log("Playbutton onClick");
        });
        m_ReplayButton.onClick.AddListener(() =>
        {
            Debug.Log("replay button clicked");
            Vector3 pos = m_ReplayButton.GetComponent<RectTransform>().anchoredPosition;
            foreach (var entry in m_EntryNames)
            {
                var go = Instantiate(m_ButtonPrefab, Vector3.zero, Quaternion.identity, m_ButtonHolder.transform).GetComponent<RectTransform>();

                go.anchoredPosition = pos + new Vector3(0, 0, -0.01f);
                go.transform.GetChild(0).GetComponent<Text>().text = entry;
                go.GetComponent<Button>().onClick.AddListener(()=>{
         
                });

            }
        });

    }
    private void Start()
    {
        m_EntryNames = InputRecorder.Instance.GetListOfLogs();
        foreach (var v in m_EntryNames)
        {
            Debug.Log(v);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AnimateMenu(true);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AnimateMenu(false);
        }
    }
    #region Button Actions

    private void ReplayButtonAction()
    {

    }
    #endregion
    #region Animations
    private void AnimateMenu(bool open)
    {

        if (open)
        {
            var sequence = DOTween.Sequence();
            var playRect = m_PlayButton.GetComponent<RectTransform>();
            var replayRect = m_ReplayButton.GetComponent<RectTransform>();
            sequence.Insert(0, playRect.DOAnchorPosX(m_PlayFinalPosition.x, m_AnimateMeduim));
            sequence.Insert(m_AnimateShort, replayRect.DOAnchorPosX(m_ReplayFinalPosition.x, m_AnimateMeduim));

            sequence.PlayForward();
        }
        else
        {
            var sequence = DOTween.Sequence();
            var playRect = m_PlayButton.GetComponent<RectTransform>();
            var replayRect = m_ReplayButton.GetComponent<RectTransform>();
            sequence.Insert(0, playRect.DOAnchorPosX(m_PlayFinalPosition.y, m_AnimateMeduim));
            sequence.Insert(m_AnimateShort, replayRect.DOAnchorPosX(m_ReplayFinalPosition.y, m_AnimateMeduim));
            sequence.PlayForward();
        }

    }
    private void AnimatePlayButtonClicked()
    {
        var sequence = DOTween.Sequence();


    }

    #endregion
}
