using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eWeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missle,
    laser,
    shield

}

[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;

    public string letter;
    public Color powerUpColor = Color.white;
    public GameObject weaponModelPrefab;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float damagePerSecond;
    public float delayBetweenShots = 0;
    public float velocity = 50;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Dynamic")]
    [SerializeField]
    private eWeaponType _type;
    public WeaponDefinition def;
    public float nextShotTime;

    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        shotPointTrans = transform.GetChild(0);
        SetType(_type);
        Hero hero = GetComponentInParent<Hero>();
        if(hero != null) hero.fireEvent += Fire;
    }

    public eWeaponType type
    {
        get{return(_type);}
        set{SetType(value);}
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if(_type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GET_WEAPONDEFINITION(_type);
        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0;
    }

    private void Fire()
    {
        if(!gameObject.activeInHierarchy) return;
        if(Time.time < nextShotTime) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;
        switch (type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;
            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;

        }
    }

    private ProjectileHero MakeProjectile()
    {
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab,PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return(p);
    }

    
}
