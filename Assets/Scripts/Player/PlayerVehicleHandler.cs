using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVehicleHandler : MonoBehaviour
{
    Player player;
    VehicleController curVehicle;
    Transform motorCycle;
    PlayerAnimationHandler playerAnim;

    [SerializeField] Vector3 spawnOffset;

    public void Init(Player player, PlayerAnimationHandler playerAnim)
    {
        this.player = player;
        this.playerAnim = playerAnim;
    }

    public void SetVehicle(VehicleController vehicle, PlayerInput playerInput)
    {

        if (vehicle != null)
        {
            // 처음 탑승 시 본인 탈 것 저장
            if (motorCycle == null)
            {
                motorCycle = vehicle.transform;
            }

            player.ChangeState(PlayerState.Vehicle);
            playerAnim.SetRide(true);

            curVehicle = vehicle;

            transform.SetParent(vehicle.MountPos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            vehicle.Init(playerInput);
        }
        else
        {
            player.ChangeState(PlayerState.Idle);
            playerAnim.SetRide(false);

            transform.SetParent(null);
            curVehicle.StopControl();

            curVehicle = null;
        }
    }

    public void CallVehicle()
    {
        if (motorCycle == null || player.CurState == PlayerState.Vehicle) return;

        motorCycle.position = player.transform.position + spawnOffset;
        motorCycle.rotation = Quaternion.identity;
    }
}
