using UnityEngine;

public interface ICounterable
{
    public bool canBeCounter { get; }
    public void HandleCounter();
}
