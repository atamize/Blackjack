using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public CardData cardData;
    public Sprite backSprite;

    bool faceUp = true;
    public bool FaceUp
    {
        get
        {
            return faceUp;
        }
        set
        {
            faceUp = value;
            spriteRenderer.sprite = value ? cardData.sprite : backSprite;
        }
    }

    public int GetValue()
    {
        return cardData.value;
    }
}
