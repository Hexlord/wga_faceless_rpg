using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Image MaskHighlihgt, SwordHighlight;
    public Sprite ShieldUp, ShieldDown, DashUp, DashDown;
    public GridLayoutGroup physicalChargesGrid, magicalChargesGrid;
    public GameObject player;
    private Character character;
    private DefenseSystem defenseSystem;

    private Image[] physicalChargesIndicator, magicalChargesIndicator;
    // Start is called before the first frame update
    void Start()
    {
        defenseSystem = player.GetComponent<DefenseSystem>();
        character = player.GetComponent<Character>();
        physicalChargesIndicator = physicalChargesGrid.GetComponentsInChildren<Image>();
        magicalChargesIndicator = magicalChargesGrid.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    public void UpdateUI()
    {
       for (int i = physicalChargesIndicator.Length - 1; i >= 0; i--)
       {
            physicalChargesIndicator[i].sprite = (defenseSystem.physicalDefenseChargesAvailable[i]) ? ShieldUp : ShieldDown;
       }

        for (int i = magicalChargesIndicator.Length - 1; i >= 0; i--)
        {
            magicalChargesIndicator[i].sprite = (defenseSystem.magicalDefenseChargesAvailable[i]) ? DashUp : DashDown;
        }
    }
}
