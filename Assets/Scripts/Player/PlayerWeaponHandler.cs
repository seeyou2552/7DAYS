using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private Player player;
    private PlayerStatus playerStatus;
    private PlayerAnimationHandler playerAnim;
    private TrajectoryController trajectoryController;

    private Dictionary<WeaponType, WeaponController> weaponDict = new Dictionary<WeaponType, WeaponController>();
    [SerializeField] List<WeaponController> unlockWeapons = new List<WeaponController>();
    [SerializeField] WeaponController curWeapon;
    [SerializeField] Transform throwPoint;

    private bool isAiming = false;

    public void Init(Player player, PlayerStatus playerStatus, PlayerAnimationHandler playerAnim)
    {
        this.player = player;
        this.playerStatus = playerStatus;
        this.playerAnim = playerAnim;

        trajectoryController = GetComponentInChildren<TrajectoryController>();

        if (trajectoryController)
            trajectoryController.Init(throwPoint);

        LoadWeaponData();

        UnlockWeapon(WeaponType.Buff);
        UnlockWeapon(WeaponType.Debuff);
    }

    void LoadWeaponData()
    {
        WeaponController[] weapons = Resources.LoadAll<WeaponController>("Weapon");

        foreach (var weapon in weapons)
        {
            weaponDict.Add(weapon.GetWeaponType(), weapon);
        }
    }

    public void UnlockWeapon(WeaponType type)
    {
        GameObject obj = Instantiate(weaponDict[type].gameObject, throwPoint);

        WeaponController weapon = obj.GetComponent<WeaponController>();
        weapon.ShowModel(false);
        weapon.Init(throwPoint);

        unlockWeapons.Add(weapon);
    }

    public void ChangeWeapon(int idx)
    {
        if (curWeapon != null)
        {
            curWeapon.ShowModel(false);
        }

        if (idx > -1 && idx < unlockWeapons.Count)
        {
            curWeapon = unlockWeapons[idx];
            curWeapon.ShowModel(true);
        }
    }

    public void StartAiming()
    {
        if (curWeapon == null || curWeapon.IsCoolDown) return;

        isAiming = true;
        trajectoryController.Show();
    }

    public void StopAiming()
    {
        isAiming = false;
        trajectoryController.Hide();
    }

    public void CheckThrow()
    {
        if (!isAiming || !playerStatus.UseStamina(curWeapon.WeaponDataSO.useStamina)) return;

        if (player.CurState != PlayerState.Vehicle)
        {
            player.StopVelocity();
            player.ChangeState(PlayerState.Throw);
            playerAnim.SetThrow(true);
        }
        else
        {
            ThrowGrenade();
        }
    }

    public void ThrowGrenade()
    {
        isAiming = false;
        trajectoryController.Hide();

        AudioManager.Instance.PlaySFX("ShootGranade");

        Vector3 direction = trajectoryController.GetAimDirectionForce(out float force);
        curWeapon.ThrowWeapon(direction, force);

        if (player.CurState != PlayerState.Vehicle)
        {
            player.ChangeState(PlayerState.Idle);
            playerAnim.SetThrow(false);
        }

        player.PlayerEvents.RaisedWeaponUsed(curWeapon.GetWeaponType());
    }
}