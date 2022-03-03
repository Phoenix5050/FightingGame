using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildStateManager : MonoBehaviour
{
    SpriteRenderer squareSpriteRenderer;

    Transform squareTransform;

    public int rollback = 7;

    // Contains the current gamestate of the child object
    Dictionary<string, Variable> state;

    //!TODO : Make   a file for constants
    public static string X_POS = "x";
    public static string Y_POS = "y";
    public static string COLOUR = "colour"; 

    // Start is called before the first frame update
    void Awake()
    {
        squareSpriteRenderer = GetComponent<SpriteRenderer>();
        squareSpriteRenderer.color = Color.green;

        squareTransform = GetComponent<Transform>();

        // initialize state dictionary
        newState();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // Finally, after all changes have been made for the frame, this function will be called to update variables for state dictionary
        updateState();
    }

    // initializes state dictionary
    public void newState()
    {
        state = new Dictionary<string, Variable>();

        state.Add(X_POS, new Variable());
        state.Add(Y_POS, new Variable());

        state.Add(COLOUR, new Variable());
    }

    public void updateState()
    {
        // perhaps this can be changed to only have 7 states made and used in stateManager instead of a new state every frame
        newState();
        // x and y coordinates
        state[X_POS].value = squareTransform.position.x;
        state[Y_POS].value = squareTransform.position.y;

        //colour
        state[COLOUR].value = squareSpriteRenderer.color;
    }

    // reset object state to given newState state dictionary. Executed once per rollback
    public void resetState(Dictionary<string, Dictionary<string, Variable>> newState)
    {
        // check if this object exists within the checked game state
        if (newState.ContainsKey(this.gameObject.name))
        {
            // update this gameobject's paramaters to be that of the new gamestate
            float newX = (float)newState[this.gameObject.name][X_POS].value;
            float newY = (float)newState[this.gameObject.name][Y_POS].value;
            
            squareTransform.position = new Vector2(newX, newY);

            squareSpriteRenderer.color = (Color)newState[this.gameObject.name][COLOUR].value;
        }
        else
        {
            // free gameobject and delete from memory, since it is not in the new gamestate
            Destroy(this.gameObject);
        }
    }

    // code to run at beginning of rollback frame
    public void FrameStart()
    {

    }

    // calculate the state of the object for the given input
    // this is where movement and collision detection should be
    // may be called multiple times per frame
    // TODO: Merge with existing playercontroller script
    public void inputUpdate(InputData input)
    {
        
    }

    // code to run at end of rollback frame
    public void frameEnd()
    {

    }

    public Dictionary<string, Variable> getState()
    {
        return state;
    }
}