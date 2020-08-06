using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct tile
{

}

public class Board : MonoBehaviour
{
    public const int numX = 5;
    public const int numY = 6;
    int[,] board = new int[numX, numY];

    public Sprite[] ballSprite;

    private void Start()
    {
        
    }
}
