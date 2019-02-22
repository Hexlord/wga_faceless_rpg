using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobStatusSystem : BasicStatusSystem
{

    public override void OnDeath()
    {
        base.OnDeath();

        healthPoints = 100.0f;
        gameObject.GetComponent<BaseCharacter>().ResetPosition();
    }

}
