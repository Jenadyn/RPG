using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorHandl : MonoBehaviour
{
    [HideInInspector] public Animator Anim;
    private InputHandler inputHandler;
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;

    private int horizontal;
    private int vertical;

    public bool CanRotate;

    public void Initialize()
    {
        Anim = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        inputHandler = GetComponentInParent<InputHandler>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerManager = GetComponentInParent<PlayerManager>();
    }

    public void UpdateAnimation(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        float tempVertical = 0f;
        float tempHorizontal = 0f;

        #region Vertical
        if (verticalMovement > 0f && verticalMovement < 0.55f)
        {
            tempVertical = 0.5f;
        }
        else if(verticalMovement > 0.55f)
        {
            tempVertical = 1f;
        }
        else if(verticalMovement < 0f && verticalMovement > -0.55f)
        {
            tempVertical = -0.5f;
        }
        else if(verticalMovement < -0.55f)
        {
            tempVertical = -1f;
        }
        else
        {
            tempVertical = 0f;
        }
        #endregion
        #region Horizontal
        if (horizontalMovement > 0f && horizontalMovement < 0.55f)
        {
            tempHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            tempHorizontal = 1f;
        }
        else if (horizontalMovement < 0f && horizontalMovement > -0.55f)
        {
            tempHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            tempHorizontal = -1f;
        }
        else
        {
            tempHorizontal = 0f;
        }
        #endregion

        if(isSprinting && inputHandler.Vertical > 0 && inputHandler.Horizontal > 0)
        {
            tempVertical = 2f;
            tempHorizontal = horizontalMovement;
        }

        Anim.SetFloat(vertical, tempVertical, 0.1f, Time.deltaTime);
        Anim.SetFloat(horizontal, tempHorizontal, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimationName, bool isInteracting)
    {
        Anim.applyRootMotion = isInteracting;
        Anim.SetBool("IsInteracting", isInteracting);
        Anim.CrossFade(targetAnimationName, 0.1f);
    }

    public void StartRotation() => CanRotate = true;
    public void StopRotation() => CanRotate = false;

    private void OnAnimatorMove()
    {
        if (playerManager.IsInteracting == false) return;

        float delta = Time.deltaTime;
        playerMovement.rb.drag = 0f;

        Vector3 deltaPos = Anim.deltaPosition;
        deltaPos.y = 0f;
        playerMovement.rb.velocity = deltaPos / delta;
    }
}
