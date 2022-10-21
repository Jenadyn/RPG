using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputHandler))]
public class PlayerMovemen : MonoBehaviour
{
    private Transform cameraObject;
    private InputHandler inputHandler;

    private PlayerManager playerManager;

    private Vector3 moveDirection;
    private Vector3 normalVector;
    //private Vector3 targetPosition;

    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public AnimatorHandler animatorHandler;

    public Rigidbody rb;
    public GameObject NormalCamera;

    [Header("Player Info")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        animatorHandler.Initialize();
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        cameraObject = Camera.main.transform;
        PlayerTransform = transform;
    }

    public void HandleRotation(float delta)
    {
        if (!animatorHandler.CanRotate) return;

        Vector3 targetDirection = Vector3.zero;
        float moveOverride = inputHandler.MoveAmount;

        targetDirection = cameraObject.forward * inputHandler.Vertical;
        targetDirection += cameraObject.right * inputHandler.Horizontal;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection = PlayerTransform.forward;
        }

        float rs = rotationSpeed; //rs это rotation speed

        Quaternion tr = Quaternion.LookRotation(targetDirection); //tr это target rotation
        Quaternion targetRotation = Quaternion.Slerp(PlayerTransform.rotation, tr, rotationSpeed * delta);
        PlayerTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.RollFlag) return;

        moveDirection = cameraObject.forward * inputHandler.Vertical;
        moveDirection += cameraObject.right * inputHandler.Horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0f; //Hard

        float speed = movementSpeed;
        if (inputHandler.SprintFlag)
        {
            speed = sprintSpeed;
            playerManager.IsSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            moveDirection *= speed;
        }
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rb.velocity = projectedVelocity;

        animatorHandler.UpdateAnimation(inputHandler.MoveAmount, 0f, playerManager.IsSprinting);
        if (animatorHandler.CanRotate) HandleRotation(delta);
    }

    public void HandleRollingAndSprint(float delta)
    {
        if(animatorHandler.Anim.GetBool("IsInteracting")) return;
        
        if(inputHandler.RollFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.Vertical;
            moveDirection += cameraObject.right * inputHandler.Horizontal;
            if (inputHandler.MoveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Roll", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                PlayerTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }
}
