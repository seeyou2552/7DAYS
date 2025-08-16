using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] public WeaponDataSO weaponDataSO;
    public WeaponDataSO WeaponDataSO { get => weaponDataSO; }
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] GameObject modelObject;
    Transform throwPoint;
    [SerializeField] private bool isCoolDown;
    public bool IsCoolDown { get => isCoolDown; }

    public void Init(Transform throwPoint)
    {
        isCoolDown = false;
        this.throwPoint = throwPoint;
    }

    public void ShowModel(bool isShow)
    {
        if (modelObject != null)
            modelObject.SetActive(isShow);
    }

    public void ThrowWeapon(Vector3 direction, float force)
    {
        if (!isCoolDown)
        {
            GameObject obj = Instantiate(grenadePrefab, throwPoint.position, Quaternion.Euler(Random.Range(0f, 360f), 0f, Random.Range(0f, 360f)));

            if (obj.TryGetComponent(out Grenade grenade))
            {
                grenade.Init(weaponDataSO, direction, force);
                StartCoroutine(CoolDownCoroutine());
            }
        }
    }

    private IEnumerator CoolDownCoroutine()
    {
        isCoolDown = true;

        yield return new WaitForSeconds(weaponDataSO.cooldown);

        isCoolDown = false;
    }

    public WeaponType GetWeaponType()
    {
        return weaponDataSO.weaponType;
    }
}
