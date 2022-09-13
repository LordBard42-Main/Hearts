using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{

    [TabGroup("South Player AI")]
    [SerializeField]
    private Button basicRulesSouth;

    [TabGroup("South Player AI")]
    [SerializeField]
    private Button basicAISouth;

    

    [TabGroup("West Player AI")]
    [SerializeField]
    private Button basicRulesWest;

    [TabGroup("West Player AI")]
    [SerializeField]
    private Button basicAIWest;

    public AILevels testLevels;


    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetSouthPlayerAI(AILevels aiLevel)
    {
        testLevels = aiLevel;
        Debug.Log(aiLevel);
    }

}



public class ButtonGroup
{

    private List<Button> buttons;

    public ButtonGroup(List<Button> buttons)
    {
        foreach(Button button in buttons)
        {
            this.buttons.Add(button);
        }

    }


    public bool SetActiveButton(Button button)
    {
        if (!buttons.Contains(button))
            return false;

        foreach(Button buttonDeselct in buttons)
        {
           
        }

        button.Select();
        return true;
    }

}
