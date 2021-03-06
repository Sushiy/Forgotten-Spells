﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gesture;

public abstract class Spell : MonoBehaviour
{
    public struct CastingData
    {
        //WandData
        public Vector3 _v3WandPos;
        public Quaternion _qWandRot;
        public Vector3 _v3WandVelocity;

        //HandsData
        public int _iCastingHandIndex;

        //PlayerData
        public GameObject _goPlayer;
    }

    [SerializeField]
    protected GameObject m_goLoadedFX;
    [SerializeField]
    protected SpellType m_spelltypeThis;
    public SpellType SpellType { get { return m_spelltypeThis; } }

    //[SerializeField]
    //protected gestureTypes m_gestureTypeThis;
    //public gestureTypes Gesture{ get { return m_gestureTypeThis; } }
    public bool m_bIsServer = false;

    protected CastingData thisCastingData;

    protected Vector3 m_v3Target;
    public Vector3 TargetPosition { get { return m_v3Target; } }
    
    public abstract void Fire(CastingData spelldata);
    public abstract void PlayerHit(GameObject _goPlayer);
    public abstract void SpellHit();
    public abstract void Deactivate();

    public GameObject LoadedFX()
    {
        return m_goLoadedFX;
    }

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {

    }
}
