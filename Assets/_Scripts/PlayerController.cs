using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    SpriteRenderer squareSpriteRenderer;

    Transform squareTransform;

    public float movementSpeed = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        squareSpriteRenderer = GetComponent<SpriteRenderer>();
        squareSpriteRenderer.color = Color.green;

        squareTransform = GetComponent<Transform>();
    }
   
    public void SubscribeToInputRecorder(bool unSubsribe = false)
    {

        if (unSubsribe)
        {
            InputRecorder.Instance.OnInputRecorded -= ProcessInput;
        }
        else
        {
            InputRecorder.Instance.OnInputRecorded += ProcessInput;
        }
    }
    public void SubscribeToReplayRunner(ReplayRunner runner, bool unSubsribe = false){
        if(unSubsribe){
            runner.OnInputReceived -= ProcessInput;
        }
        else{
            runner.OnInputReceived += ProcessInput; 
        }
    }

    public void ProcessInput(InputFrame data)
    {
        if (data.EmptyInputFrame) return;

        if (data.P1.UpPressed)
        {
            squareTransform.position = new Vector2(squareTransform.position.x, transform.position.y + movementSpeed);
        }
        if (data.P1.DownPressed)
        {
            squareTransform.position = new Vector2(squareTransform.position.x, transform.position.y - movementSpeed);
        }
        if (data.P1.LeftPressed)
        {
            squareTransform.position = new Vector2(transform.position.x - movementSpeed, squareTransform.position.y);
        }
        if (data.P1.RightPressed)
        {
            squareTransform.position = new Vector2(transform.position.x + movementSpeed, squareTransform.position.y);
        }
        if (data.P1.SpacePressed)
        {
            if (squareSpriteRenderer.color == Color.green)
            {
                squareSpriteRenderer.color = Color.red;
            }
            else
            {
                squareSpriteRenderer.color = Color.green;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            // This command will be for testing rolling back
            print("return");
        }
    }
}