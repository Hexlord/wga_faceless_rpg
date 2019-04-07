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

    private GameObject PhysAttackFill;
    private GameObject MageAttackFill;

    private GameObject PhysStateFill;
    private GameObject MageStateFill;
    private GameObject PhysStateUpgrade;
    private GameObject MageStateUpgrade;

    private DashUISystem dashUiSystem;

    private Image Health;
    private Image Concentration;

    private Image XP;

    protected void Awake()
    {
        if (!battleUI)
        {
            battleUI = GameObject.Find("UI").FindChildPrecise("Canvas").transform.Find("BattleUI").gameObject;
        }

        healthSystem = GetComponent<HealthSystem>();
        concentrationSystem = GetComponent<ConcentrationSystem>();
        skillSystem = GetComponent<SkillSystem>();
        dashSystem = GetComponent<DashSystem>();
        shieldSystem = GetComponent<ShieldSystem>();
        xpSystem = GetComponent<XpSystem>();
        bodyStateSystem = GetComponent<BodyStateSystem>();

        PhysAttackFill = battleUI.transform.Find("PhysAttack").Find("Fill").gameObject;
        MageAttackFill = battleUI.transform.Find("MageAttack").Find("Fill").gameObject;

        PhysStateFill = battleUI.transform.Find("PhysState").Find("Fill").gameObject;
        MageStateFill = battleUI.transform.Find("MageState").Find("Fill").gameObject;
        PhysStateUpgrade = battleUI.transform.Find("PhysState").Find("Upgrade").gameObject;
        MageStateUpgrade = battleUI.transform.Find("MageState").Find("Upgrade").gameObject;

        dashUiSystem = battleUI.transform.Find("Dash").GetComponent<DashUISystem>();

        Health = battleUI.transform.Find("Status").Find("Health").GetComponent<Image>();
        Concentration = battleUI.transform.Find("Status").Find("Concentration").GetComponent<Image>();
        XP            = battleUI.transform.Find("XPBar").Find("Fill").GetComponent<Image>();

        XP.fillAmount = 0;

        battleUI.SetActive(true);

    }
    protected void Update()
    {
        var physical = bodyStateSystem.State == BodyStateSystem.BodyState.Physical;
        var magical = !physical;

        PhysAttackFill.SetActive(physical);
        MageAttackFill.SetActive(magical);

        PhysStateFill.SetActive(physical);
        MageStateFill.SetActive(magical);
        PhysStateUpgrade.SetActive(physical && xpSystem.SwordPoints > 0);
        MageStateUpgrade.SetActive(magical && xpSystem.MaskPoints > 0);

        dashUiSystem.DashCharges = dashSystem.ChargesAvailabe;

        Health.fillAmount = healthSystem.Health / healthSystem.healthMaximum;
        Concentration.fillAmount = concentrationSystem.Concentration / concentrationSystem.concentrationMaximum;

        XP.fillAmount = xpSystem.LevelCompletion;
        
    }

}
