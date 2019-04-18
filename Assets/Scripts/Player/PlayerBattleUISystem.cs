using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 15.03.2019   aknorre     Created
 * 17.03.2019   bkrylov     Allocated to Component Menu
 */
[AddComponentMenu("ProjectFaceless/Player/PlayerBattleUISystem")]
public class PlayerBattleUISystem : MonoBehaviour
{

    // Public

    [Header("UI Settings")]


    [Tooltip("BattleUI object")]
    public GameObject battleUI;

    // Private

    private HealthSystem healthSystem;
    private ConcentrationSystem concentrationSystem;
    private SkillSystem skillSystem;
    private DashSystem dashSystem;
    private ShieldSystem shieldSystem;
    private XpSystem xpSystem;
    private BodyStateSystem bodyStateSystem;

    // Cache

    private GameObject Raven;
    private GameObject JewelRed;
    private GameObject JewelBlue;

    private DashUISystem dashUiSystem;

    private Image Health;
    private Image Concentration;

    private Image XP;

    protected void Awake()
    {
        if (!battleUI)
        {
            battleUI = GameObject.Find("UI").FindPrecise("Canvas").FindPrecise("BattleUI").gameObject;
        }

        healthSystem = GetComponent<HealthSystem>();
        concentrationSystem = GetComponent<ConcentrationSystem>();
        skillSystem = GetComponent<SkillSystem>();
        dashSystem = GetComponent<DashSystem>();
        shieldSystem = GetComponent<ShieldSystem>();
        xpSystem = GetComponent<XpSystem>();
        bodyStateSystem = GetComponent<BodyStateSystem>();

        var skillsPanel = battleUI.transform.Find("SkillsPanel");

        Raven = skillsPanel.Find("Raven").gameObject;
        JewelRed = Raven.FindPrecise("JewelRed").gameObject;
        JewelBlue = Raven.FindPrecise("JewelBlue").gameObject;

        dashUiSystem = battleUI.transform.Find("DashPanel").GetComponent<DashUISystem>();

        var statusPanel = battleUI.transform.Find("StatusPanel");
        var status = statusPanel.Find("Status");

        Health = status.Find("Health").GetComponent<Image>();
        Concentration = status.Find("Concentration").GetComponent<Image>();
        XP = statusPanel.Find("XPPanel").Find("XPBar").GetComponent<Image>();

        Health.fillAmount = 1;
        Concentration.fillAmount = 0;
        XP.fillAmount = 0;

        battleUI.SetActive(true);

    }
    protected void Update()
    {
        var physical = bodyStateSystem.State == BodyStateSystem.BodyState.Physical;
        var magical = !physical;

        JewelRed.SetActive(physical);
        JewelBlue.SetActive(magical);

        dashUiSystem.DashCharges = dashSystem.ChargesAvailabe;

        Health.fillAmount = healthSystem.Health / healthSystem.healthMaximum;
        Concentration.fillAmount = concentrationSystem.Concentration / concentrationSystem.concentrationMaximum;
        XP.fillAmount = xpSystem.LevelCompletion;
        
    }

}
