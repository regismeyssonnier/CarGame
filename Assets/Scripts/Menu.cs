using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    PlayerController manette;
    //public static bool GameEnd = true;

    public GameObject Menu_Princ;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Awake()
    {
        

    }

    public void MenuPrinc(InputAction.CallbackContext ctx)
    {
        
        if (Player.GameEnd)
        {
            ShowEnd();
        }
        else
        {
            HideEnd();
        }
        Player.GameEnd = !Player.GameEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.GameEnd)
        {
            ShowEnd();
        }
        else
        {
            HideEnd();
        }
    }

    public void ShowEnd()
    {
        Menu_Princ.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;

    }

    public void HideEnd()
    {
        Menu_Princ.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Application.Quit();
    }



}
