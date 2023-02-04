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

        // TODO Add gather resources to hand

        // TODO Pay upkeep from hand

        // TODO Check for forced actions (e.g. not enough upkeep -> destry plant parts)

    }

    



}
