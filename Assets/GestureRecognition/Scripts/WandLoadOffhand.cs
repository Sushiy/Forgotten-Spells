﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MagicWand))]
public class WandLoadOffhand : MonoBehaviour {

    private MagicWand m_wand;

    void Awake()
    {
        m_wand = GetComponent<MagicWand>();
        Assert.IsNotNull(m_wand);
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            if (m_wand.IsWandLoaded())
            {
                MagicWand offhandWand = other.GetComponentInChildren<MagicWand>();
                Assert.IsNotNull(offhandWand);
                offhandWand.LoadWand(m_wand.LoadedSpell);
                m_wand.UnLoadWand();
            }
        }
    }
}
