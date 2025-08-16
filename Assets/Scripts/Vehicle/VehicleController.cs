using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    [SerializeField] VehicleDataSO vehicleDataSO;
    [SerializeField] private Transform mountPos;
    VehicleAudioHandler vehicleAudio;
    public Transform MountPos { get => mountPos; }

    // Vehicle Stats
    private float speed;
    private float turnSpeed;

    // Input 
    private InputAction moveAction;
    private Vector2 moveInput = Vector2.zero;
    private bool isControlled = false;
    private Rigidbody _rigidbody;
    void Start()
    {
        vehicleAudio = GetComponent<VehicleAudioHandler>();
        _rigidbody = GetComponent<Rigidbody>();

        if (vehicleAudio)
            vehicleAudio.Init();
    }

    public void Init(PlayerInput playerInput)
    {
        // 데이터 세팅
        speed = vehicleDataSO.Speed;
        turnSpeed = vehicleDataSO.TurnSpeed;

        // InputAction 세팅
        moveAction = playerInput.actions["VehicleMove"];

        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        moveAction.Enable();

        StartControl();
        vehicleAudio.PlayVehicleStart();
    }

    private void StartControl() => isControlled = true;
    public void StopControl()
    {
        isControlled = false;
        _rigidbody.velocity = Vector3.zero;
        
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        moveAction?.Disable();
        vehicleAudio.PlayVehicleStop();
        
    }

    void Update()
    {
        if (!isControlled || moveAction == null) return;
        Move();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _rigidbody.velocity = Vector3.zero;

            moveInput = context.ReadValue<Vector2>();
            moveInput = moveInput.normalized;
            vehicleAudio.SetPitch(true);
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            _rigidbody.velocity = Vector3.zero;

            moveInput = Vector2.zero;
            vehicleAudio.SetPitch(false);
        }
    }
    void Move()
    {
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        transform.Translate(Vector3.forward * vertical * speed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontal * turnSpeed * Time.deltaTime);
    }
}
