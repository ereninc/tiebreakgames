using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sprite;
    void Start()
    {
        transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, 0, 120), 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
