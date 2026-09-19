using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    [Header("SFX Settings")]
    [SerializeField] protected float soundDistance = 15f;

    [SerializeField]
    protected SFXPriority sfxPriority =
        SFXPriority.Normal;

    [SerializeField] protected bool showGizmo;

    protected virtual void Awake()
    {
    }

    #region PLAY SFX

    protected void PlaySFX(string sfxName)
    {
        if (string.IsNullOrEmpty(sfxName))
            return;

        if (AudioManager.instance == null)
            return;

        AudioManager.instance.PlaySFX(
            sfxName,
            transform,
            soundDistance,
            sfxPriority
        );
    }

    #endregion

    #region LOOP SFX

    protected void PlayLoopingSFX(
        string sfxName,
        ref float timer,
        float interval,
        bool isActive)
    {
        if (!isActive)
        {
            timer = 0f;
            return;
        }

        if (string.IsNullOrEmpty(sfxName))
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlaySFX(sfxName);

            timer = Mathf.Max(
                0.01f,
                interval
            );
        }
    }

    #endregion

    #region GIZMO

    protected virtual void OnDrawGizmos()
    {
        if (!showGizmo)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            soundDistance
        );
    }

    #endregion
}