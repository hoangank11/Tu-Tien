using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    private Entity entity;

    [Header("On Damage VFX")]
    [SerializeField] private Material onDamageVFX;
    [SerializeField] private float onDamageDuration = .1f;
    private Material originalM;
    private Coroutine onDamageVFXCoroutine;

    [Header("Damage VFX")]
    [SerializeField] private Color hitVfxColor;
    [SerializeField] private GameObject HitVFX;
    [SerializeField] private GameObject CritVFX;

    [Header("Elemental Color")]
    [SerializeField] private Color iceVfx = Color.cyan;
    [SerializeField] private Color fireVfx = Color.red;
    [SerializeField] private Color toxicVfx = Color.green;
    [SerializeField] private Color elecVfx = Color.yellow;
    [SerializeField] private Color windVfx = Color.white;
    [SerializeField] private Color earthVfx = new Color(.55f, .32f, .15f);
    [SerializeField] private Color woodVfx = new Color(.13f, .55f, .13f);
    [SerializeField] private Color waterVfx = new Color(.1f, .45f, .85f);

    // Mỗi loại elemental effect có coroutine VFX riêng -> có thể chạy song song,
    // dừng loại này không ảnh hưởng tới loại khác đang active.
    private Coroutine iceVfxCo;
    private Coroutine fireVfxCo;
    private Coroutine toxicVfxCo;
    private Coroutine elecVfxCo;
    private Coroutine windVfxCo;
    private Coroutine earthVfxCo;
    private Coroutine woodVfxCo;
    private Coroutine waterVfxCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalM = sr.material;

    }

    public void PlayVfx(float duration, ElementalType elemental)
    {
        switch (elemental)
        {
            case ElementalType.Ice:
                if (iceVfxCo != null) StopCoroutine(iceVfxCo);
                iceVfxCo = StartCoroutine(PlayStatusVfxCo(duration, iceVfx));
                break;
            case ElementalType.Fire:
                if (fireVfxCo != null) StopCoroutine(fireVfxCo);
                fireVfxCo = StartCoroutine(PlayStatusVfxCo(duration, fireVfx));
                break;
            case ElementalType.Toxic:
                if (toxicVfxCo != null) StopCoroutine(toxicVfxCo);
                toxicVfxCo = StartCoroutine(PlayStatusVfxCo(duration, toxicVfx));
                break;
            case ElementalType.Electric:
                if (elecVfxCo != null) StopCoroutine(elecVfxCo);
                elecVfxCo = StartCoroutine(PlayStatusVfxCo(duration, elecVfx));
                break;
            case ElementalType.Wind:
                if (windVfxCo != null) StopCoroutine(windVfxCo);
                windVfxCo = StartCoroutine(PlayStatusVfxCo(duration, windVfx));
                break;
            case ElementalType.Earth:
                if (earthVfxCo != null) StopCoroutine(earthVfxCo);
                earthVfxCo = StartCoroutine(PlayStatusVfxCo(duration, earthVfx));
                break;
            case ElementalType.Wood:
                if (woodVfxCo != null) StopCoroutine(woodVfxCo);
                woodVfxCo = StartCoroutine(PlayStatusVfxCo(duration, woodVfx));
                break;
            case ElementalType.Water:
                if (waterVfxCo != null) StopCoroutine(waterVfxCo);
                waterVfxCo = StartCoroutine(PlayStatusVfxCo(duration, waterVfx));
                break;
        }
    }

    // Dừng VFX của MỘT loại elemental cụ thể (không đụng tới các loại khác đang chạy song song)
    public void StopVfx(ElementalType elemental)
    {
        switch (elemental)
        {
            case ElementalType.Ice:
                if (iceVfxCo != null) StopCoroutine(iceVfxCo);
                iceVfxCo = null;
                break;
            case ElementalType.Fire:
                if (fireVfxCo != null) StopCoroutine(fireVfxCo);
                fireVfxCo = null;
                break;
            case ElementalType.Toxic:
                if (toxicVfxCo != null) StopCoroutine(toxicVfxCo);
                toxicVfxCo = null;
                break;
            case ElementalType.Electric:
                if (elecVfxCo != null) StopCoroutine(elecVfxCo);
                elecVfxCo = null;
                break;
            case ElementalType.Wind:
                if (windVfxCo != null) StopCoroutine(windVfxCo);
                windVfxCo = null;
                break;
            case ElementalType.Earth:
                if (earthVfxCo != null) StopCoroutine(earthVfxCo);
                earthVfxCo = null;
                break;
            case ElementalType.Wood:
                if (woodVfxCo != null) StopCoroutine(woodVfxCo);
                woodVfxCo = null;
                break;
            case ElementalType.Water:
                if (waterVfxCo != null) StopCoroutine(waterVfxCo);
                waterVfxCo = null;
                break;
        }

        // Chỉ reset màu về trắng khi KHÔNG còn loại elemental nào khác đang active
        if (iceVfxCo == null && fireVfxCo == null && toxicVfxCo == null && elecVfxCo == null
            && windVfxCo == null && earthVfxCo == null && woodVfxCo == null && waterVfxCo == null)
            sr.color = Color.white;
    }

    public void StopAllVfx()
    {
        StopAllCoroutines();
        iceVfxCo = null;
        fireVfxCo = null;
        toxicVfxCo = null;
        elecVfxCo = null;
        windVfxCo = null;
        earthVfxCo = null;
        woodVfxCo = null;
        waterVfxCo = null;
        sr.color = Color.white;
        sr.material = originalM;
    }


    private IEnumerator PlayStatusVfxCo(float duration, Color color)
    {
        float tickInterval = .25f; // thời gian của các vfx hoạt động mỗi frame
        float timer = 0; // thời gian hoạt động của vfx effect

        Color lightColor = color * 1.2f;
        Color darkColor = color * .8f;

        bool toggle = false;

        while (timer < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;
            yield return new WaitForSeconds(tickInterval);
            timer = timer + tickInterval;
        }
        sr.color = Color.white;
    }

    public void CreateHitVFX(Transform target, bool isCrit, ElementalType elemental)
    {
        GameObject hitPrefab = isCrit ? CritVFX : HitVFX;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        //vfx.GetComponentInChildren<SpriteRenderer>().color = UpdateHitColor(elemental); 
    }

    public void PlayOnDamageVFX()
    {
        if (onDamageVFXCoroutine != null)
            StopCoroutine(onDamageVFXCoroutine);

        onDamageVFXCoroutine = StartCoroutine(OnDamageVFX());
    }

    public Color UpdateHitColor(ElementalType elemental)
    {
        switch (elemental)
        {
            case ElementalType.Ice: return iceVfx;
            case ElementalType.Fire: return fireVfx;
            case ElementalType.Toxic: return toxicVfx;
            case ElementalType.Electric: return elecVfx;
            case ElementalType.Wind: return windVfx;
            case ElementalType.Earth: return earthVfx;
            case ElementalType.Wood: return woodVfx;
            case ElementalType.Water: return waterVfx;
            default:
                return Color.white;
        }
    }

    private IEnumerator OnDamageVFX()
    {
        sr.material = onDamageVFX;
        yield return new WaitForSeconds(onDamageDuration);
        sr.material = originalM;
    }
}
