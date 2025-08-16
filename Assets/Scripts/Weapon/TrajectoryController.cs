using UnityEngine;

public class TrajectoryController : MonoBehaviour
{
    private LineRenderer trajectoryLine;
    private Transform throwPoint;
    [SerializeField] private float minThrowForce = 2f;
    [SerializeField] private float maxThrowForce = 10f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private int pointCount = 30;
    [SerializeField] private float timeStep = 0.1f;
    private Camera cam;

    public void Init(Transform throwPoint)
    {
        trajectoryLine = GetComponent<LineRenderer>();
        this.throwPoint = throwPoint;

        cam = Camera.main;
        Hide();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    void Update()
    {
        if (!trajectoryLine.enabled) return;

        Vector3 direction = GetAimDirectionForce(out float force);
        Vector3[] points = new Vector3[pointCount];

        Vector3 pos = throwPoint.position;
        Vector3 velocity = direction * force;
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = pos;
            pos += velocity * timeStep;
            velocity += Physics.gravity * timeStep;
        }

        trajectoryLine.positionCount = pointCount;
        trajectoryLine.SetPositions(points);
    }

    public Vector3 GetAimDirectionForce(out float force)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, throwPoint.position);
        if (groundPlane.Raycast(ray, out float enter))
        {
            // 평면에서 위쪽으로 45에 향하는 벡터 구하기
            Vector3 target = ray.GetPoint(enter);
            Vector3 dir = (target - throwPoint.position).normalized;

            Quaternion tilt = Quaternion.AngleAxis(-45f, Vector3.Cross(Vector3.up, dir));
            Vector3 finalDirection = tilt * dir;

            force = GetThrowForce(target);
            return finalDirection;
        }

        force = minThrowForce;
        return transform.forward;
    }

    public float GetThrowForce(Vector3 targetPos)
    {
        float distance = Vector3.Distance(throwPoint.position, targetPos);
        float t = Mathf.Clamp01(distance / maxDistance);
        return Mathf.Lerp(minThrowForce, maxThrowForce, t);
    }
}