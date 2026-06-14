using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyingMode : MonoBehaviour
{
    [SerializeField]
    private Dictionary<bool, float> speedModes = new Dictionary<bool, float>
    {
        [true] = 200f,      // true - �����, false - �� �����
        [false] = 40f
    };

    private Dictionary<bool, float> cameraModes = new Dictionary<bool, float>
    { 
        [true] = 70f,
        [false] = 40f
    };

    private float currentSpeed = 200f;
    private float currentCameraSize = 128f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera mainCamera;
    private GameObject character;
    public bool isFlying
    {
        get
        {
            return isflying;
        }
        set
        {
            isflying = value;

            character.SetActive(!value);
            currentCameraSize = cameraModes[value];
            currentSpeed = speedModes[value];

        }
    }
    private bool isflying = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        character = transform.GetChild(0).gameObject;

        isFlying = true; 
}

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = Vector2.LerpUnclamped(rb.linearVelocity, moveInput * currentSpeed, 0.03f);
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isFlying = !isFlying;
        }
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, currentCameraSize, 0.02f);     // �����������
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;
    }

}

