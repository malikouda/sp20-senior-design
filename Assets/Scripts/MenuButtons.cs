using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{

    public GameObject OptionsMenu;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            OptionsMenu.SetActive(!OptionsMenu.activeInHierarchy);
        }
    }
       
    public void ExitButton()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().NextLevel();
    }

    public void OptionButton()
    {
        OptionsMenu.SetActive(!OptionsMenu.activeInHierarchy);
    }
}
