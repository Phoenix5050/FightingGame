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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            squareTransform.position = new Vector2 (squareTransform.position.x, transform.position.y + movementSpeed);
        }
        if (Input.GetKey("down"))
        {
            squareTransform.position = new Vector2(squareTransform.position.x, transform.position.y - movementSpeed);
        }
        if (Input.GetKey("right"))
        {
            squareTransform.position = new Vector2(transform.position.x + movementSpeed, squareTransform.position.y);
        }
        if (Input.GetKey("left"))
        {
            squareTransform.position = new Vector2(transform.position.x - movementSpeed, squareTransform.position.y);
        }
        if (Input.GetKeyDown("space"))
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
        if (Input.GetKeyDown("return"))
        {
            // This command will be for testing rolling back
            print("return");
        }
    }
}