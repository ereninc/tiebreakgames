using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update
    //for creating hexes
    private float _xOffset = 0.708f;
    private float _yOffset = 0.404f;
    //for creating sprites, where players click 
    private float _spriteOffsetX = 0.236f;
    private float _spriteOffsetY = 0.404f;
    [SerializeField]
    private GameObject[] hexs;
    [SerializeField]
    private Transform _tileMap;

    [SerializeField]
    private GameObject _Sprite;
    [SerializeField]
    private Transform _SpriteF1; //sagda 1 hex, solda 2 hex olan grup
    [SerializeField]
    private Transform _SpriteF2;//sagda 2 hex, solda 1 hex olan grup
    private Animator _myAnim;

    private int _gameStatus = 1; // 1 is active, 0 is locked(not playable) 2 is object selected 
    private GameObject[] player;    //holds selected hexes and it's sprite
    private Vector3 _lastMouseClick; //holds last mouse click location
    private Vector2[] _objectLocationsB;


    void Start()
    {
        player = new GameObject[4];
        generateMap();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                Debug.Log("Raycast hit something!" + hit.collider.name);
                if(hit.collider.name.Contains("Sprite"))
                {
                    _lastMouseClick = Input.mousePosition;
                    if (player[0] != null)
                    {
                        deActivate(player[1], player[2], player[3]);
                    }
                    player[0] = hit.collider.gameObject; //player'a seçilen sprite atılır
                    if (hit.collider.transform.parent.tag == "SpriteF1")
                    {
                        player = _getObjects(Input.mousePosition, 1, player);
                        activateF1(player[1], player[2], player[3]);
                        _gameStatus = 2;
                    }
                    else
                    {
                        player = _getObjects(Input.mousePosition, 2, player);
                        activateF2(player[1], player[2], player[3]);
                        _gameStatus = 2;
                    }
                }
                
            }
        }
        if(_gameStatus==2 && Input.GetKeyDown(KeyCode.Space))
        {
            
            deActivate(player[1], player[2], player[3]);
            _objectLocationsB = new Vector2[3];
            int x = 0;
            for (int i = 1; i < 4; i++)
            {
                _objectLocationsB[x++] = player[i].transform.position;
                player[i].transform.SetParent(player[0].transform);
            }
            _myAnim = player[0].GetComponentInChildren<Animator>();
            _myAnim.SetBool("sRotate120", true);
            StartCoroutine(Coroutine()); 
        }

    }

    void generateMap()
    {
        float xval = 0;
        float yval = 0;
        float spriteXval = 0;
        float spriteYval = _spriteOffsetY;
        int rand = Random.Range(0, 4);
        int index = 0;
        float delay = 0.02f;
        int flag = 0; //for sprite creation
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
                    hex_go.name = "Hex_" + y + "_" + x;
                }
                else
                {
                    hex_go = Instantiate(hexs[rand], new Vector2(xval, (yval + _yOffset)), Quaternion.identity);
                    hex_go.name = "Hex_" + y + "_" + ( x);
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
        for(int y = 0; y<18;y++)
        {
            flag = 0; // flag her satır için sıfırlandı
            spriteXval = _spriteOffsetX;
            if(y %2 == 1)
                spriteXval += _spriteOffsetX;
            for (int x = 0; x < 6; x++)
            {
                GameObject sprite_go;
                if (y%2==0) // sprite satırı tek ise patternler farklı, offset sayısının artımı
                {
                    
                    if (flag == 0)
                    {

                        sprite_go = Instantiate(_Sprite, new Vector2(spriteXval, spriteYval), Quaternion.identity);
                        spriteXval += (4 * _spriteOffsetX);
                        sprite_go.transform.SetParent(_SpriteF1);
                        flag++;
                    }
                    else
                    {

                        sprite_go = Instantiate(_Sprite, new Vector2(spriteXval, spriteYval), Quaternion.identity);
                        spriteXval += (2 * _spriteOffsetX);
                        sprite_go.transform.SetParent(_SpriteF2);
                        flag = 0;
                    }
                }
                else //çift ise
                {
                    if (flag == 0)
                    {

                        sprite_go = Instantiate(_Sprite, new Vector2(spriteXval, spriteYval), Quaternion.identity);
                        spriteXval += (2 * _spriteOffsetX);
                        sprite_go.transform.SetParent(_SpriteF2);
                        flag++;
                    }
                    else
                    {

                        sprite_go = Instantiate(_Sprite, new Vector2(spriteXval, spriteYval), Quaternion.identity);
                        spriteXval += (4 * _spriteOffsetX);
                        sprite_go.transform.SetParent(_SpriteF1);
                        flag = 0;
                    }
                }
                sprite_go.name = "Sprite" + y + "_" + x;


            }
            spriteYval += _spriteOffsetY;
        }
    }
    public IEnumerator CoroutineWithMultipleParameters(Animator _myAnim,float delay)
    {
        yield return new WaitForSeconds(delay);

        // Insert your Play Animations here
        _myAnim.SetTrigger("Gen");
    }

    void activateF1(GameObject midRight, GameObject topLeft, GameObject botLeft)
    {
        GameObject child;
 
        for (int i = 0; i<midRight.transform.childCount;i++)
        {
            if(midRight.transform.GetChild(i).name == "top" || midRight.transform.GetChild(i).name == "bot"|| midRight.transform.GetChild(i).name == "topRight"|| midRight.transform.GetChild(i).name == "botRight")
            {
                child = midRight.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }  
        }
        for (int i = 0; i < topLeft.transform.childCount; i++)
        {
            if (topLeft.transform.GetChild(i).name == "topRight" || topLeft.transform.GetChild(i).name == "top" || topLeft.transform.GetChild(i).name == "topLeft" || topLeft.transform.GetChild(i).name == "botLeft")
            {
                child = topLeft.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }
        }
        for (int i = 0; i < botLeft.transform.childCount; i++)
        {
            if (botLeft.transform.GetChild(i).name == "topLeft" || botLeft.transform.GetChild(i).name == "botLeft" || botLeft.transform.GetChild(i).name == "bot" || botLeft.transform.GetChild(i).name == "botRight")
            {
                child = botLeft.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }
        }
        
    }
    void activateF2(GameObject topRight, GameObject botRight, GameObject midLeft)
    {
        GameObject child;

        for (int i = 0; i < topRight.transform.childCount; i++)
        {
            if (topRight.transform.GetChild(i).name == "topLeft" || topRight.transform.GetChild(i).name == "top" || topRight.transform.GetChild(i).name == "topRight" || topRight.transform.GetChild(i).name == "botRight")
            {
                child = topRight.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }
        }
        for (int i = 0; i < botRight.transform.childCount; i++)
        {
            if (botRight.transform.GetChild(i).name == "topRight" || botRight.transform.GetChild(i).name == "botRight" || botRight.transform.GetChild(i).name == "bot" || botRight.transform.GetChild(i).name == "botLeft")
            {
                child = botRight.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }
        }
        for (int i = 0; i < midLeft.transform.childCount; i++)
        {
            if (midLeft.transform.GetChild(i).name == "top" || midLeft.transform.GetChild(i).name == "topLeft" || midLeft.transform.GetChild(i).name == "botLeft" || midLeft.transform.GetChild(i).name == "bot")
            {
                child = midLeft.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }
        }
    }

    void deActivate(GameObject a1, GameObject a2, GameObject a3)
    {
        GameObject child;
        for (int i = 0; i < a1.transform.childCount; i++)
        {
            child = a1.transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
        for (int i = 0; i < a2.transform.childCount; i++)
        {
            child = a2.transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
        for (int i = 0; i < a3.transform.childCount; i++)
        {
            child = a3.transform.GetChild(i).gameObject;
            child.SetActive(false);
        }

    }

    static  GameObject[] _getObjects(Vector3 mousePos, int type, GameObject[] objects) // sagda 1hex solda 2 hex için input "1", sagda 2 hex, solda 1 hex için input "2"
    {
        if(type == 1)
        {
            objects[1] = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(90f, 0, 0)), Vector2.zero).transform.gameObject;
            objects[2] = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(-50f, 70f, 0)), Vector2.zero).transform.gameObject;
            objects[3] = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(-50f, -70f, 0)), Vector2.zero).transform.gameObject;
            
        }
        else
        {
            objects[3] = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(-90f, 0, 0)), Vector2.zero).transform.gameObject;
            objects[1] = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(+50f, 70f, 0)), Vector2.zero).transform.gameObject;
            objects[2] = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(+50f, -70f, 0)), Vector2.zero).transform.gameObject;
        }
        return objects;
    }
    public IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 1; i < 4; i++)
        {
            player[i].transform.SetParent(_tileMap);
        }

        
        if (player[0].transform.parent.name == "SpriteF1")
        {
            player[3].transform.position = _objectLocationsB[0];
            player[1].transform.position = _objectLocationsB[1];
            player[2].transform.position = _objectLocationsB[2];

            player = _getObjects(_lastMouseClick, 1, player);
            activateF1(player[1], player[2], player[3]);
        }
        else
        {
            player = _getObjects(_lastMouseClick, 2, player);
            activateF2(player[1], player[2], player[3]);
        }
        _myAnim.SetBool("sRotate120",false);
    }


}
