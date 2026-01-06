using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public BuffType buffType;
    public float value;
    public float duration;
    public float elapsedTime;

    public StatusEffect(BuffType type, float value, float duration = 0)
    {
        this.buffType = type;
        this.value = value;
        this.duration = duration;
        this.elapsedTime = 0f;
    }

    public bool IsPermanent => duration <= 0;
}
