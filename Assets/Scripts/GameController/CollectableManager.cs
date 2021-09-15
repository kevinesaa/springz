using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour {

    public GameObject panelCollectionFinish;
    private Collectable[] collectables;
    private event Action OnCollectionFinish;
    

    private void Awake()
    {
        OnCollectionFinish += DebugCollectionFinish;
        collectables = FindObjectsOfType<Collectable>();
        if (collectables != null && collectables.Length > 0)
        {
            for(int i=0; i < collectables.Length; i++)
            {
                collectables[i].Index = i;
                collectables[i].OnCaught += CollectableCaught;
            }
        }
    }


    private void CollectableCaught(int index)
    {
        bool finish = true;
        foreach(Collectable item in collectables)
        {
            finish = finish && item.IsWasCollect;
            if (!finish)
                return;
        }

        if (finish && OnCollectionFinish != null)
            OnCollectionFinish();
    }

    private void DebugCollectionFinish()
    {
        panelCollectionFinish.SetActive(true);
    }
}
