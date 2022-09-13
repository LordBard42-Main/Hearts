using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameSprites : MonoBehaviour
{

    public static GameSprites instance;

    [HideInInspector]
    public Sprite SpadesSprite;
    [HideInInspector]
    public Sprite HeartsSprite;
    [HideInInspector]
    public Sprite ClubsSprite;
    [HideInInspector]
    public Sprite DiamondsSprite;

    [TabGroup("Card UI")]
    [SerializeField] private List<Sprite> hearts;
    [TabGroup("Card UI")]
    [SerializeField] private List<Sprite> spades;
    [TabGroup("Card UI")]
    [SerializeField] private List<Sprite> diamonds;
    [TabGroup("Card UI")]
    [SerializeField] private List<Sprite> clubs;
    [TabGroup("Card UI")]
    [SerializeField] private Sprite cardBack;

    private void Awake()
    {

        #region Singleton
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        #endregion


    }

    public List<Sprite> Hearts { get => hearts; }
    public List<Sprite> Spades { get => spades; }
    public List<Sprite> Diamonds { get => diamonds; }
    public List<Sprite> Clubs { get => clubs; }
    public Sprite CardBack { get => cardBack;}
}
