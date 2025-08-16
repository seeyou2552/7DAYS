using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator anim;
    private SkinnedMeshRenderer[] renderers;
    [SerializeField] float hitDuration = 0.5f;
    private readonly int IsMoving = Animator.StringToHash("IsMoving");
    private readonly int IsRide = Animator.StringToHash("IsRide");
    private readonly int IsGathering = Animator.StringToHash("IsGathering");
    private readonly int IsThrow = Animator.StringToHash("IsThrow");
    private readonly int IsDead = Animator.StringToHash("IsDead");
    private readonly int IsDash = Animator.StringToHash("IsDash");

    public void Init(Animator anim)
    {
        this.anim = anim;
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public void SetMoveSpeed(float speed)
    {
        anim.SetFloat(IsMoving, speed);
    }

    public void SetDash(bool isDash)
    {
        anim.SetBool(IsDash, isDash);
    }

    public void SetThrow(bool isThrow)
    {
        anim.SetBool(IsThrow, isThrow);
    }

    public void SetRide(bool isRide)
    {
        anim.SetBool(IsRide, isRide);
    }

    public void SetDeath()
    {
        anim.SetTrigger(IsDead);
    }

    public void SetGathering(bool isGathering)
    {
        anim.SetBool(IsGathering, isGathering);
    }

    public void Hit(Action onCompleted)
    {
        StartCoroutine(HitCoroutine(onCompleted));
    }

    IEnumerator HitCoroutine(Action onCompleted)
    {
        float time = 0f;
        float t = 0f;

        foreach (var renderer in renderers)
        {
            renderer.material.color = Color.red;
        }

        while (time <= hitDuration)
        {
            time += Time.deltaTime;
            t = time / hitDuration;

            foreach (var renderer in renderers)
            {
                renderer.material.color = Color.Lerp(Color.red, Color.white, t);
            }

            yield return null;
        }

        foreach (var renderer in renderers)
        {
            renderer.material.color = Color.white;
        }

        onCompleted?.Invoke();
    }
}
