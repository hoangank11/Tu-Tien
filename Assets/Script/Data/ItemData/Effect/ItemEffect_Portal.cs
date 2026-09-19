using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Portal Scroll", fileName = "Item Effect - Portal")]
public class ItemEffect_Portal : ItemEffectDataSO
{
    public override void ExecuteEffect()
    {
        if (SceneManager.GetActiveScene().name == "Sân Thanh Vân Môn")
        {
            return;
        }

        Player player = Player.instance;
        Vector3 portalPosition = player.transform.position + new Vector3(player.facingDir * 4f, 0);
        Object_Portal.instance.ActivatePortal(portalPosition, player.facingDir);

        //Object_Portal.instance.ActivatePortal(player.facingDir);
    }
}
