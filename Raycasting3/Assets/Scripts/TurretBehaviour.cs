using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    enum State { Turning, ReadyingShot, ReadyingTurn, Destroyed }

    public GameObject projectilePrefab;

    private HealthBehaviour healthBehaviour;

    private State currentState = State.ReadyingTurn;
    private float timeToReadyShot = 3;
    private float timeToReadyTurn = 1;
    private float elapsedTime = 0;
    private float degreesTurned = 0;

    void Start()
    {
        healthBehaviour = GetComponentInParent<HealthBehaviour>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (healthBehaviour.GetCurrentHealth() <= 0) currentState = State.Destroyed;

        switch (currentState) {
            case State.Turning:
                transform.Rotate(0, 1, 0, Space.Self);
                degreesTurned += 1;
                if (degreesTurned >= 90) {
                    currentState = State.ReadyingShot;
                    degreesTurned = 0;
                }
                break;
            case State.ReadyingShot:
                if (elapsedTime > timeToReadyShot) {
                    Shoot();
                    currentState = State.ReadyingTurn;
                    elapsedTime = 0;
                }
                break;
            case State.ReadyingTurn:
                if (elapsedTime > timeToReadyTurn) {
                    currentState = State.Turning;
                    elapsedTime = 0;
                }
                break;
            case State.Destroyed:
                break;
            default:
                break;
        }
    }

    private void Shoot()
    {
        Vector3 projectilePosition = transform.position;
        projectilePosition += transform.forward * projectilePrefab.GetComponent<SphereCollider>().radius;
        projectilePosition += transform.forward * 0.3f;
        projectilePosition += transform.up * 0.4f;
        Instantiate(projectilePrefab, projectilePosition, transform.rotation);
    }
}
