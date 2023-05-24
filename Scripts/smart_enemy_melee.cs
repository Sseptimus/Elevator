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
	bool attack_delayed;
    public Vector2 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetTree().Root.GetNode<CharacterBody2D>($"Main/Player");
        enemy_anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
        healthbar = player.GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
        health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        collider = GetNode<CollisionShape2D>("CollisionShape2D");
		timer = GetNode<Timer>("Attack_timer");

        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 4.0f;
        _navigationAgent.TargetDesiredDistance = 4.0f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        ActorSetup();
        Vector2 velocity = Velocity;
        Vector2 current = Vector2.Zero;
        LookAt(player.Position);

        Vector2 currentAgentPosition = GlobalTransform.Origin;
        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

        Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        
		if(attack_delayed){
			newVelocity = -newVelocity;
		}
        else if (Position.DistanceTo(player.Position) <= 50)
        {
            newVelocity = Vector2.Zero;
        }
		newVelocity *= _movementSpeed;
        Velocity = newVelocity;

        MoveAndSlide();
        if (Position.DistanceTo(player.Position) <= 70)
        {
            enemy_anim.Play("Enemy_melee");
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
            healthbar.Value -= 5;
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
}
