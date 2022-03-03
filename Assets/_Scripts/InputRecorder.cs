using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

///<summary> 
/// INputRecorder is the singleton that handles input 
///</summary> 
public class InputRecorder : MonoBehaviour
{
    public Action<InputFrame> OnInputRecorded;
    private int m_frameCounter = 0;
    private bool m_captureFrames = false;
    private InputData p1InputData;
    [SerializeField] private bool m_PrintLogs;
    public static InputRecorder Instance;

    private static string LogPath
    {
        get
        {
            return Application.persistentDataPath + Path.DirectorySeparatorChar + "InputLogs" + Path.DirectorySeparatorChar;
        }
    }
    private static string LogDictionaryPath
    {
        get
        {
            return Application.persistentDataPath + Path.DirectorySeparatorChar + "LogLookup.json";
        }
    }


    private LogLookup m_logDictionary;
    private InputLog m_activeLog;

    #region Monobehaviour LifeCycle
    private void Awake()
    {
        // The singleton pattern 
        if (InputRecorder.Instance == null) InputRecorder.Instance = this;
        else Destroy(this);
        Debug.Log(InputRecorder.Instance);

        LoadLogDictionary();
    }

    private void OnEnable()
    {
        this.OnInputRecorded += InputDataReceived;
    }
    private void OnDisable()
    {
        this.OnInputRecorded -= InputDataReceived;
    }
    private void OnDestroy()
    {
        SaveLogDictionary();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            BeginCapture();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            EndCapture();
        }
    }
    private void FixedUpdate()
    {
        bool p1Up = Input.GetButton("Up");
        bool p1Down = Input.GetButton("Down");
        bool p1Right = Input.GetButton("Right");
        bool p1Left = Input.GetButton("Left");
        bool p1Space = Input.GetButton("Space");

        p1InputData = new InputData(
            p1Up, p1Down, p1Left, p1Right, p1Space
        );

        InputFrame frame = new InputFrame(empty: !(p1Up || p1Down || p1Left || p1Right || p1Space),
                                        m_frameCounter, p1InputData);

        OnInputRecorded?.Invoke(frame);


        m_frameCounter++;

    }
    #endregion

    #region Public Methods
    public InputData getInputData()
    {
        return p1InputData;
    }
    public void BeginCapture()
    {
        m_captureFrames = true;
        m_frameCounter = 0;
        m_activeLog = new InputLog();
    }
    public void EndCapture()
    {
        m_captureFrames = false;
        m_activeLog.FrameCount = m_frameCounter;
        m_frameCounter = 0;
        Debug.Log("m_activeLog name" + m_activeLog.Name);
        SaveNewLog(m_activeLog);
    }

    public InputLog GetReplayLog(int logID)
    {
        var logName = MakeName(logID);
        if (m_logDictionary.Contains(logName))
        {
            Debug.Log("Log Found");
            return GetInputLog(GetLogPath(logName));

        }
        Debug.LogError("Log Not Found");
        return null;
    }

    public List<string> GetListOfLogs()
    {
        return m_logDictionary.Entries;
    }

    #endregion


    #region IO methods
    private void LoadLogDictionary()
    {
        //  creates the directory for the logs
        if (!Directory.Exists(LogPath))
        {
            Directory.CreateDirectory(LogPath);
        }


        if (File.Exists(LogDictionaryPath))
        {
            ConsoleLog("File Exists loading log dictioanry");
            var content = File.ReadAllText(LogDictionaryPath);
            m_logDictionary = JsonUtility.FromJson<LogLookup>(content);
        }
        else
        {
            ConsoleLog("File not found, making new file");
            m_logDictionary = new LogLookup();
        }
    }
    private void SaveLogDictionary()
    {
        var logDictionaryJSON = JsonUtility.ToJson(m_logDictionary);
        File.WriteAllText(LogDictionaryPath, logDictionaryJSON);
    }
    private void SaveNewLog(InputLog log)
    {
        Debug.Log("Saving To Dictionary");
        // saving a log means two things. 

        // add that file to the LogLookup. 
        m_logDictionary.Add(log);
        Debug.Log("Count: " + m_logDictionary.Entries.Count);

        // make a new log file.
        var jsonLog = JsonUtility.ToJson(log);

        File.WriteAllText(GetLogPath(log), jsonLog);
    }
    public string GetLogPath(InputLog log)
    {
        return LogPath + log.Name + ".json";
    }
    public string GetLogPath(string logName)
    {
        return LogPath + logName + ".json";
    }

    public string MakeName(int id)
    {
        return "InputLog_" + id.ToString();
    }
    public InputLog GetInputLog(string path)
    {
        var content = File.ReadAllText(path);
        InputLog log = JsonUtility.FromJson<InputLog>(content);
        return log;
    }

    #endregion

    #region Actions
    private void InputDataReceived(InputFrame frameData)
    {
        if (frameData.EmptyInputFrame) return;
        if (m_captureFrames)
        {
            ConsoleLog("OnInputDataReceived: Adding InputData data: " + frameData.ToString());
            m_activeLog.AddFrame(frameData);
        }
    }
    #endregion


    private void ConsoleLog(string text)
    {
        if (m_PrintLogs) Debug.Log(text);
    }

}
[Serializable]
public class LogLookup
{

    public int LogCounter = 0;
    public List<string> Entries;
    public LogLookup()
    {
        this.Entries = new List<string>();
    }
    public void Add(InputLog entry)
    {
        if (Entries.Contains(entry.Name)) return;

        if (string.IsNullOrWhiteSpace(entry.Name))
        {
            entry.Name = InputRecorder.Instance.MakeName(LogCounter++);
        }
        Entries.Add(entry.Name);
    }
    public void Remove(string name)
    {
        if (Entries.Contains(name)) Entries.Remove(name);
    }
    public bool Contains(string logID)
    {
        return Entries.Contains(logID);
    }


}
[Serializable]
public class InputLog
{
    public string Name;
    public int FrameCount;
    public List<InputFrame> InputFrames;
    public InputLog()
    {
        this.InputFrames = new List<InputFrame>();
    }
    public void AddFrame(InputFrame data)
    {
        this.InputFrames.Add(data);
    }

    public void PrintLog()
    {
        foreach (InputFrame frame in InputFrames)
        {
            Debug.Log(frame.ToString());
        }
    }
}
[Serializable]
public struct InputData
{
    public bool UpPressed;
    public bool DownPressed;
    public bool LeftPressed;
    public bool RightPressed;
    public bool SpacePressed;
    public InputData(bool up, bool down, bool left, bool right, bool space)
    {
        this.UpPressed = up;
        this.DownPressed = down;
        this.LeftPressed = left;
        this.RightPressed = right;
        this.SpacePressed = space;
    }

    public override string ToString()
    {

        return "UpPressed: " + this.UpPressed.ToString() +
                     "DownPressed: " + this.DownPressed.ToString() +
                     "LeftPressed: " + this.LeftPressed.ToString() +
                     "RightPressed: " + this.RightPressed.ToString() +
                     "SpacePressed: " + this.SpacePressed.ToString();
    }

    public InputData duplicate()
    {
        InputData duplicate = new InputData(this.DownPressed, this.DownPressed, this.LeftPressed, this.RightPressed, this.SpacePressed);
        return duplicate;
    }
}
[Serializable]
public struct InputFrame
{
    public bool EmptyInputFrame;
    public int Frame;
    public InputData P1;
    public InputFrame(bool empty, int frame, InputData p1)
    {
        this.EmptyInputFrame = empty;
        this.Frame = frame;
        this.P1 = p1;
    }
}

