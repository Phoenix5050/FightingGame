using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReplayRunner : MonoBehaviour
{
    private InputFrame EMPTY_FRAME = new InputFrame(true,-1,new InputData(false,false,false,false,false)); 
    public Action<InputFrame> OnInputReceived;
    private InputLog m_ReplayLog;
    private int m_LogID;
    private bool m_RunReplay = false;
    private int m_FrameCounter;
    // private int m_NextInputFrame = 0;
    private int m_InputPosition = -1;
    private InputFrame m_NextInputFrame; 

    #region Monobehaviour lifecycle
    private void OnEnable() {
        OnInputReceived += ReceivedInput; 
    }
    private void OnDisable(){
        OnInputReceived -= ReceivedInput; 
    }
    #endregion

    // Initialize the RepplayRunner with the log it will be replaying. 
    public void Init(int logID)
    {
        this.m_LogID = logID;
        m_ReplayLog = InputRecorder.Instance.GetReplayLog(logID);
        Debug.Log("Replay log: " + m_ReplayLog.Name);
    }

    public void StartReplay()
    {
        Debug.Log("Starting Replay");
        m_RunReplay = true;
        m_FrameCounter = 0;
        m_InputPosition = 0; 
        m_NextInputFrame = GetFrame(m_InputPosition); 
    }

    public void StopReplay()
    {
        Debug.Log("Stopping Replay: ");
        m_RunReplay = false;

    }
    private void FixedUpdate()
    {
        if (m_RunReplay)
        {

            // if the frame Counter has reached the m_NextInputFrames input time 
            if (m_FrameCounter == m_NextInputFrame.Frame)
            {
                OnInputReceived?.Invoke(m_NextInputFrame);
                m_NextInputFrame = GetFrame(++m_InputPosition); 
                if(m_NextInputFrame.EmptyInputFrame){
                    // stop the input if there is no more data; 
                    StopReplay();
                }
            }



            m_FrameCounter += 1; 
        }
    }


    private void ReceivedInput(InputFrame frame){
        Debug.Log("received frame: "+ frame.Frame); 
    }
    public InputFrame GetNextFrame()
    {
        var frame = m_ReplayLog.InputFrames[m_InputPosition];
        m_InputPosition += 1;
        return frame;
    }
    public InputFrame GetFrame(int frameNumber)
    {   
        if(frameNumber <0 || frameNumber >= m_ReplayLog.InputFrames.Count) return EMPTY_FRAME; 
        return m_ReplayLog.InputFrames[frameNumber];
    }
}
