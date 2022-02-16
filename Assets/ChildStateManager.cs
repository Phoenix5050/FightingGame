using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildStateManager : MonoBehaviour
{
    SpriteRenderer squareSpriteRenderer;

    Transform squareTransform;

    public int rollback = 7;

    Dictionary<string, Variable> state;


    // Start is called before the first frame update
    void Start()
    {
        squareSpriteRenderer = GetComponent<SpriteRenderer>();
        squareSpriteRenderer.color = Color.green;

        squareTransform = GetComponent<Transform>();

        // initialize state dictionary
        state = new Dictionary<string, Variable>();

        state.Add("x", new Variable());
        state.Add("y", new Variable());

        state.Add("colour", new Variable());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateState()
    {
        // Finally, after all changes have been made for the frame, this function will be called to update variables for state dictionary
        // left off here, change "Variable" data type to something generic for multiple data types. Maybe try Dictionary<string, object> and cast?

        // x and y coordinates
        state["x"].value = squareTransform.position.x;
        state["y"].value = squareTransform.position.y;

        //colour
        state["colour"].value = squareSpriteRenderer.color;
    }
    // reset object state to given newState state dictionary. Executed once per rollback
    public void resetState(Dictionary<string, Dictionary<string, Variable>> newState)
    {
        // check if this object exists within the checked game state
        if (newState.ContainsKey(this.gameObject.name))
        {
            // update this gameobject's paramaters to be that of the new gamestate
            float newX = (float)newState[this.gameObject.name]["x"].value;
            float newY = (float)newState[this.gameObject.name]["y"].value;
            squareTransform.position = new Vector2(newX, newY);

            squareSpriteRenderer.color = (Color)newState[this.gameObject.name]["colour"].value;
        }
        else
        {
            // free gameobject and delete from memory, since it is not in the new gamestate
            Destroy(this.gameObject);
        }
    }

    // Return the state of this game object back to gameStateManager script
    public Dictionary<string, Variable> getState()
    {
        return state;
    }
}
