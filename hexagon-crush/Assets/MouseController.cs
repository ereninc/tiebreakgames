using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Start is called before the first frame update
    private float _rotationSpeed = 20f;
    private Vector3 _horizontalMovement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        _horizontalMovement = new Vector3(0f, 0f, -Input.GetAxis("Horizontal"));

        transform.Rotate(_horizontalMovement * _rotationSpeed * Time.deltaTime);



        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit)
        {
            Debug.Log("Raycast hit something!");
        }

    }
}
