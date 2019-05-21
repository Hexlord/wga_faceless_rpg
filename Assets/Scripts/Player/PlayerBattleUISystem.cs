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
    private PlayerSkillBook skillBook;
    private DashSystem dashSystem;
    private ShieldSystem shieldSystem;
    private SheathSystem sheathSystem;
    private XpSystem xpSystem;
    private BodyStateSystem bodyStateSystem;

    private SkillsUISystem slot1;
    private SkillsUISystem slot2;
    private SkillsUISystem slotSpecial;
    private SkillsUtilityUISystem slotUtility1;
    private SkillsUtilityUISystem slotUtility2;

    // Cache

    private GameObject Raven;
    private GameObject JewelRed;
    private GameObject JewelBlue;

    private DashUISystem dashUiSystem;
    private ShieldUISystem shieldUiSystem;

    private Image Health;
    private Image Concentration;

    private Image XP;

    protected void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        concentrationSystem = GetComponent<ConcentrationSystem>();
        skillSystem = GetComponent<SkillSystem>();
        skillBook = GetComponent<PlayerSkillBook>();
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
        shieldUiSystem = battleUI.transform.Find("ShieldPanel").GetComponent<ShieldUISystem>();

        var statusPanel = battleUI.transform.Find("StatusPanel");
        var status = statusPanel.Find("Status");

        Health = status.Find("Health").GetComponent<Image>();
        Concentration = status.Find("Concentration").GetComponent<Image>();
        XP = statusPanel.Find("XPPanel").Find("XPBar").GetComponent<Image>();

        slot1 = skillsPanel.FindPrecise("Slot1").GetComponent<SkillsUISystem>();
        slot2 = skillsPanel.FindPrecise("Slot2").GetComponent<SkillsUISystem>();
        slotSpecial = skillsPanel.FindPrecise("SpecialSkillSlot").GetComponent<SkillsUISystem>();
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

        dashUiSystem.Charges = dashSystem.ChargesAvailabe;
        shieldUiSystem.Charges = shieldSystem.GetFullShieldCharges();

        Debug.Log("Current charges: " + shieldSystem.GetFullShieldCharges());

        Health.fillAmount = healthSystem.Health / healthSystem.healthMaximum;
        Concentration.fillAmount = concentrationSystem.Concentration / concentrationSystem.concentrationMaximum;
        XP.fillAmount = xpSystem.LevelCompletion;

        var type = physical ? SkillType.Physical : SkillType.Magical;
        var skill1 = skillBook.GetSkill(type, 0);
        var skill2 = skillBook.GetSkill(type, 1);
        var skillSpecial = skillBook.GetSkill(SkillType.Special, 0);
        slot1.Slot = skill1;
        slot2.Slot = skill2;
        slotSpecial.Slot = skillSpecial;
        slot1.Active = !sheathSystem.Sheathed;
        slot2.Active = !sheathSystem.Sheathed;
        slotSpecial.Active = !sheathSystem.Sheathed && (!skillSpecial.HasValue || concentrationSystem.Concentration >= skillSystem.ConcentrationCost(skillSpecial.Value));
        if (skill1.HasValue) slot1.CooldownNormalized = skillSystem.GetCooldownNormalized(skill1.Value);
        if (skill2.HasValue) slot2.CooldownNormalized = skillSystem.GetCooldownNormalized(skill2.Value);
        if (skillSpecial.HasValue) slotSpecial.CooldownNormalized = skillSystem.GetCooldownNormalized(skillSpecial.Value);
        slot1.Selected = skill1.HasValue && skillSystem.SelectedSkill != null && (skillSystem.SelectedSkill.Type == skill1);
        slot2.Selected = skill2.HasValue && skillSystem.SelectedSkill != null && (skillSystem.SelectedSkill.Type == skill2);
        slotSpecial.Selected = skillSpecial.HasValue && skillSystem.SelectedSkill != null && (skillSystem.SelectedSkill.Type == skillSpecial);
        
        slotUtility1.Slot = magical ? SkillUtility.Shoot : SkillUtility.Attack;
        slotUtility2.Slot = magical ? SkillUtility.Dash : SkillUtility.Shield;
        slotUtility1.Active = !sheathSystem.Sheathed;
        slotUtility2.Active = !sheathSystem.Sheathed;
    }

}
