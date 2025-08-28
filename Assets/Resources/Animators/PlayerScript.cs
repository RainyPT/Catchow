using UnityEngine;
using Unity.Netcode;
using UnityEngine.Audio;

public class PlayerScript : NetworkBehaviour
{
    [Header("Movement Settings")]
    public NetworkVariable<float> _moveSpeed = new NetworkVariable<float>(5.0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

    [SerializeField] private float _crouchSpeed;
    private float _currentSpeed;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _gravity;
    private Transform _groundCheck;
    private Vector3 _velocity;
    private bool _isCrouching;
    private float _crouchHeight;

    [Header("Mouse Settings")]
    [SerializeField] private float _mouseSensitivity;
    private float _cameraPitch;

    [Header("Camera Settings")]
    [SerializeField] public Camera _playerCamera;
    private Transform _playerCrosshair;
    private float _crosshairDistance;

    [Header("Character Components")]
    private Transform _playerBodyAsset;
    private CharacterController _characterController;
    private float _characterControllerOriginalHeight;
    private Vector3 _characterControllerOriginalCenter;
    public Animator _characterAnimator;

    public AudioMixer audioMixer;
    private float savedVolume;
    private float db;
    private AudioSource footstepsSource;
    void Start()
    {
        _characterAnimator = GetComponent<Animator>();
        footstepsSource = GetComponent<AudioSource>();
        if (!IsOwner) return;
        _playerCamera.gameObject.SetActive(true);
        _playerCamera.fieldOfView = PlayerPrefs.GetFloat("FOV", 60f);

        savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        db = (savedVolume <= 0.0001f) ? -80f : Mathf.Log10(savedVolume) * 20f;
        audioMixer.SetFloat("MasterVolume", db);
        
        // Getting Components from our character
        _groundCheck = transform.Find("GroundCheck");
        _playerCrosshair = transform.Find("Crosshair");
        _playerBodyAsset = transform.Find("BodyAsset");
        _characterController = GetComponent<CharacterController>();

        // Setting up our sensitivity according to what the player selected before (or just leave it as is)
        _mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 300f);

        // Locking the mouse at the middle and make it invisible, not just fly everywhere like we would be in the Desktop per example
        Cursor.lockState = CursorLockMode.Locked;

        // Getting info for character controller hitbox changes (from crouch)
        _isCrouching = false;
        _characterControllerOriginalHeight = _characterController.height;
        _characterControllerOriginalCenter = _characterController.center;

        // Getting distance between the crosshair and the player so we can move it without any problems
        _crosshairDistance = Vector3.Distance(_playerCamera.transform.position, _playerCrosshair.position);
    }

    void Update()
    {
        if (!IsOwner) return;

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        if (Ingame_menu_Manager.isOpen)
        {
            moveX = 0f;
            moveZ = 0f;
        }
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        bool isMoving = moveX != 0f || moveZ != 0f;
        bool isWalking = isMoving && !_isCrouching;
        UpdateWalkingRpc(isWalking);

        // Jumping
        if (_characterController.isGrounded && !Ingame_menu_Manager.isOpen)
        {
            if (_velocity.y < 0)
                _velocity.y = -1f; // Keep player grounded so he doesn't fly for some reason

            if (Input.GetButtonDown("Jump"))
            {
                JumpRpc();
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity); // Get the jumping going
            }
        }
        else
        {
            _velocity.y += _gravity * Time.deltaTime;
        }

        // Crouching
        if (Input.GetKey(KeyCode.LeftControl) && !Ingame_menu_Manager.isOpen)
        {
            if (!_isCrouching)
            {
                _characterController.height = _crouchHeight;
                _characterController.center = new Vector3(0, (float)0.6, 0);
                _isCrouching = true;
            }
        }
        else
        {
            _characterController.height = _characterControllerOriginalHeight;
            _characterController.center = _characterControllerOriginalCenter;
            _isCrouching = false;
        }

        UpdateCrouchRpc(_isCrouching);

        _currentSpeed = _isCrouching ? _crouchSpeed : _moveSpeed.Value;

        Vector3 finalMove = move * _currentSpeed + new Vector3(0, _velocity.y, 0); // End result of inputs

        _characterController.Move(finalMove * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Alpha1) && !Ingame_menu_Manager.isOpen) // "1" key on the keyboard
        {
            BreakDanceRpc();
        }

        // Mouse
        _mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 300f); // Never forget this in case player changes sens in-game

        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        if (Ingame_menu_Manager.isOpen)
        {
            mouseX = 0f;
            mouseY = 0f;
        }
        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f); // Clamp it between up 90º and down 90º (so we don't flip upside down)
        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        Vector3 crosshairPos = _playerCamera.transform.position + _playerCamera.transform.forward * _crosshairDistance;
        _playerCrosshair.position = crosshairPos; // Crosshair movement

        // Camera FOV
        _playerCamera.fieldOfView = PlayerPrefs.GetFloat("FOV", 60f); // Never forget this in case player changes fov in-game

        savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        db = (savedVolume <= 0.0001f) ? -80f : Mathf.Log10(savedVolume) * 20f;
        audioMixer.SetFloat("MasterVolume", db);
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateWalkingRpc(bool isWalking)
    {
        _characterAnimator.SetBool("isWalking", isWalking);
        if (isWalking == true)
        {
            if (!footstepsSource.isPlaying)
                footstepsSource.Play();
            return;
        }
        if (footstepsSource.isPlaying)
            footstepsSource.Stop();

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void JumpRpc()
    {
        _characterAnimator.SetTrigger("Jump");
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateCrouchRpc(bool _isCrouching)
    {
        _characterAnimator.SetBool("isCrouched", _isCrouching);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void BreakDanceRpc()
    {
        _characterAnimator.SetTrigger("Action");
    }
}
