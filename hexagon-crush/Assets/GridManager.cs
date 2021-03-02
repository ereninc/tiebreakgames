using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    private float _xOffset = 0.708f;
    private float _yOffset = 0.404f;
    [SerializeField]
    private GameObject[] hexs;
    [SerializeField]
    private Transform _tileMap;
    private Animator _myAnim;
    void Start()
    {
        generateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateMap()
    {
        float xval = 0;
        float yval = 0;
        int rand = Random.Range(0, 4);
        int[] rands;
        int index = 0;
        float delay = 0.02f;
        for (int y = 0; y < 10; y++)
        {
            xval = 0;
            for (int x = 0; x < 7; x++)
            {
                rand = Random.Range(0, 4);
                GameObject hex_go;
                if (x % 2 == 0)
                {
                    hex_go = Instantiate(hexs[rand], new Vector2(xval, yval), Quaternion.identity);
                    hex_go.name = "Hex_" + xval + "_" + yval;
                }
                else
                {
                    hex_go = Instantiate(hexs[rand], new Vector2(xval, (yval + _yOffset)), Quaternion.identity);
                    hex_go.name = "Hex_" + xval + "_" + (_yOffset + yval);
                }
                xval += _xOffset;
                index++;
                hex_go.transform.SetParent(_tileMap);
                _myAnim = hex_go.GetComponentInChildren<Animator>();
                StartCoroutine(CoroutineWithMultipleParameters( _myAnim,(index*delay)));
                //_myAnim.SetTrigger("Gen");
            }
            yval += _yOffset*2;
        }
    }
    public IEnumerator CoroutineWithMultipleParameters(Animator _myAnim,float delay)
    {
        yield return new WaitForSeconds(delay);

        // Insert your Play Animations here
        _myAnim.SetTrigger("Gen");
    }

}
