using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * History:
 * 
 * Date         Author      Description
 * 
 * 03.03.2019   aknorre     Created
 * 
 */

public class MobStatusSystem : BasicStatusSystem
{

    public override void OnDeath()
    {
        base.OnDeath();

        healthPoints = 100.0f;
        gameObject.GetComponent<BaseCharacter>().ResetPosition();
    }

}
