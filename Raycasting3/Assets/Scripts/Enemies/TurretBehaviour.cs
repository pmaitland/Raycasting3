using UnityEngine;

public class TurretBehaviour : MonoBehaviour {

    private enum State {
        TURNING,
        READYINGSHOT,
        READYINGTURN,
        DESTROYED
    }

    private State currentState = State.READYINGTURN;
    private float timeToReadyShot = 3;
    private float timeToReadyTurn = 1;
    private float elapsedTime = 0;
    private float degreesTurned = 0;
    
    private Health health;

    public GameObject projectilePrefab;

    void Start() {
        health = GetComponentInParent<Health>();
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (health.GetCurrentHealth() <= 0) currentState = State.DESTROYED;

        switch (currentState) {
            case State.TURNING:
                transform.Rotate(0, 1, 0, Space.Self);
                degreesTurned += 1;
                if (degreesTurned >= 90) {
                    currentState = State.READYINGSHOT;
                    degreesTurned = 0;
                }
                break;
            case State.READYINGSHOT:
                if (elapsedTime > timeToReadyShot) {
                    Shoot();
                    currentState = State.READYINGTURN;
                    elapsedTime = 0;
                }
                break;
            case State.READYINGTURN:
                if (elapsedTime > timeToReadyTurn) {
                    currentState = State.TURNING;
                    elapsedTime = 0;
                }
                break;
            case State.DESTROYED:
                break;
            default:
                break;
        }
    }

    private void Shoot() {
        Vector3 projectilePosition = transform.position;
        projectilePosition += transform.forward * projectilePrefab.GetComponent<SphereCollider>().radius;
        projectilePosition += transform.forward * 0.3f;
        projectilePosition += transform.up * 0.4f;
        GameObject projectile = Instantiate(projectilePrefab, projectilePosition, transform.rotation);
        projectile.GetComponent<ProjectileBehaviour>().SetCreator(gameObject);
    }
}
