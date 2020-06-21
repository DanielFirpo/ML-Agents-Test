using UnityEngine;

public class Weapon: MonoBehaviour {

    protected BotAgent botWeaponHolder;

    protected Player playerWeaponHolder;

    public MonoBehaviour WeaponHolder {
        get {
            if (botWeaponHolder != null) {
                return botWeaponHolder;
            }
            else if (playerWeaponHolder != null) {
                return playerWeaponHolder;
            }
            else {
                return null;
            }
        }

        set {
            if (value is BotAgent) {
                botWeaponHolder = value as BotAgent;
            }
            else if (value is Player) {
                playerWeaponHolder = value as Player;
            }
        }
    }

    protected int ammo;

    public int Ammo {
        get { return ammo; }
        internal set { ammo = value; }
    }

    [SerializeField]
    protected int maxAmmo;

    public int MaxAmmo {
        get { return maxAmmo; }
        private set { maxAmmo = value; }
    }

    [SerializeField]
    protected float reloadTime;

    [SerializeField]
    protected float fireRate;

    [SerializeField]
    protected float recoil;

    [SerializeField]
    protected float spread;

    [SerializeField]
    protected AnimationCurve damageFallOff;

    [SerializeField]
    protected float damageCurveLength;

    [SerializeField]
    protected Transform muzzle;

    protected float timeAtReloadStart;

    protected float timeAtLastShot;

    protected bool isReloading;

    private void FixedUpdate() {
        if (isReloading) {
            if (Time.fixedTime - timeAtReloadStart >= reloadTime) {
                isReloading = false;
                Ammo = maxAmmo;
                Debug.Log("Reloaded");
            }
        }
    }

    public virtual bool Fire() {
        if (Ammo > 0 && Time.time - timeAtLastShot >= fireRate) {
            timeAtLastShot = Time.time;
            Ammo--;
            //Debug.Log(Ammo);
            return true;
        }
        else {
            return false;
        }
    }

    public virtual void Reload() {
        if (Ammo < maxAmmo && !isReloading) {
            Debug.Log(WeaponHolder.gameObject.name + " is reloading...");
            timeAtReloadStart = Time.time;
            isReloading = true;
        }
    }

}
