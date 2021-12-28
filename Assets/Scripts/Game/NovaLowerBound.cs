using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaLowerBound : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!NovaGame.PlayerIsDeadly())
            NovaGame.OnReset();
    }
}
