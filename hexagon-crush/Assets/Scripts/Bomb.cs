using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bomb : MonoBehaviour
{
    private int move;
    private float flow = 0f;
    private Touch _touch;
    private Vector2 _touchPosStart, _touchPosEnd;
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
        TouchInput();
    }

    private void TouchInput()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {
                _touchPosStart = _touch.position;
            }
            else if (_touch.phase == TouchPhase.Moved || _touch.phase == TouchPhase.Ended)
            {
                _touchPosEnd = _touch.position;
                float x = _touchPosEnd.x - _touchPosStart.x;
                float y = _touchPosEnd.y - _touchPosStart.y;
                if (_touch.phase == TouchPhase.Began)
                {
                    _touchPosStart = _touch.position;
                }
                //TURN SELECTED OBJECTS
                else if (Mathf.Abs(x) > Mathf.Abs(y) && flow >= 0.75f && GridManager.instance.player[0] != null)
                {
                    //GetNeighboors and turn clockwise or counter-clockwise.
                    if (x > 0)
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
                    else
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
        }
    }
}