using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class PlayerManag : MonoBehaviour
{
    private InputHandler inputHandler;
    private Animator anim;
    private CameraHandler cameraHandler;
    private PlayerMovement playerMovement;

    public bool IsInteracting;
    public bool IsSprinting;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraHandler = CameraHandler.Singltone;
    }

    private void Update()
    {
        IsInteracting = anim.GetBool("IsInteracting");

        float delta = Time.deltaTime;

        playerMovement.HandleMovement(delta);
        playerMovement.HandleRollingAndSprint(delta);

        IsSprinting = inputHandler.b_Input;

        inputHandler.TickInput(delta);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCamera(delta, inputHandler.MouseX, inputHandler.MouseY);
        }
    }

    private void LateUpdate()
    {
        inputHandler.RollFlag = false;
        inputHandler.SprintFlag = false;
    }
}
