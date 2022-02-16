using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public int rollback = 7;

    // queue for FrameStates of past frames (for rollback)
    public Queue<FrameState> stateQueue = new Queue<FrameState>();

    // tracker for current frame
    private int frameNum = 0;

    /*
     * Game state variable
     * A dictionary of dictionaries where the upper dictionary uses tags to identify child objects in the game as a key,
     * with values being dictionaries belonging to said child objects detailing in-game information
     * 
     * Child dictionaries utilize keys as variable names (tranform.x, transform.y, colour, etc.) and values as said variable's value on the child.
     * 
     */
    Dictionary<string, Dictionary<string, Variable>> gameState = new Dictionary<string, Dictionary<string, Variable>>();
    

    // Start is called before the first frame update
    void Awake()
    {
        // initialize state queue
        for (int i = 0; i < rollback; i++)
        {
            // empty input, frame 0, initial gamestate
            stateQueue.Enqueue(new FrameState (0, getGameState()));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // save current gamestate before recording any inputs
        Dictionary<string, Dictionary<string, Variable>> preGameState = getGameState();

        // frameStartAll

        // do some input processing stuff stuff

        // todo: test restting of state
        
        // frameUpdateAll

        // frameEndAll

        // store current framestate into queue
        stateQueue.Enqueue(new FrameState(frameNum, getGameState()));

        //remove oldest state from queue
        stateQueue.Dequeue();

        frameNum = (frameNum + 1) % 256; // increment current frameNum cycle
    }

    // Iterate through each child of this class and get their state dictionaries to form the game state dictionary
    public Dictionary<string, Dictionary<string, Variable>> getGameState()
    {
        var upperDict = new Dictionary<string, Dictionary<string, Variable>>();

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            // Get child
            Transform child = this.gameObject.transform.GetChild(i);

            // Get child state dictionary
            Dictionary<string, Variable> lowerDict = child.gameObject.GetComponent<ChildStateManager>().getState();

            // Add to game state dictionary
            upperDict.Add(child.name, lowerDict);
        }
        return upperDict;
    }
}


public class FrameState
{
    // add variable for input
    private int frame;
    private Dictionary<string, Dictionary<string, Variable>> gameState;

    public FrameState(int newFrame, Dictionary<string, Dictionary<string, Variable>> newGameState)
    {
        frame = newFrame;
        gameState = newGameState;
    }
}

public class Variable {
    public object value { get; set; }
}