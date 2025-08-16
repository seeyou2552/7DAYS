using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private Camera cam;

    private Vector3 moveDirection;
    public Vector3 MoveDirection { get => moveDirection; }

    private Vector3 lookDirection;
    public Vector3 LookDirection { get => lookDirection; }

    public void Init(Player player)
    {
        this.player = player;
        cam = Camera.main;
    }
    void Update()
    {
        Debug.DrawRay(transform.position - Vector3.up * 0.2f, transform.forward);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();

            moveDirection = Vector3.right * moveInput.x + Vector3.forward * moveInput.y;
            moveDirection = moveDirection.normalized;
        }

        else if (context.phase == InputActionPhase.Canceled)
        {
            moveDirection = Vector3.zero;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Started)
        {
            player.Dash();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        Vector2 mouseScreenPos = context.ReadValue<Vector2>();

        Ray ray = cam.ScreenPointToRay(mouseScreenPos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float point))
        {
            Vector3 hitPoint = ray.GetPoint(point);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0;

            if (direction.magnitude < 0.9f)
            {
                lookDirection = Vector3.zero;
            }
            else
            {
                lookDirection = direction.normalized;
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Started)
        {
            // 현재 탈 것 탑승중이면 해제
            if (player.CurState == PlayerState.Vehicle)
            {
                player.SetVehicle(null);
                player.ChangeState(PlayerState.Idle);
                return;
            }

            Ray ray = new Ray(transform.position - Vector3.up * 0.2f, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {
                if (hit.collider.TryGetComponent(out VehicleController vehicle))
                {
                    player.SetVehicle(vehicle);
                }

                else if (hit.collider.TryGetComponent(out Resource resource))
                {
                    player.GatheringResource(resource);
                }

                else if (hit.collider.TryGetComponent(out DroneManagerOffice office))
                {
                    office.OnDroneUI();
                }

                else if (hit.collider.TryGetComponent(out Refinery refinery))
                {
                    refinery.ShowUI();
                }
            }
        }
    }

    public void OnCallVehicle(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Started)
        {
            player.CallVehicle();
        }
    }

    public void OnCommand(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Started)
        {

        }
    }

    public void OnSelectSlot(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase != InputActionPhase.Started) return;

        string keyString = context.control.name;

        if (int.TryParse(keyString, out int keyNum))
        {
            int slotIdx = keyNum - 1;

            // 선택한 슬롯 인덱스 이벤트 호출
            player.PlayerEvents.RaisedSelectSlot(slotIdx);
        }
    }

    public void OnSelectWeapon(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase != InputActionPhase.Started) return;

        string keyString = context.control.name;

        if (int.TryParse(keyString, out int keyNum))
        {
            int weaponIdx = keyNum == 5 ? 0 : 1;

            player.SelectWeapon(weaponIdx);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Started)
        {
            player.ThrowGrenade();
        }
    }

    public void OnStartAiming(InputAction.CallbackContext context)
    {
        if (player.IsDead || player.CurState == PlayerState.Gathering) return;

        if (context.phase == InputActionPhase.Started)
        {
            player.StartAiming();
        }
    }

    public void OnStopAiming(InputAction.CallbackContext context)
    {
        if (player.IsDead) return;

        if (context.phase == InputActionPhase.Started)
        {
            player.StopAiming();
        }
    }
}
