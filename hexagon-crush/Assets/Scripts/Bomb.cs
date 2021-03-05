using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int move;
    // Start is called before the first frame update
    void Start()
    {
        move = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            move--;
            Debug.Log(transform.name + " " + move);
            if (move == 0)
            {
                GridManager.instance._gameStatus = 9;
            }
        }
    }
}