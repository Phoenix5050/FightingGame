using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // Amount of input delay to the player in frames
    [SerializeField] private int inputDelay = 0;
    // Number of frame states to save to implement rollback (max number of frames able to rollback)
    [SerializeField] private int rollback = 7;

    // list to hold 256 inputs for use in rollback
    private List<InputData> inputArray = new List<InputData>();

    // queue for FrameStates of past frames (for rollback)
    private List<FrameState> stateQueue = new List<FrameState>();

    // tracker for current frame
    private int frameNum = 0;

    // for use in getGameState() when fetching child game objects and adding to stateQueue (saving computation resources making here)
    private Dictionary<string, Dictionary<string, Variable>> upperDict;
    /*
     * Game state variable
     * A dictionary of dictionaries where the upper dictionary uses tags to identify child objects in the game as a key,
     * with values being dictionaries belonging to said child objects detailing in-game information
     * 
     * Child dictionaries utilize keys as variable names (tranform.x, transform.y, colour, etc.) and values as said variable's value on the child.
     * 
     */
    Dictionary<string, Dictionary<string, Variable>> gameState = new Dictionary<string, Dictionary<string, Variable>>();
    
    [Space]
    [Header("References")]
    [SerializeField] private InputRecorder inputRecorder;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;

        // initialize input array
        for (int i = 0; i < 256; i++)
        {
            // inputFrame takes true as first argument to indicate no inputs are in InputData, frame 0
            inputArray.Add(new InputData());
        }

        // initialize state queue
        for (int i = 0; i < rollback; i++)
        {
            // empty input, frame 0, initial gamestate
            stateQueue.Add(new FrameState (new InputData(), 0, getGameState()));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // save current gamestate before recording any inputs
        Dictionary<string, Dictionary<string, Variable>> preGameState = getGameState();
        
        frameStartAll();
        
        // save current inputs to array of inputFrames, adding input delay,
        // so when client recieves input array for rollback, games remain in sync
        // TODO: perhaps make a function in input recorder that isolates relevant parts in fixedUpdate
        // and call function to ensure input is recorded before this
        inputArray[(frameNum + inputDelay) % 256] = inputRecorder.getInputData();

        // Get input from (inputDelay) frames ago to use as current input since input delay time is up
        InputData currentInput = inputArray[frameNum].duplicate();

        // reset state (rollback) according to new inputs from client here
        // resetStateAll(stateQueue[0].gameState);

        inputUpdateAll(currentInput);
        frameEndAll();

        // store current framestate into queue 
        stateQueue.Add(new FrameState(currentInput, 0, preGameState));

        //remove oldest state from queue
        stateQueue.RemoveAt(0);

        // increment current frameNum cycle
        frameNum = (frameNum + 1) % 256;
    }

    
    #region children function calls
    // Child state managers are gotten in each method as opposed to being sent as an argument since children can be deleted during rollback
    public void frameStartAll()
    {
        ChildStateManager[] childStateManagers = GetComponentsInChildren<ChildStateManager>();
        foreach(ChildStateManager childStateManager in childStateManagers)
            childStateManager.FrameStart();
    }

    public void resetStateAll(Dictionary<string, Dictionary<string, Variable>> newGameState)
    {
        ChildStateManager[] childStateManagers = GetComponentsInChildren<ChildStateManager>();
        // TODO: test deleting gameobjects later
        foreach(ChildStateManager childStateManager in childStateManagers)
            childStateManager.resetState(newGameState);
    }

    public void inputUpdateAll(InputData input)
    {
        ChildStateManager[] childStateManagers = GetComponentsInChildren<ChildStateManager>();
        foreach(ChildStateManager childStateManager in childStateManagers)
            childStateManager.inputUpdate(input);
    }

    public void frameEndAll()
    {
        ChildStateManager[] childStateManagers = GetComponentsInChildren<ChildStateManager>();
        foreach(ChildStateManager childStateManager in childStateManagers)
            childStateManager.frameEnd();
    }
    #endregion

    // Iterate through each child of this class and get their state dictionaries to form the game state dictionary
    public Dictionary<string, Dictionary<string, Variable>> getGameState()
    {
        upperDict = new Dictionary<string, Dictionary<string, Variable>>();

        for (int i = 0; i < transform.childCount; i++)
        {
            // Get child
            Transform child = transform.GetChild(i);

            // Get child state dictionary
            Dictionary<string, Variable> lowerDict = child.gameObject.GetComponent<ChildStateManager>().getState();

            // Add to game state dictionary
            upperDict.Add(child.name, lowerDict);
        }
        return upperDict;
    }

    // This command is for testing rollback
    public void test()
    {
        resetStateAll(stateQueue[0].gameState);
    }
}

public class FrameState
{
    public InputData inputs;
    public int frame;
    // inputFrame also contains frame number
    // frame number is accessed via inputs.frame
    // inputData is accessed via inputs.inputData
    public Dictionary<string, Dictionary<string, Variable>> gameState;

    public FrameState(InputData newInputs, int newFrame, Dictionary<string, Dictionary<string, Variable>> newGameState)
    {
        inputs = newInputs;
        frame = newFrame;
        gameState = newGameState;
    }
}

// maybe its this
public class Variable {
    public object value { get; set; }
}