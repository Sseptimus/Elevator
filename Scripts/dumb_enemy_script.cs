using Godot;
using System;

public partial class dumb_enemy_script : CharacterBody2D
{
	CharacterBody2D player;
	AnimationPlayer enemy_anim;
	TextureProgressBar healthbar;
	TextureProgressBar health;
	private NavigationAgent2D _navigationAgent;
	private float _movementSpeed = 200.0f;
	CollisionShape2D collider;
	bool knockback = false;
	AnimatedSprite2D sprite;
	AnimationPlayer animator;
	public Vector2 MovementTarget
	{
		get { return _navigationAgent.TargetPosition; }
		set { _navigationAgent.TargetPosition = value; }
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().Root.GetNode<CharacterBody2D>($"Main/Player");
		enemy_anim = GetNode<AnimationPlayer>("WeaponAnimation");
		healthbar = player.GetNode<TextureProgressBar>("Visuals_Container/Health_Bar_Container/Health_Bar");
		health = GetNode<TextureProgressBar>("Health_Bar_Container/Health_Bar");
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		collider = GetNode<CollisionShape2D>("CollisionShape2D");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animator = GetNode<AnimationPlayer>("AnimatedSprite2D/AnimationPlayer");
		animator.Play("-1 0");

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
		if (_navigationAgent.IsNavigationFinished())
		{
			return;
		}

		Vector2 currentAgentPosition = GlobalTransform.Origin;
		Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
		health.GetParent<Node2D>().GlobalRotation = 0;
		Vector2 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
		newVelocity *= _movementSpeed;
		if (Position.DistanceTo(player.Position) <= 70)
		{
			newVelocity = Vector2.Zero;
		}
		if(knockback){
			Velocity = -newVelocity*1.5f;
		}else{
		Velocity = newVelocity;
		}
		MoveAndSlide();
		Area2D a = GetNode<Area2D>("Hitbox_container");
		a.LookAt(player.Position);
		if (Position.DistanceTo(player.Position) <= 150)
		{
			enemy_anim.Play("Enemy_melee");
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
			hit();
		}
	}
	async void hit(){
		healthbar.Value -= 5;
		player.GetNode<AnimatedSprite2D>("Visuals_Container/AnimatedSprite2D").Modulate = new Color(1, 0, 0, 1);
		await ToSignal(GetTree().CreateTimer(0.3),"timeout");
		player.GetNode<AnimatedSprite2D>("Visuals_Container/AnimatedSprite2D").Modulate = new Color(1, 1, 1, 1);

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
