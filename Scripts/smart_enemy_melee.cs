using Godot;
using System;

public partial class smart_enemy_melee : CharacterBody2D
{
    CharacterBody2D player;
    AnimationPlayer enemy_anim;
    TextureProgressBar healthbar;
    TextureProgressBar health;
    private NavigationAgent2D _navigationAgent;
    private float _movementSpeed = 200.0f;
    CollisionShape2D collider;
	Timer timer;
	bool attack_delayed = false;
    bool knockback = false;
    AnimationPlayer animator;
    Timer hurt_timer;
    Random rnd;
    public Vector2 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rnd = new Random();
        player = GetTree().Root.GetNode<CharacterBody2D>($"Main/Player");
        enemy_anim = GetNode<AnimationPlayer>("WeaponAnimation");
        healthbar = player.GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
        health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
        health.MaxValue = 100*Upgrades.EnemyHealthMultiplier;
        health.Value = 100*Upgrades.EnemyHealthMultiplier;
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        collider = GetNode<CollisionShape2D>("CollisionShape2D");
		timer = GetNode<Timer>("Attack_timer");
        hurt_timer = GetTree().Root.GetNode<Timer>("Main/Player/Hurt_Timer");
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        GetNode<AnimatedSprite2D>("AnimatedShirtSprite").Modulate = new Color((float)rnd.Next(0,100)/100,(float)rnd.Next(0,100)/100,(float)rnd.Next(0,100)/100);
		GetNode<AnimatedSprite2D>("AnimatedPantsSprite").Modulate = new Color((float)rnd.Next(0,100)/100,(float)rnd.Next(0,100)/100,(float)rnd.Next(0,100)/100);
        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 4.0f;
        _navigationAgent.TargetDesiredDistance = 4.0f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _PhysicsProcess(double delta)
    {
        
        ActorSetup();
        Vector2 velocity = Velocity;
        Vector2 current = Vector2.Zero;

        Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

        Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        
		
        if (Position.DistanceTo(MovementTarget) <= 50)
        {
            newVelocity = Vector2.Zero;
        }
		newVelocity *= _movementSpeed;
        newVelocity += Vector2.One*Mathf.Log(player.Position.DistanceTo(Position)+50);
		float angle = newVelocity.Angle();
		float look_direction = Mathf.Round(angle/(Mathf.Pi/2))*(Mathf.Pi/2);
		Vector2 aim_direction;
		aim_direction.X = MathF.Round(Mathf.Cos(look_direction));
		aim_direction.Y = MathF.Round(MathF.Sin(look_direction));
        if (aim_direction.X == -0){
			aim_direction.X = 0;
		}if (aim_direction.Y == -0){
			aim_direction.Y = 0;
		}
		animator.Play($"{aim_direction.X} {aim_direction.Y}");
        if(attack_delayed && Position.Y > 120){
			newVelocity = -newVelocity;
		}else if (knockback){
            newVelocity = -newVelocity * 1.5f;
        }
        Velocity = newVelocity;
        MoveAndSlide();
        if (Position.DistanceTo(player.Position) <= 70)
        {
            enemy_anim.Play($"Enemy Attack {aim_direction.X} {aim_direction.Y}");
            await ToSignal(enemy_anim, "animation_finished");
			attack_delay();
        }
        if (health.Value <= 0)
        {
            QueueFree();
        }
        collider.GlobalRotation = 0;
    }
   public void _on_hitbox_container_area_entered(Area2D area)
    {
        if (area.Name == "Player_hitbox_container")
        {
            healthbar.Value -= 3*Upgrades.EnemyDamageMultiplier;
            hurt_timer.Start();
        }
    }
    
	private async void attack_delay(){
		attack_delayed = true;
		timer.Start();
		await ToSignal(timer, "timeout");
		attack_delayed = false;
	}
    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
		MovementTarget = player.Position;
    }
    public void _on_health_bar_value_changed(float value){
        knockback_delay();
    }
    private async void knockback_delay(){
        knockback = true;
        await ToSignal(GetTree().CreateTimer(0.05),"timeout");
        knockback = false;
    }
}
