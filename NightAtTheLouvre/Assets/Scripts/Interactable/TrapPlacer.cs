using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapPlacer : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("The trap prefab to be placed.")]
    public GameObject trapPrefab;
    
    [Tooltip("The action to trigger placing a trap.")]
    public InputAction placeTrapAction;

    [Tooltip("The action to trigger picking up a trap.")]
    public InputAction pickupTrapAction;

    [Tooltip("The maximum number of traps that can be active at once.")]
    public int maxTraps = 3;

    [Tooltip("The cooldown in seconds between placing traps.")]
    public float trapPlacementCooldown = 5.0f;

    private List<GameObject> _activeTraps = new List<GameObject>();
    private float _lastTrapTime = -100f;
    private List<Trap> _pickupableTraps = new List<Trap>();

    private void OnEnable()
    {
        placeTrapAction.Enable();
        pickupTrapAction.Enable();
    }

    private void OnDisable()
    {
        placeTrapAction.Disable();
        pickupTrapAction.Disable();
    }

    void Update()
    {
        // Clean up any destroyed traps from the list
        _activeTraps.RemoveAll(item => item == null);

        if (placeTrapAction.WasPressedThisFrame())
        {
            TryPlaceTrap();
        }

        if (pickupTrapAction.WasPressedThisFrame())
        {
            TryPickupTrap();
        }
    }

    private void TryPlaceTrap()
    {
        if (trapPrefab == null)
        {
            Debug.LogWarning("Trap prefab is not assigned in the TrapPlacer script.");
            return;
        }

        if (_activeTraps.Count >= maxTraps)
        {
            Debug.Log("Cannot place more traps. Maximum limit reached.");
            return;
        }

        if (Time.time < _lastTrapTime + trapPlacementCooldown)
        {
            Debug.Log("Trap placement is on cooldown.");
            return;
        }

        _lastTrapTime = Time.time;
        GameObject newTrap = Instantiate(trapPrefab, transform.position, Quaternion.identity);
        _activeTraps.Add(newTrap);
    }

    private void TryPickupTrap()
    {
        if (_pickupableTraps.Count > 0)
        {
            // Pick up the most recently entered trap trigger
            Trap trapToPickup = _pickupableTraps[_pickupableTraps.Count - 1];
            if (trapToPickup != null)
            {
                _pickupableTraps.Remove(trapToPickup);
                // The active traps list will be cleaned up in the next Update frame.
                Destroy(trapToPickup.gameObject);
            }
        }
    }

    public void RegisterPickupableTrap(Trap trap)
    {
        if (!_pickupableTraps.Contains(trap))
        {
            _pickupableTraps.Add(trap);
        }
    }

    public void UnregisterPickupableTrap(Trap trap) => _pickupableTraps.Remove(trap);
}