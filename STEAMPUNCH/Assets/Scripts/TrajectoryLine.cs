using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{

    [SerializeField] private int segmentCount = 100;
    [SerializeField] private float lineLength = 2f;

    private Vector2[] segments;
    private LineRenderer lineRenderer;

    private PlayerController controller;
    private ThrowableEnemy enemy;
    private float enemyMass;
    private float enemyGravityScale;
    private Vector2 holdingPoint;
    private Vector2 throwForce;

    // Start is called before the first frame update
    void Start()
    {
        segments = new Vector2[segmentCount];

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;

        controller = this.gameObject.GetComponentInParent<PlayerController>();

        holdingPoint = controller.holdingPosition;
        enemy = controller.NearbyKnockedEnemy;

        enemyMass = enemy.Mass;
        enemyGravityScale = enemy.GravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        holdingPoint = controller.holdingPosition;
        enemy = controller.NearbyKnockedEnemy;

        enemyMass = enemy.Mass;
        enemyGravityScale = enemy.GravityScale;

        Vector2 startPos = holdingPoint;
        segments[0] = startPos;
        lineRenderer.SetPosition(0, startPos);

        // Jank bs
        throwForce = controller.GetThrowForce() / 50f;

        Vector2 startingVelo = throwForce / enemyMass;

        for (int i = 1; i < segmentCount; i++)
        {
            float timeOffset = i * Time.fixedDeltaTime * lineLength;

            Vector2 gravityOffset = 0.5f * Physics2D.gravity * enemyGravityScale * Mathf.Pow(timeOffset, 2);

            segments[i] = segments[0] + (startingVelo * timeOffset) + gravityOffset;

            lineRenderer.SetPosition(i, new Vector3(segments[i].x, segments[i].y, -1.0f));
        }
    }
}
