using UnityEngine;

public class HitVFX : MonoBehaviour
{
    [SerializeField] private float destroyDelay = .5f;
    [SerializeField] private float xMinOffSet = -1f;
    [SerializeField] private float xMaxOffSet = 1.0f;
    [Space]
    [SerializeField] private float yMinOffSet = -1f;
    [SerializeField] private float yMaxOffSet = 1.0f;
    private void Start()
    {
        RandomVFX();
        Destroy(gameObject, destroyDelay);
    }

    private void RandomVFX()
    {
        float xOffset = Random.Range(xMinOffSet, xMaxOffSet);
        float yOffset = Random.Range(yMinOffSet, yMaxOffSet);

        transform.position = transform.position + new Vector3(xOffset, yOffset);
    }
}
