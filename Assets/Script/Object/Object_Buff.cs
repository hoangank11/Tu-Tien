using System.Collections;
using UnityEngine;

public class Object_Buff : MonoBehaviour
{
    private PlayerInformation startToMods;

    [Header("Tinh chỉnh vị trí của object X & Y")]
    [SerializeField] private float YSpeed = 1f;
    [SerializeField] private float YRange = .1f;
    private Vector3 startPosition;

    [Header("Buff Details")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4f;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float yOffSet = Mathf.Sin(Time.time * YSpeed) * YRange;
        transform.position = startPosition + new Vector3(0, yOffSet);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        startToMods = collision.GetComponent<PlayerInformation>();

        if (startToMods.CanApplyBuff(buffName))
        {
            startToMods.ApplyBuff(buffs, buffDuration, buffName);
            Destroy(gameObject);
        }

    }

}
