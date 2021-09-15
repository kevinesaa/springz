using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Collectable : MonoBehaviour {

    public event Action<int> OnCaught;
    public bool IsWasCollect { get; set; }
    public int Index { get; set; }

    private void Awake()
    {
        IsWasCollect = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsWasCollect = true;
            if (OnCaught != null)
                OnCaught(Index);
            gameObject.SetActive(false);
        }
    }
}
