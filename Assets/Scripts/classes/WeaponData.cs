using UnityEngine;

[CreateAssetMenu(fileName ="WeaponData",menuName ="My Game/WeaponData")]
public class WeaponData :ScriptableObject
{
    public string name = "SubMachine Gun";
    public float damage = 25f;
    public float range = 100f;

    public float fireRate = 3.5f;

    public int magazineSize= 10;
    public float reloadTime=1.5f;

    public GameObject graphics;
}
