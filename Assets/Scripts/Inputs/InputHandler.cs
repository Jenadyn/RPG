using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float Horizontal;
    public float Vertical;
    public float MouseX;
    public float MouseY;
    public float MoveAmount;

    private PlayerInput inputActions;

    private Vector2 movementInput;
    private Vector2 cameraInput;

    private float rollTimer;

    public bool b_Input;
    public bool RollFlag;
    public bool SprintFlag;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInput();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void MoveInput(float delta)
    {
        Horizontal = movementInput.x;
        Vertical = movementInput.y;
        MouseX = cameraInput.x;
        MouseY = cameraInput.y;
        MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
    }

    private void HandleRollInput(float delta)
    {
        b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        if (b_Input)
        {
            rollTimer += Time.deltaTime;
            SprintFlag = true;
        }
        else
        {
            if (rollTimer > 0f && rollTimer < 0.3f)
            {
                SprintFlag = false;
                RollFlag = true;
            }
            rollTimer = 0f;
        }
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
    }
}