using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 5f;
    public float rubberBandStrength = 0.1f;
    public float rubberBandDamping = 0.9f;

    private Transform player;
    private Transform plane;
    private Vector3 velocity;
    private Vector3 offset;

    private bool followingOverride = false;
    private float overrideTimer = 0f;
    private float overrideDuration = 6f;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();

        inputActions.Player.PlaneBomb.started += ctx => TriggerCameraOverride();
        inputActions.Player.PlaneSoldier.started += ctx => TriggerCameraOverride();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            offset = transform.position - player.position;
        }
        else
        {
            Debug.LogError("No GameObject found with tag 'Player'");
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos;

        if (followingOverride && plane != null)
        {
            overrideTimer += Time.deltaTime;
            if (overrideTimer >= overrideDuration)
            {
                followingOverride = false;
                overrideTimer = 0f;
            }

            targetPos = plane.position + offset;
        }
        else
        {
            targetPos = player.position + offset;
        }

        Vector3 direction = targetPos - transform.position;
        Vector3 springForce = direction * rubberBandStrength;

        velocity += springForce;
        velocity *= rubberBandDamping;

        transform.position += velocity * followSpeed * Time.deltaTime;
    }

    private void TriggerCameraOverride()
    {
        GameObject planeObj = GameObject.FindGameObjectWithTag("Plane");
        if (planeObj != null)
        {
            plane = planeObj.transform;
            overrideTimer = 0f;
            followingOverride = true;
        }
        else
        {
            Debug.LogWarning("No GameObject found with tag 'Plane'");
        }
    }
}
