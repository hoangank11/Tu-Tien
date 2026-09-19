public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        start = player.start;
        skillManager = player.skillManager;
    }


    public override void Update()
    {
        base.Update();

        /* Đại khái là có thể sài skill ngay cả khi ở trên không. 
         Căn bản chỉ dành cho dash hoặc 1 vài skill ngoại lệ. 
         Còn nếu k thì sài skill ở groundState */
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
            stateMachine.ChangeState(player.dashState);
    }

    public override void UpdateAnimParameter()
    {
        base.UpdateAnimParameter();

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private bool CanDash()
    {
        if (skillManager.dash.CanUseSkill() == false)
            return false;

        if (player.wallDetected)
            return false;

        if (stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainState)
            return false;
        return true;
    }
}
