using UnityEngine;

public class ItemEffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDes;
    protected Player player;
    protected Enemy enemy;

    public virtual bool CanBeUse()
    {
        return true;
    }

    public virtual void ExecuteEffect()
    {

    }

    public virtual void Subscribe(Player player)
    {
        this.player = player;
    }

    public virtual void Unsubscribe()
    {

    }

}
