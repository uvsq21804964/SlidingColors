using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileColor Color;

    public SpriteRenderer Renderer;
}

public enum TileColor {
    Red, 
    Green,
    Blue,
    Yellow,
    Purple,
    Orange,
    Pink,
    Turquoise,
    Wall
}