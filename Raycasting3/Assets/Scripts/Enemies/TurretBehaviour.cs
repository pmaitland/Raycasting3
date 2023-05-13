using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{

    private enum State
    {
        TURNING,
        READYINGSHOT,
        READYINGTURN,
        DESTROYED
    }

    private Health _health;

    private State _currentState = State.READYINGTURN;

    private const float TIME_TO_READY_SHOT = 3;
    private const float TIME_TO_READY_TURN = 1;

    private float _elapsedTime = 0;
    private float _degreesTurned = 0;

    public GameObject ProjectilePrefab;

    void Start()
    {
        _health = GetComponentInParent<Health>();
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_health.CurrentHealth <= 0) _currentState = State.DESTROYED;

        switch (_currentState)
        {
            case State.TURNING:
                transform.Rotate(0, 1, 0, Space.Self);
                _degreesTurned += 1;
                if (_degreesTurned >= 90)
                {
                    _currentState = State.READYINGSHOT;
                    _degreesTurned = 0;
                }
                break;
            case State.READYINGSHOT:
                if (_elapsedTime > TIME_TO_READY_SHOT)
                {
                    Shoot();
                    _currentState = State.READYINGTURN;
                    _elapsedTime = 0;
                }
                break;
            case State.READYINGTURN:
                if (_elapsedTime > TIME_TO_READY_TURN)
                {
                    _currentState = State.TURNING;
                    _elapsedTime = 0;
                }
                break;
            case State.DESTROYED:
                break;
            default:
                break;
        }
    }

    private void Shoot()
    {
        Vector3 projectilePosition = transform.position;
        projectilePosition += transform.forward * ProjectilePrefab.GetComponent<SphereCollider>().radius;
        projectilePosition += transform.forward * 0.3f;
        projectilePosition += transform.up * 0.4f;

        GameObject projectile = Instantiate(ProjectilePrefab, projectilePosition, transform.rotation);
        projectile.GetComponent<ProjectileBehaviour>().Creator = gameObject;
    }
}
