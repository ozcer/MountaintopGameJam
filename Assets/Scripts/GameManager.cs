using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Cinemachine.CinemachineVirtualCamera Camera;
    public CinemachineTargetGroup TargetGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ChangeCameraTarget(Transform newTarget)
    {
        if (Camera != null)
        {
            Camera.m_Follow = newTarget;
        }
    }

    public void ResetAndAddTargets(Transform[] targets)
    {
        if (Camera == null || TargetGroup == null)
        {
            return;
        }

        Camera.m_Follow = TargetGroup.gameObject.transform;
        Camera.m_LookAt = TargetGroup.gameObject.transform;
        TargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

        foreach (Transform target in targets)
        {
            TargetGroup.AddMember(target, 1, 0);
        }
    }

    public void ResetTargets()
    {
        if (TargetGroup == null)
        {
            return;
        }

        TargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
    }
}
