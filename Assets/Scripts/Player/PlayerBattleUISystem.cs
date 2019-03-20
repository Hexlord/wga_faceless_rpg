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
    private BodyStateSystem bodyStateSystem;

    // Cache

    private GameObject PhysAttackFill;
    private GameObject MageAttackFill;

    private GameObject PhysStateFill;
    private GameObject MageStateFill;
    private GameObject PhysStateUpgrade;
    private GameObject MageStateUpgrade;

    private Image Health;
    private Image Concentration;

    private Image XP;

    protected void Start()
    {
        if (!battleUI)
        {
            battleUI = GameObject.Find("Canvas").transform.Find("BattleUI").gameObject;
        }

        healthSystem = GetComponent<HealthSystem>();
        concentrationSystem = GetComponent<ConcentrationSystem>();
        skillSystem = GetComponent<SkillSystem>();
        bodyStateSystem = GetComponent<BodyStateSystem>();

        PhysAttackFill = battleUI.transform.Find("PhysAttack").Find("Fill").gameObject;
        MageAttackFill = battleUI.transform.Find("MageAttack").Find("Fill").gameObject;

        PhysStateFill = battleUI.transform.Find("PhysState").Find("Fill").gameObject;
        MageStateFill = battleUI.transform.Find("MageState").Find("Fill").gameObject;
        PhysStateUpgrade = battleUI.transform.Find("PhysState").Find("Upgrade").gameObject;
        MageStateUpgrade = battleUI.transform.Find("MageState").Find("Upgrade").gameObject;

        Health        = battleUI.transform.Find("Status").Find("Health").gameObject.GetComponent<Image>();
        Concentration = battleUI.transform.Find("Status").Find("Concentration").gameObject.GetComponent<Image>();
        XP            = battleUI.transform.Find("XPBar").Find("Fill").gameObject.GetComponent<Image>();

        XP.fillAmount = 0;

        battleUI.SetActive(true);

    }
    protected void Update()
    {
        bool physical = bodyStateSystem.State == BodyStateSystem.BodyState.Physical;

        PhysAttackFill.SetActive(physical);
        MageAttackFill.SetActive(!physical);

        PhysStateFill.SetActive(physical);
        MageStateFill.SetActive(!physical);
        PhysStateUpgrade.SetActive(physical && false);
        MageStateUpgrade.SetActive(!physical && false);

        Health.fillAmount = healthSystem.Health / healthSystem.healthMaximum;
        Concentration.fillAmount = concentrationSystem.Concentration / concentrationSystem.concentrationMaximum;

        XP.fillAmount += 0.03f * Time.deltaTime;



    }

}
