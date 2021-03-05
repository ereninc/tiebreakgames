using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bomb : MonoBehaviour
{
    private int move;
    private float flow = 0f;
    // Start is called before the first frame update
    void Start()
    {
        flow = GridManager.instance._flow2;
        move = 10;
    }

    // Update is called once per frame
    void Update()
    {
        flow += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && flow >= 0.75f && GridManager.instance.player[0] != null)
        {
            move--;
            this.GetComponent<TextMeshProUGUI>().text = move.ToString();
            Debug.Log(transform.name + " " + move);
            if (move == 0)
            {
                GridManager.instance._gameStatus = 9;
            }
            flow = 0;
        }
    }
}