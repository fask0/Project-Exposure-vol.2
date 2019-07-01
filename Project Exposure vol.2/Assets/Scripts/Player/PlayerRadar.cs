﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadar : MonoBehaviour
{
    [SerializeField]
    private GameObject _objToActivate;

    private GameObject _target;
    private DateTime _activationTime = DateTime.MaxValue;
    private bool _hasBeenActivated = false;

    private GameObject _artifactParent;
    private ArtifactParent _artifactParentScript;

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.SoundWaveManager.onFishScanEvent += ResetRadar;
        SetTarget(SingleTons.QuestManager.GetCurrentTarget());

        _artifactParent = GameObject.FindGameObjectWithTag("ArtifactParent");
        _artifactParentScript = _artifactParent.GetComponent<ArtifactParent>();

        _activationTime = DateTime.Now.AddSeconds(_artifactParentScript.GetArrowTime(SingleTons.QuestManager.GetCurrentTargetIndex()));
    }

    private void ResetRadar(GameObject pGameObject)
    {
        if (pGameObject.tag.Contains("Target"))
        {
            _activationTime = DateTime.Now.AddSeconds(_artifactParentScript.GetArrowTime(SingleTons.QuestManager.GetCurrentTargetIndex()));
            _objToActivate.SetActive(false);
            _hasBeenActivated = false;
            SetTarget(SingleTons.QuestManager.GetCurrentTarget());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SingleTons.QuestManager.GetCurrentTarget() == null)
        {
            _activationTime = DateTime.MaxValue;
            _objToActivate.SetActive(false);
            _hasBeenActivated = false;
            return;
        }

        if (DateTime.Now > _activationTime)
        {
            if (!_hasBeenActivated)
            {
                _objToActivate.SetActive(true);
                _hasBeenActivated = true;
            }

            transform.LookAt(_target.transform);
        }
    }

    public void SetTarget(GameObject pTarget)
    {
        _target = pTarget;
    }

    public GameObject GetTarget()
    {
        return _target;
    }
}
