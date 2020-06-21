using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARWeapon : Weapon {

    public override bool Fire() {

        if (!base.Fire())
            return false;

        RaycastHit hit;

        LayerMask layerMask = LayerMask.GetMask("Default");

        if (LayerMask.LayerToName(botWeaponHolder.gameObject.layer) == "team1") {
            layerMask = LayerMask.GetMask("team2");//will only hit team2 layer
            //Debug.Log(botWeaponHolder.name + " is targeting team2");
        }
        else {
            layerMask = LayerMask.GetMask("team1");//will only hit team1 layer
            //Debug.Log(botWeaponHolder.name + " is targeting team1");
        }

        Debug.DrawRay(muzzle.position, muzzle.forward*20, Color.red, .5f);

        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, 500, layerMask)) {
            //Debug.Log("bullet hit SOMETHING");
            float damage = 0;

            if (hit.distance < damageCurveLength && !hit.transform.CompareTag("Player")) {

                damage = damageFallOff.Evaluate(hit.distance / damageCurveLength);
            }
            else {
                damage = damageFallOff.Evaluate(1);//The bullet hit something past max distance, lets just default to the lowest damagew now (don't want it to do NO dmg)
            }

            Player player = hit.transform.gameObject.GetComponent<Player>();
            if (player != null) {
                Debug.Log("damaging");
                player.Damage(damage);
            }

            BotAgent bot = hit.transform.gameObject.GetComponent<BotAgent>();
            if (bot != null) {
                Debug.Log(botWeaponHolder.gameObject.name + " hit " + bot.gameObject.name + " for " + damage + " damage!");
                bot.Damage(damage);
                botWeaponHolder.AddReward(.2f);
            }

        }

        return true;

    }

    public override void Reload() {
        base.Reload();
    }

}
