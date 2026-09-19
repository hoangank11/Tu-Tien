using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_LoadScreen : MonoBehaviour
{
    [Header("Load Screen Images")]
    [Tooltip("Kéo 5 hình ảnh loading vào đây. Mỗi lần load sẽ random 1 trong số này.")]
    [SerializeField] private Sprite[] loadScreenImages = new Sprite[5];

    private Image img;
    public Coroutine fadeEffectCo { get; private set; }


    private void Awake()
    {
        img = GetComponent<Image>();
        SetRandomLoadScreenImage();
        img.color = new Color(1, 1, 1, 1);
    }

    /// Chọn ngẫu nhiên 1 trong các ảnh loading
    private void SetRandomLoadScreenImage()
    {
        if (loadScreenImages == null || loadScreenImages.Length == 0)
            return;

        int randomIndex = Random.Range(0, loadScreenImages.Length);
        Sprite randomSprite = loadScreenImages[randomIndex];

        if (randomSprite != null)
            img.sprite = randomSprite;
    }

    public void DoFadeIn(float duration = 1)
    {
        img.color = new Color(1, 1, 1, 1);
        FadeEffect(0f, duration);
    }

    public void DoFadeOut(float duration = 1)
    {
        // Mỗi lần chuẩn bị che màn hình (bắt đầu loading) sẽ đổi random ảnh mới
        SetRandomLoadScreenImage();
        img.color = new Color(1, 1, 1, 0);
        FadeEffect(1f, duration);
    }

    private void FadeEffect(float targetAlpha, float duration)
    {
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);
        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }

    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        float startAlpha = img.color.a;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            var color = img.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            img.color = color;
            yield return null;

        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, targetAlpha);
    }

}
