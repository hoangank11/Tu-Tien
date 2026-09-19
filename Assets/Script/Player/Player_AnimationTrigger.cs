public class Player_AnimationTrigger : Entity_AnimationTriggers
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<Player>();
    }

    private void ThrowSword() => player.skillManager.sword.ThrowSword();
    private void ThrowFire() => player.skillManager.fire.ThrowFire();
    private void ThrowHand() => player.skillManager.hand01.ThrowHand();

}
