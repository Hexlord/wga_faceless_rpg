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
    private SheathSystem sheathSystem;
    private XpSystem xpSystem;
    private BodyStateSystem bodyStateSystem;

    private SkillsUISystem slot1;
    private SkillsUISystem slot2;
    private SkillsUtilityUISystem slotUtility1;
    private SkillsUtilityUISystem slotUtility2;

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
        sheathSystem = GetComponent<SheathSystem>();
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

        slot1 = skillsPanel.FindPrecise("Slot1").GetComponent<SkillsUISystem>();
        slot2 = skillsPanel.FindPrecise("Slot2").GetComponent<SkillsUISystem>();
        slotUtility1 = skillsPanel.FindPrecise("SlotUtility1").GetComponent<SkillsUtilityUISystem>();
        slotUtility2 = skillsPanel.FindPrecise("SlotUtility2").GetComponent<SkillsUtilityUISystem>();

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

        var offset = 0;
        if (magical) offset += 2;

        slot1.Slot = skillSystem.SkillTypes[offset];
        slot2.Slot = skillSystem.SkillTypes[offset + 1];
        slot1.Active = !sheathSystem.Sheathed;
        slot2.Active = !sheathSystem.Sheathed;
        slot1.CooldownNormalized = skillSystem.Skills[offset].CooldownTimerNormalized;
        slot2.CooldownNormalized = skillSystem.Skills[offset + 1].CooldownTimerNormalized;
        slot1.Selected = skillSystem.SelectedSkill == offset;
        slot2.Selected = skillSystem.SelectedSkill == offset + 1;


        slotUtility1.Slot = magical ? SkillUtility.Shoot : SkillUtility.Attack;
        slotUtility2.Slot = magical ? SkillUtility.Dash : SkillUtility.Shield;
        slotUtility1.Active = !sheathSystem.Sheathed;
        slotUtility2.Active = !sheathSystem.Sheathed;
    }

}
