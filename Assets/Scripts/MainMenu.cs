using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button enterBattleButton;
    [SerializeField] Button prepareButton;

    void Start()
    {
        enterBattleButton.onClick.AddListener(BeginBattle);
    }

    private void BeginBattle()
    {
        GameManager.Instance.LoadBattleScene();
    }
}
