using UnityEngine;

public class ProjectileArc : MonoBehaviour 
{
    [SerializeField]
    int iterations = 20;

    [SerializeField]
    Color errorColor;

    private Color initialColor;
    private LineRenderer lineRenderer;
    Material lineMaterial;
    public float _speed = -0.07f;
    Vector2 offset;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        initialColor = lineRenderer.material.color;
        lineMaterial = lineRenderer.sharedMaterial;
    }
    //public Vector3 offsetPosition = new Vector3(0.3231883f, 0.5568783f, 0.1157932f);
    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool valid)
    {
        offset = lineMaterial.mainTextureOffset;
        offset.x += _speed;
        lineMaterial.mainTextureOffset = offset;


        var parent = transform.parent;
        transform.parent = null;
        transform.localScale = Vector3.one;
        transform.parent = parent;

        Vector2[] arcPoints = ProjectileMath.ProjectileArcPoints(iterations, speed, distance, gravity, angle);        
        Vector3[] points3d = new Vector3[arcPoints.Length];

        for (int i = 0; i < arcPoints.Length; i++)
        {
            points3d[i] = new Vector3(0, arcPoints[i].y, arcPoints[i].x);
        }

        lineRenderer.positionCount = arcPoints.Length;
        lineRenderer.SetPositions(points3d);

        transform.rotation = Quaternion.LookRotation(direction);

        lineRenderer.material.color = valid ? initialColor : errorColor;
    }
}
