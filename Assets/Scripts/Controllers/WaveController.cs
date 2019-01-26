using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveController : MonoBehaviour
{
    [SerializeField]
    private float minLevel = -6f;

    [SerializeField]
    private float maxLevel = 2f;

    [SerializeField]
    private float inflowDuration = 20f;

    [SerializeField]
    private float outflowDuration = 5f;

    [SerializeField]
    private GameObject externalWave;

    [SerializeField]
    private GameObject internalWave;

    public List<WallSlot> WallSlots = new List<WallSlot>();
    private float tightness = 0f;

    private float previousExternalY = 0f;

    private void Start()
    {
        //StartInflow();
    }

    public void StartInflow()
    {
        previousExternalY = minLevel;
        internalWave.transform.DOLocalMoveY(minLevel, 0f);
        externalWave.transform.DOLocalMoveY(minLevel, 0f);
        externalWave.transform.DOLocalMoveY(maxLevel, inflowDuration)
            .OnComplete(() => { StartOutflow(); })
            .OnUpdate(()=> { OnUpdate(); })
            .SetDelay(0.1f);
    }

    public void StartOutflow()
    {
        externalWave.transform.DOLocalMoveY(maxLevel, 0f);
        internalWave.transform.DOLocalMoveY(minLevel, outflowDuration);
        externalWave.transform.DOLocalMoveY(minLevel, outflowDuration);//.OnComplete(()=> { StartInflow();  });
    }

    void OnUpdate()
    {
        // take relevant wall slots
        List<WallSlot> relevantWallSlots = new List<WallSlot>();
        for (int i = 0; i < WallSlots.Count; i++)
        {
            if (WallSlots[i].gameObject.transform.position.y <= externalWave.transform.position.y)
            {
                relevantWallSlots.Add(WallSlots[i]);
            }
        }

        if (relevantWallSlots.Count == 0)
        {
            return;
        }

        float total = 0f;
        for (int i = 0; i < relevantWallSlots.Count; i++)
        {
            total += relevantWallSlots[i].Value;
        }

        float average = total / (float)relevantWallSlots.Count;
        float externalRaise = externalWave.transform.localPosition.y - previousExternalY;
        float internalRaise = externalRaise * (100f - average) / 100f;
        UnityEngine.Debug.Log(average + " " + internalRaise + " " + externalRaise);

        Vector3 pos = internalWave.transform.localPosition;
        pos.y += internalRaise;
        internalWave.transform.localPosition = pos;

        previousExternalY = externalWave.transform.localPosition.y;
    }
}
