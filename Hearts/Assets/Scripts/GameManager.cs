using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    private void Awake()
    {

        #region Singleton

        if(instance !=null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance);

        #endregion

    }

    private void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //gameStateMachine.Tick();

    }




    public void UpdateTable()
    {
        

    }
   
}
