using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFestive : MonoBehaviour
{
    void Awake()
    {
        if(DateTime.Now.Month != 12)
        {
            gameObject.SetActive(false);
        }
    }
}
