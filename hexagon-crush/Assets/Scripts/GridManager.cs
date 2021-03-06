﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject[] _SpriteArray;

    [SerializeField]
    private GameObject _hexFall;


    private Animator _myAnim;

    public int _gameStatus = 3; // 1 is active, 0 is locked(not playable) 2 is object selected, 3 is screen loading
    public GameObject[] player;    //holds selected hexes and it's sprite
    private Vector3 _lastMouseClick; //holds last mouse click location
    private Vector2[] _objectLocationsB;
    private float _flow = 0f;
    public float _flow2 = 0f;
    private float _flow3 = 0f;
    [SerializeField] private GameObject _explodeFX;
    private bool isBoom = false;
    private int _score = 0;
    public bool panelActive = true; //game starting panel
    public static GridManager instance;
    [SerializeField] Text _scoreText;
    [SerializeField] GameObject _panel;
    private int _scoreBomb = 0;

    private Touch _touch;
    private Vector2 _touchPosStart, _touchPosEnd;

    public static bool isGameOver = false;

    AudioSource audioSource;
    public AudioClip audioClip;

    public AudioSource audioSource2;
    public AudioClip audioClip2;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Screen.SetResolution(1080, 1920, true);
        player = new GameObject[4];
        _SpriteArray = new GameObject[108];
        instance = this;
        generateMap();
        audioSource2.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameStatus == 9)
        {
            if (player[0] != null)
            {
                deActivate(player[1], player[2], player[3]);
                _playerReset();
            }
        }
        if(_gameStatus != 3 && _gameStatus != 9)
        {
            _flow += Time.deltaTime;
            _flow2 += Time.deltaTime;
            _flow3 += Time.deltaTime;
            TouchInput();

            if (!isBoom && _gameStatus == 1 && _flow3>=0.8f)
            {
                blowHexes();
            }
            if (_flow3 >= 0.9f && !isBoom && panelActive)
            {
                _panel.GetComponentInChildren<Animator>().SetTrigger("panelDeactive");
                panelActive = false;
                audioSource2.volume = 1f;
                _score = 0;
                _scoreText.text = _score.ToString();
            }
        }
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

                if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0 && _flow >= 0.45f && _gameStatus == 2) //TAPPED! (select hexagons)
                {
                    Rayc(Input.mousePosition, 1);
                    _lastMouseClick = Input.mousePosition;
                    _gameStatus = 2;
                }
                //TURN SELECTED OBJECTS
                else if (Mathf.Abs(x) > Mathf.Abs(y) && _gameStatus == 2 && _flow2 >= 0.45f && player[0] != null)
                {
                    //GetNeighboors and turn clockwise or counter-clockwise.
                    if (x > 0)
                    {
                        _gameStatus = 0;
                        Rotate();
                        audioSource.PlayOneShot(audioClip);
                    }
                    else
                    {
                        /*_gameStatus = 0;
                        Rotate();*/
                    }
                }

                else if (Mathf.Abs(x) < Mathf.Abs(y) && _gameStatus == 2 && _flow2 >= 0.45f && player[0] != null)
                {
                    //GetNeighboors and turn clockwise or counter-clockwise.
                    if (y > 0)
                    {
                        _gameStatus = 0;
                        Rotate();
                        audioSource.PlayOneShot(audioClip);
                    }
                    else
                    {
                        /*_gameStatus = 0;
                        Rotate();*/
                    }
                }
            }
        }
    }


    void generateMap()
    {
        
        int spriteIndex = 0;
        float xval = 0;
        float yval = 0;
        float spriteXval = 0;
        float spriteYval = _spriteOffsetY;
        int rand = 0;
        int index = 0;
        float delay = 0.02f;
        int flag = 0; //for sprite creation
        for (int y = 0; y < 10; y++)
        {
            xval = 0;
            for (int x = 0; x < 7; x++)
            {
                rand = Random.Range(0,6);
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
                _SpriteArray[spriteIndex++] = sprite_go;

            }
            spriteYval += _spriteOffsetY;
        }
        _gameStatus = 1;
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
            if(child.name != "Canvas")
                child.SetActive(false);
        }
        for (int i = 0; i < a2.transform.childCount; i++)
        {
            child = a2.transform.GetChild(i).gameObject;
            if (child.name != "Canvas")
                child.SetActive(false);
        }
        for (int i = 0; i < a3.transform.childCount; i++)
        {
            child = a3.transform.GetChild(i).gameObject;
            if (child.name != "Canvas")
                child.SetActive(false);
        }

    }

    static  GameObject[] _getObjects(Vector3 mousePos, int type, GameObject[] objects) // sagda 1hex solda 2 hex için input "1", sagda 2 hex, solda 1 hex için input "2"
    {
        RaycastHit2D hit;
        if(type == 1)
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(90f, 0, 0)), Vector2.zero);
            if(hit.collider != null) { objects[1] = hit.collider.transform.gameObject; }

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(-50f, 70f, 0)), Vector2.zero);
            if(hit.collider != null) { objects[2] = hit.collider.transform.gameObject; }

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(-50f, -70f, 0)), Vector2.zero);
            if (hit.collider != null) { objects[3] = hit.collider.transform.gameObject; }

        }
        else
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(-90f, 0, 0)), Vector2.zero);
            if (hit.collider != null) { objects[3] = hit.collider.transform.gameObject; }

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(+50f, 70f, 0)), Vector2.zero);
            if (hit.collider != null) { objects[1] = hit.collider.transform.gameObject; }

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos + new Vector3(+50f, -70f, 0)), Vector2.zero);
            if (hit.collider != null) { objects[2] = hit.collider.transform.gameObject; }
        }
        return objects;
    }

    private void Rotate()
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
        StartCoroutine(RotateRoutine(_myAnim));
    }

    private void Rayc(Vector3 mouse , int type) // 1 for acting like player click, 2 for using in code without activating hex edges
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mouse), Vector2.zero);

        if (hit)
        {
            if (hit.collider.name.Contains("Sprite"))
            {
                //_lastMouseClick = mouse;
                if (type == 1 && player[0] !=null && player[1] != null && player[2] != null && player[3] != null)
                {
                    deActivate(player[1], player[2], player[3]);
                }
                player[0] = hit.collider.gameObject; //player'a seçilen sprite atılır
                if (hit.collider.transform.parent.tag == "SpriteF1")
                {
                    player = _getObjects(mouse, 1, player);
                    if(type == 1)
                    {
                        activateF1(player[1], player[2], player[3]);
                        _gameStatus = 2;
                    }
                    
                }
                else
                {
                    player = _getObjects(mouse, 2, player);
                    if( type == 1)
                    {
                        activateF2(player[1], player[2], player[3]);
                        _gameStatus = 2;
                    }
                    
                }
            }

        }
        _flow = 0;
    }


    public IEnumerator RotateRoutine(Animator an)
    {
        
        RuntimeAnimatorController ac = an.runtimeAnimatorController;
        float time=0f;
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == "SpriteRotate" || ac.animationClips[i].name == "New State")        //If it has the same name as your clip
            {
                time = ac.animationClips[i].length;
            }
        }
        yield return new WaitForSeconds(time);
        if (player[0] != null)
        {
            for (int i = 1; i < 4; i++)
            {
                player[i].transform.SetParent(_tileMap);
            }
            if (player[0].transform.parent.name == "SpriteF1")
            {
                player[3].transform.position = _objectLocationsB[0];
                player[1].transform.position = _objectLocationsB[1];
                player[2].transform.position = _objectLocationsB[2];
            }
            else
            {
                player[2].transform.position = _objectLocationsB[0];
                player[3].transform.position = _objectLocationsB[1];
                player[1].transform.position = _objectLocationsB[2];
            }

            an.SetBool("sRotate120", false);
            _flow2 = 0;
            blowHexes();
        }
            
    }

  


    private void blowHexes()
    {
        Vector3 screenPos;
        if(player[0] != null)
            deActivate(player[1], player[2], player[3]);
        for (int i = 0; i< _SpriteArray.Length;i++) //spritelar içinde dönülüyor
        {
            screenPos = Camera.main.WorldToScreenPoint(_SpriteArray[i].transform.position);//world location alınıyor
            Rayc(screenPos,2);    //Raycast atıp player değişkeni dolduruluyor
            if(player[0] != null)  //hata kontrolü
            {
                if (player[1] != null && player[2] != null && player[3] != null)
                    if (player[1].transform.tag == player[2].transform.tag && player[1].transform.tag == player[3].transform.tag)  //hexlerin tagleri aynı ise
                    {
                        GameObject expFX = Instantiate(_explodeFX, player[0].transform.position, Quaternion.identity);
                        Destroy(expFX, 2f);
                        _score += 30;
                        if (panelActive == false)
                        {
                            _scoreBomb += 30;
                        }
                        _scoreText.text = _score.ToString();
                        isBoom = true;
                        audioSource2.PlayOneShot(audioClip2);
                        break;
                    }
            }
            _playerReset();
        }
        if (isBoom)
        {
            fallHexes();
        }
        else
        {
            Rayc(_lastMouseClick, 1);
            _gameStatus = 2;
        }

    }

    private void fallHexes()
    {
        int bomb = 9;
        if (_scoreBomb >= 120)
        {
            bomb = Random.Range(0, 1);
            _scoreBomb = 0;
        }
        float y = 155f;//115 -195     275
        Vector3[] temp = new Vector3[3];
        RaycastHit2D hit;
        List<GameObject> fallingHexes = new List<GameObject>();
        List<GameObject> fallingHexes2 = new List<GameObject>();

        List<Vector3> fallingHexesOldLocations = new List<Vector3>();
        List<Vector3> fallingHexes2OldLocations = new List<Vector3>();
        for (int j = 0; j < 3; j++)
        {
            temp[j] = Camera.main.WorldToScreenPoint(player[j + 1].transform.position);
            //Destroy(player[j + 1]);
        }
        if (player[0].transform.parent.name == "SpriteF1")   //pattern kontrol
        {
            for(int i=0;i<10;i++)
            {
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(temp[0]+ new Vector3(0,y,0)), Vector2.zero);
                if(hit.collider != null)
                {
                    fallingHexes.Add(hit.transform.gameObject);
                }
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(temp[1] + new Vector3(0, y, 0)), Vector2.zero);
                if (hit.collider != null)
                {
                    fallingHexes2.Add(hit.transform.gameObject);
                }
                y += 155f;   
            }

            //new hex spawning
            if (fallingHexes.Count == 0)
                if (bomb == 0)  //if de bomba-else de hex spawn.
                    fallingHexes.Add(Instantiate(hexs[Random.Range(7, 11)], player[1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
                else
                    fallingHexes.Add(Instantiate(hexs[Random.Range(0, 6)], player[1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
            else
                if (bomb == 0)
                    fallingHexes.Add(Instantiate(hexs[Random.Range(7, 11)], fallingHexes[fallingHexes.Count - 1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
                else
                    fallingHexes.Add(Instantiate(hexs[Random.Range(0, 6)], fallingHexes[fallingHexes.Count - 1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));

            if (fallingHexes2.Count == 0)
            {
                if (bomb == 1)
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(7, 11)], player[3].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                else
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], player[3].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], player[2].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
            }
            else
            {
                if (bomb == 1)
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(7, 11)], fallingHexes2[fallingHexes2.Count - 1].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                else
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], fallingHexes2[fallingHexes2.Count - 1].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], fallingHexes2[fallingHexes2.Count - 1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
                
            }

            fallingHexes2OldLocations.Add(player[3].transform.position);
            fallingHexes2OldLocations.Add(player[2].transform.position);
            fallingHexesOldLocations.Add(player[1].transform.position);

        }
        else        //hex pattern 2 which means 2 hexes on right, 1 hex on left
        {
            for (int i = 0; i < 10; i++)
            {
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(temp[0] + new Vector3(0, y, 0)), Vector2.zero);
                if (hit.collider != null)
                {
                    fallingHexes2.Add(hit.transform.gameObject);
                    //temp[0] = Camera.main.ScreenToWorldPoint(hit.transform.position);
                }
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(temp[2] + new Vector3(0, y, 0)), Vector2.zero);
                if (hit.collider != null)
                {
                    fallingHexes.Add(hit.transform.gameObject);
                    //temp[0] = Camera.main.ScreenToWorldPoint(hit.transform.position);
                }
                y += 155f;
            }

            if (fallingHexes.Count == 0)
                if(bomb == 0)
                    fallingHexes.Add(Instantiate(hexs[Random.Range(7, 11)], player[3].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
                else
                    fallingHexes.Add(Instantiate(hexs[Random.Range(0, 6)], player[3].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
            else
                if(bomb == 0)
                    fallingHexes.Add(Instantiate(hexs[Random.Range(7, 11)], fallingHexes[fallingHexes.Count - 1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
                else
                    fallingHexes.Add(Instantiate(hexs[Random.Range(0, 6)], fallingHexes[fallingHexes.Count - 1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));




            if (fallingHexes2.Count == 0)
            {
                if(bomb == 1)
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(7, 11)], player[2].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                else
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], player[2].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], player[1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
            }
            else
            {
                if(bomb == 1)
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(7, 11)], fallingHexes2[fallingHexes2.Count - 1].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                else
                    fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], fallingHexes2[fallingHexes2.Count - 1].transform.position + new Vector3(0, 1.616f, 0), Quaternion.identity));
                fallingHexes2.Add(Instantiate(hexs[Random.Range(0, 6)], fallingHexes2[fallingHexes2.Count - 1].transform.position + new Vector3(0, 0.808f, 0), Quaternion.identity));
            }

            fallingHexes2OldLocations.Add(player[2].transform.position);
            fallingHexes2OldLocations.Add(player[1].transform.position);
            fallingHexesOldLocations.Add(player[3].transform.position);

        }


        
        for (int i = 0; i < fallingHexes.Count; i++)
        {
            fallingHexes[i].transform.SetParent(_hexFall.transform);
            fallingHexesOldLocations.Add(fallingHexes[i].transform.position);
        }
        for(int i = 0; i<fallingHexes2.Count;i++)
        {
            fallingHexes2[i].transform.SetParent(_hexFall.transform);
            fallingHexes2OldLocations.Add( fallingHexes2[i].transform.position);
        }

        _hexFall.GetComponent<Animator>().SetBool("HexFall", true);
        for (int j = 0; j < 3; j++)
        {
            player[j + 1].SetActive(false);
        }
        StartCoroutine(FallRoutine(_hexFall.GetComponent<Animator>(), temp, fallingHexes, fallingHexes2, fallingHexesOldLocations, fallingHexes2OldLocations));
        
    }
    public IEnumerator FallRoutine(Animator an, Vector3[] temp, List<GameObject> fallingHexes, List<GameObject> fallingHexes2, List<Vector3> fallingHexesOldLocations, List<Vector3> fallingHexes2OldLocations)
    {
        Vector3 temp2;
        RuntimeAnimatorController ac = an.runtimeAnimatorController;
        float time = 0f;
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == "hexIdle" || ac.animationClips[i].name == "hexFall")        //If it has the same name as your clip
            {
                time = ac.animationClips[i].length;
            }
        }
        yield return new WaitForSeconds(0.25f);
        
        if (player[0].transform.parent.name == "SpriteF1")   //pattern kontrol
        {
            temp[0] = player[1].transform.position;  //ground of falling transfered

            for (int i = 0; i < fallingHexes.Count; i++) // right side hexes falled
            {
                temp2 = fallingHexesOldLocations[i];
                fallingHexes[i].transform.position = temp2;
                fallingHexes[i].transform.SetParent(_tileMap);
                temp[0] = temp2;
            }



            for(int i = 1;i>=0;i--)
            {
                for (int j = i; j < fallingHexes2.Count; j++) // right side hexes falled
                {
                    temp2 = fallingHexes2OldLocations[j];
                    fallingHexes2[j].transform.position = temp2;
                    fallingHexes2[j].transform.SetParent(_tileMap);
                    temp[1] = temp2;
                }
            }


        }
        else
        {
            temp[0] = player[3].transform.position;  //ground of falling transfered
            for (int i = 0; i < fallingHexes.Count; i++) // right side hexes falled
            {
                temp2 = fallingHexesOldLocations[i];
                fallingHexes[i].transform.position = temp2;
                fallingHexes[i].transform.SetParent(_tileMap);
                temp[0] = temp2;
            }

            for (int i = 1; i >= 0; i--)
            {
                for (int j = i; j < fallingHexes2.Count; j++) // right side hexes falled
                {
                    temp2 = fallingHexes2OldLocations[j];
                    fallingHexes2[j].transform.position = temp2;
                    fallingHexes2[j].transform.SetParent(_tileMap);
                    temp[1] = temp2;
                }
            }
        }
        for (int j = 0; j < 3; j++)
        {
            Destroy(player[j + 1]);
        }
        _playerReset();
        an.SetBool("HexFall", false);
        isBoom = false;
        _gameStatus = 1;
        _flow3 =0;
        
    }

    private void _playerReset()
    {
        for (int i = 0; i < 4; i++)
            player[i] = null;
    }

    

}
