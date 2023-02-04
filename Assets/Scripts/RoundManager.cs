using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{

    private ResourceManager resourceManager;

    private void Awake()
    {
        resourceManager = GetComponent<ResourceManager>();
    }


    public void StartRound()
    {
        var gatheredResources = resourceManager.GatherAllResources();

        Debug.Log($"Gathered resources: {gatheredResources}");

        // TODO Add gathered resources to spendable resource counts

        // TODO Pay upkeep from the spendable resources

        // TODO Check for forced actions (e.g. not enough upkeep -> destroy plant parts)

    }


}
