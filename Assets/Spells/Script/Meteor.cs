﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Spell
{
    private Rigidbody m_rigidThis;
    private int m_iDamage = 20;
    public GameObject explosionPrefab;
    public float m_fVelocityMultiplier = 2.0f;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Deactivate()
    {
        Destroy(gameObject);
        transform.parent.GetComponent<MeteorSpawner>().ChildDestroyed();
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("Spell: Fire!");
        MP_VR_PlayerController player = spelldata._goPlayer.GetComponent<MP_VR_PlayerController>();
        Vector3 targetPosition = Vector3.zero;
        if (player.Opponent != null)
        {
            m_transTarget = player.Opponent.GetComponentInChildren<HomingTarget>().transform;
            targetPosition = m_transTarget.position;
        }

        gameObject.transform.position = spelldata._v3WandPos;
        m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.velocity = ((targetPosition - transform.position) * m_fVelocityMultiplier);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        GameObject goOther = collision.gameObject;
        if (goOther.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHit(goOther);
        }
        Deactivate();
    }

    public override void PlayerHit(GameObject _goPlayer)
    {
        _goPlayer.GetComponentInParent<MP_Health>().TakeDamage(m_iDamage);
    }

    public override void SpellHit()
    {
        throw new NotImplementedException();
    }
}