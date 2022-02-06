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
    private void OnEnable()
    {
        InputRecorder.Instance.OnActionRecorded += ProcessInput;
    }
    private void OnDisable()
    {
        InputRecorder.Instance.OnActionRecorded -= ProcessInput;
    }
    private void ProcessInput(InputData data)
    {
        if (data.UpPressed)
        {
            squareTransform.position = new Vector2(squareTransform.position.x, transform.position.y + movementSpeed);
        }
        if (data.DownPressed)
        {
            squareTransform.position = new Vector2(squareTransform.position.x, transform.position.y - movementSpeed);
        }
        if (data.LeftPressed)
        {
            squareTransform.position = new Vector2(transform.position.x - movementSpeed, squareTransform.position.y);
        }
        if (data.RightPressed)
        {
            squareTransform.position = new Vector2(transform.position.x + movementSpeed, squareTransform.position.y);
        }
        if (data.SpacePressed)
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