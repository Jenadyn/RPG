using UnityEngine;
using System.Collections;

public class RollAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void Start()
    {
        StartCoroutine(StartRoll());
    }

    public IEnumerator StartRoll()
    {
        yield return new WaitForSeconds(3f);
        anim.Play("Jump");
        yield return new WaitForSeconds(3f);
        anim.Play("RunningJump");
    }
}
