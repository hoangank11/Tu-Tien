using System.Collections;
using UnityEngine;

public class PlayerVFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [Range(.01f, .2f)]
    [SerializeField] private float imgEchoInterval = .05f;
    [SerializeField] private GameObject imgEchoPrefab;
    private Coroutine imgEchoCo;

    public void CreateEffectOf(GameObject effect, Transform target)
    {
        Instantiate(effect, target.position, Quaternion.identity);
    }

    public void DoImgEchoEffect(float duration)
    {
        if (imgEchoCo != null)
            StopCoroutine(imgEchoCo);

        imgEchoCo = StartCoroutine(ImgEchoEffectCo(duration));
    }


    private IEnumerator ImgEchoEffectCo(float duration)
    {
        float timeTracker = 0f;
        while (timeTracker < duration)
        {
            CreateImgEcho();
            yield return new WaitForSeconds(imgEchoInterval);
            timeTracker += imgEchoInterval;
        }
    }

    private void CreateImgEcho()
    {
        GameObject imgEcho = Instantiate(imgEchoPrefab, transform.position, transform.rotation);
        imgEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;


    }
}
