using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {
    
    public int healingAmount = 1;

    public int GetHealingAmount() {
        return healingAmount;
    }
}
