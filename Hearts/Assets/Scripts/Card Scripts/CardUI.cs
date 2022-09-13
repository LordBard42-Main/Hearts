using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardVisibility { Hidden, Visible}

public class CardUI : MonoBehaviour
{

    [SerializeField] 
    private SpriteRenderer cardRenderer;


    [SerializeField]
    private Sprite frontSprite;

    [SerializeField]
    private Sprite backSprite;



    [SerializeField] private Text rank;


    public void SetCardUI(Sprite frontSprite, Sprite backSprite, int layerOrder)
    {
        this.frontSprite = frontSprite;
        this.backSprite = backSprite;

        CardRenderer.sprite = backSprite;
        CardRenderer.sortingOrder = layerOrder;

    }

    public void UpdateCardSprite(CardVisibility visibility)
    {
        switch (visibility)
        {
            case CardVisibility.Hidden:
                CardRenderer.sprite = backSprite;
                break;
            case CardVisibility.Visible:
                CardRenderer.sprite = frontSprite;
                break;
            default:
                break;
        }
    }

    public void UpdateSortingLayer(int layerOrder)
    {
        CardRenderer.sortingOrder = layerOrder;
    }

    public SpriteRenderer CardRenderer { get => cardRenderer;}
}
