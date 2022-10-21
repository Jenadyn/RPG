using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputHandler))]
public class PlayerMovement : MonoBehaviour
{
    private Transform cameraObject;
    private InputHandler inputHandler;

    private PlayerManager playerManager;

    public Vector3 MoveDirection;

    private Vector3 normalVector;
    private Vector3 targetPosition;

    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public AnimatorHandler animatorHandler;

    public Rigidbody rb;
    public GameObject NormalCamera;

    private float groundDetectionRay = 0.5f;
    private float minimumDistanceToFall = 1f;
    private float groundDetectionDistance = 0.2f;

    private LayerMask ignoreGroundLayer;

    public float AirTimer;

    [Header("Player Info")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fallSpeed = 50f;

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
        playerManager.IsGrouned = true;
        ignoreGroundLayer = ~(1 << 8 | 1 << 11);
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

        if (targetDirection == Vector3.zero)
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

        if (playerManager.IsInteracting) return;

        MoveDirection = cameraObject.forward * inputHandler.Vertical;
        MoveDirection += cameraObject.right * inputHandler.Horizontal;
        MoveDirection.Normalize();
        MoveDirection.y = 0f; //Hard

        float speed = movementSpeed;
        if (inputHandler.SprintFlag)
        {
            speed = sprintSpeed;
            playerManager.IsSprinting = true;
            MoveDirection *= speed;
        }
        else
        {
            MoveDirection *= speed;
        }
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(MoveDirection, normalVector);
        rb.velocity = projectedVelocity;

        animatorHandler.UpdateAnimation(inputHandler.MoveAmount, 0f, playerManager.IsSprinting);
        if (animatorHandler.CanRotate) HandleRotation(delta);
    }

    public void HandleRollingAndSprint(float delta)
    {
        if (animatorHandler.Anim.GetBool("IsInteracting")) return;

        if (inputHandler.RollFlag)
        {
            MoveDirection = cameraObject.forward * inputHandler.Vertical;
            MoveDirection += cameraObject.right * inputHandler.Horizontal;
            if (inputHandler.MoveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Roll", true);
                MoveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(MoveDirection);
                PlayerTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    public void HandlerFalling(float delta, Vector3 moveDirection)
    {
        playerManager.IsGrouned = false;
        RaycastHit hit;
        Vector3 startingPoint = PlayerTransform.position;
        startingPoint.y += groundDetectionRay;

        if (Physics.Raycast(startingPoint, PlayerTransform.forward, out hit, 0.5f))
        {
            moveDirection = Vector3.zero;
        }

        if (playerManager.IsInAir)
        {
            rb.AddForce(Vector3.down * fallSpeed);
            rb.AddForce(moveDirection * fallSpeed / 5f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        startingPoint += dir * groundDetectionDistance;
        targetPosition = PlayerTransform.position;
        Debug.DrawRay(startingPoint, Vector3.down * minimumDistanceToFall, Color.black, 0.1f);

        if (Physics.Raycast(startingPoint, Vector3.down, out hit, minimumDistanceToFall, ignoreGroundLayer))
        {
            normalVector = hit.normal;
            Vector3 pos = hit.point;
            playerManager.IsGrouned = true;
            targetPosition.y = pos.y;

            if (playerManager.IsInAir)
            {
                if(AirTimer > 0.3f)
                {
                    animatorHandler.PlayTargetAnimation("Land", true);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Player Movement", false);
                }

                playerManager.IsInAir = false;
            }
        }
        else
        {
            if (playerManager.IsGrouned) playerManager.IsGrouned = false;

            if (!playerManager.IsInAir)
            {
                if (!playerManager.IsInteracting) animatorHandler.PlayTargetAnimation("Falling", true);
                Vector3 speed = rb.velocity; 
                speed.Normalize();
                rb.velocity = speed * (movementSpeed / 2);
                playerManager.IsInAir = true;
            }
        }

        if (playerManager.IsGrouned)
        {
            if (inputHandler.MoveAmount > 0f || playerManager.IsInteracting)
            {
                PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, targetPosition, delta);
            }
            else
            {
                PlayerTransform.position = targetPosition;
            }
        }
    }
}
