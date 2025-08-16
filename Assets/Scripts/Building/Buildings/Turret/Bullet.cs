using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float 
        speed = 10,     // 총알 속도
        bulletLifeCycle = 5; // 총알 생명주기
    // 타겟인 적의 레이어
    [SerializeField] LayerMask enemyLayer;

    // 공격력
    float atk;
    // 이번에 공격할 적
    Transform target;
    Vector3 moveVec;

    // 총알이 생성될 때 터렛에서 초기화해야 할 값들
    public void InitBullet(float atk, Transform target)
    {
        this.atk = atk;
        this.target = target;
    }


    private void Start()
    {
        // Time.deltaTime과는 다르게 fixedDeltaTime은 고정값이기에 미리 곱해두면 연산을 줄일 수 있다
        speed *= Time.fixedDeltaTime;
        Destroy(gameObject, bulletLifeCycle);
        if (target != null)
        {
            if (target.gameObject.activeSelf)
            {
                moveVec = (target.position - transform.position).normalized;
                transform.forward = moveVec;
            }
        }
    }

    private void FixedUpdate()
    {

        transform.position += moveVec * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 맞은 오브젝트가 적이라면, 대미지 가함
        if ((1 << collision.gameObject.layer) == enemyLayer)
        {
            IDamageable damageable = collision.transform.GetComponent<IDamageable>();
            if(damageable != null) 
            {
                damageable.TakeDamage(atk);
            }

            Destroy(gameObject);
        }

        if (collision.transform.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
