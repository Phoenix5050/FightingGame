using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;


public class InputRecorder : MonoBehaviour
{
    public Action<InputData> OnActionRecorded;
    private int m_frameCounter = 0;
    private bool m_captureFrames = false;
    [SerializeField] private bool m_PrintLogs;
    public static InputRecorder Instance;

    private string LogPath
    {
        get
        {
            return Application.persistentDataPath + Path.DirectorySeparatorChar + "InputLogs" + Path.DirectorySeparatorChar;
        }
    }
    private string LogDictionaryPath
    {
        get
        {
            return Application.persistentDataPath + Path.DirectorySeparatorChar + "LogLookup.json";
        }
    }

    private LogLookup m_logDictionary;
    private InputLog m_activeLog;
    private int m_logCounter = 0;

    #region Monobehaviour LifeCycle
    private void Awake()
    {
        // The singleton pattern 
        if (InputRecorder.Instance != null) Destroy(this);
        else InputRecorder.Instance = this;

        LoadLogDictionary();
    }

    private void OnEnable()
    {
        this.OnActionRecorded += InputDataReceived;
    }
    private void OnDisable()
    {
        this.OnActionRecorded -= InputDataReceived;
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

        InputData inputData = new InputData(
            m_frameCounter,
            Input.GetButtonDown("Up"),
            Input.GetButtonDown("Down"),
            Input.GetButtonDown("Left"),
            Input.GetButtonDown("Right"),
            Input.GetButtonDown("Space")
        );


        OnActionRecorded?.Invoke(inputData);


        m_frameCounter++;

    }
    #endregion

    #region Public Methods
    public void BeginCapture()
    {
        m_captureFrames = true;
        m_frameCounter = 0;
        m_activeLog = new InputLog(
                        "Input_Log_" + (m_logCounter++).ToString(),
                         UnityEngine.Random.Range(Int16.MinValue, Int16.MaxValue).ToString());
        ConsoleLog("m_activeLog: " + m_activeLog.Name);
    }
    public void EndCapture()
    {
        m_captureFrames = false;
        m_frameCounter = 0;
        Debug.Log("m_activeLog name" + m_activeLog.Name);
        SaveNewLog(m_activeLog);
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
            m_logDictionary.LoadDictionaryFromLists();
        }
        else
        {
            ConsoleLog("File not found, making new file");
            m_logDictionary = new LogLookup();
        }
    }
    private void SaveLogDictionary()
    {
        m_logDictionary.PrepareDictionaryForSerialization();
        var logDictionaryJSON = JsonUtility.ToJson(m_logDictionary);
        Debug.Log(m_logDictionary.Entries.Count);
        foreach (var log in m_logDictionary.Entries)
        {
            Debug.Log("log: " + log.Key + log.Value.ToString());
        }
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
        File.WriteAllText(GetUniquePath(log), jsonLog);
    }
    private InputLog ReadInputLog(string path)
    {
        var content = File.ReadAllText(path);
        InputLog log = JsonUtility.FromJson<InputLog>(content);
        return log;
    }

    #endregion

    #region Actions
    private void InputDataReceived(InputData data)
    {

        if (m_captureFrames)
        {
            ConsoleLog("OnInputDataReceived: Adding InputData data: " + data.ToString());
            m_activeLog.AddFrame(data);
        }
    }
    #endregion
    public string GetUniquePath(InputLog log)
    {
        var hash = new Hash128();
        hash.Append(log.Name + log.Salt);
        return LogPath + hash.ToString() + ".json";
    }

    private void ConsoleLog(string text)
    {
        if (m_PrintLogs) Debug.Log(text);
    }





}
[Serializable]
public class LogLookup
{
    [SerializeField] private List<string> m_keys;
    [SerializeField] private List<LogID> m_values;
    public Dictionary<string, LogID> Entries;
    public LogLookup()
    {
        this.Entries = new Dictionary<string, LogID>();
    }
    public void Add(InputLog entry)
    {

        Debug.Log("Checking: " + entry.Name + ":" + this.Entries.ContainsKey(entry.Name));

        if (this.Entries.ContainsKey(entry.Name))
        {
            return;
        }
        var hashedName = InputRecorder.Instance.GetUniquePath(entry);
        var data = new LogID(entry.Name, entry.Salt, hashedName);
        this.Entries.Add(entry.Name, data);
    }
    public void Remove(string name)
    {
        if (this.Entries.ContainsKey(name))
        {
            this.Entries.Remove(name);
        }
    }

    public void PrepareDictionaryForSerialization()
    {
        m_keys = new List<string>();
        m_values = new List<LogID>();
        foreach (var pair in this.Entries)
        {
            m_keys.Add(pair.Key);
            m_values.Add(pair.Value);
        }
    }
    public void LoadDictionaryFromLists()
    {
        this.Entries = new Dictionary<string, LogID>();
        int size = Mathf.Min(m_keys.Count, m_values.Count);
        for (int i = 0; i < m_keys.Count; i++)
        {
            this.Entries.Add(m_keys[i], m_values[i]);
        }
    }
}
[Serializable]
public class InputLog
{
    public string Name
    {
        get { return this.id.Name; }
        set { this.id.Name = value; }
    }
    public string Salt
    {
        get { return this.id.Salt; }
        set { this.id.Name = value; }
    }
    LogID id;
    public List<InputData> InputFrames;
    public InputLog(string name, string salt)
    {
        this.InputFrames = new List<InputData>();
        this.id = new LogID(name, salt, "");
        this.Name = name;
        this.Salt = salt;
    }
    public void AddFrame(InputData data)
    {
        this.InputFrames.Add(data);
    }
    public void PrintLog()
    {
        foreach (InputData frame in InputFrames)
        {
            Debug.Log(frame.ToString());
        }
    }
}
[Serializable]
public struct InputData
{
    public int Frame;
    public bool UpPressed;
    public bool DownPressed;
    public bool LeftPressed;
    public bool RightPressed;
    public bool SpacePressed;
    public InputData(int frame, bool up, bool down, bool left, bool right, bool space)
    {
        this.Frame = frame;
        this.UpPressed = up;
        this.DownPressed = down;
        this.LeftPressed = left;
        this.RightPressed = right;
        this.SpacePressed = space;
    }

    public override string ToString()
    {

        return "Frame: " + this.Frame.ToString() +
                    "UpPressed: " + this.UpPressed.ToString() +
                     "DownPressed: " + this.DownPressed.ToString() +
                     "LeftPressed: " + this.LeftPressed.ToString() +
                     "RightPressed: " + this.RightPressed.ToString() +
                     "SpacePressed: " + this.SpacePressed.ToString();


    }
}
[Serializable]
public struct LogID
{
    public string Name;
    public string Salt;
    public string HashedName;
    public LogID(string name, string salt, string hashedName)
    {
        this.Name = name;
        this.Salt = salt;
        this.HashedName = hashedName;
    }
}
