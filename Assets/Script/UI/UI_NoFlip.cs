using UnityEngine;

public class UI_NoFlip : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        entity.OnFlip += HPFlip;
    }

    private void OnDisable()
    {
        entity.OnFlip -= HPFlip;
    }
    private void HPFlip() => transform.rotation = Quaternion.identity;
}
