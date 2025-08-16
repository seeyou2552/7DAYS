using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private Rigidbody _rigidbody;
    private PlayerAnimationHandler playerAnim;
    public void Init(Player player, Rigidbody rigidbody, PlayerAnimationHandler playerAnim)
    {
        this.player = player;
        _rigidbody = rigidbody;
        this.playerAnim = playerAnim;
    }

    public void Move(Vector3 moveDirection, float moveSpeed)
    {
        if (player.CurState == PlayerState.Dash || player.CurState == PlayerState.Throw) return;

        if (moveDirection == Vector3.zero)
            player.ChangeState(PlayerState.Idle);
        else
            player.ChangeState(PlayerState.Walk);

        // y 속력값은 건드리지 않기 위함
        Vector3 velocity = _rigidbody.velocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;

        _rigidbody.velocity = velocity;

        playerAnim.SetMoveSpeed(velocity.magnitude);
    }

    public void Rotate(Vector3 lookDirection)
    {
        float angle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public IEnumerator DashCoroutine(Vector3 direction, float dashSpeed, float duration, float cooldown)
    {
        player.CanDash = false;
        player.ChangeState(PlayerState.Dash);
        playerAnim.SetDash(true);

        _rigidbody.useGravity = false;

        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            _rigidbody.velocity = new Vector3(direction.x * dashSpeed, 0f, direction.z * dashSpeed);
            yield return null;
        }

        _rigidbody.useGravity = true;
        player.ChangeState(PlayerState.Idle);
        playerAnim.SetDash(false);

        player.OnInvincible = false;
        
        // 대시 쿨타임 적용
        yield return new WaitForSeconds(cooldown);
        player.CanDash = true;
    }
}
