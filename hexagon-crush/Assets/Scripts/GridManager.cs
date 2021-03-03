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
        float spriteXval = 0;
        float spriteYval = _spriteOffsetY;
        int rand = Random.Range(0, 4);
        int[] rands;
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
                sprite_go.name = "Sprite_" + x + "_" + y;


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

}
